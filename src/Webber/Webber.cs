using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Webber
{
    public class Webber
    {
        public static Func<bool> PreInvokeHanadler;
        public static Func<bool> PostInvokeHandler;
        public static Action<WebberResponse> InvokeOnErrorHandler;
        public static string AppName;

        /// <summary>
        /// Set defaults
        /// </summary>
        static Webber()
        {
            AppName = "Webber";
        }

        public static WebberResponse Post(
                                string url,
                                string data = "",
                                string contentType = ContentTypes.Json,
                                ICredentials credentials = null,
                                NameValueCollection customHeaders = null)
        {
            return Invoke(url, data, contentType, MethodType.Post, credentials, customHeaders);
        }

        public static WebberResponse<T> Post<T>(
            string url,
            string data = "",
            string contentType = ContentTypes.Json,
            ICredentials credentials = null,
            NameValueCollection customHeaders = null) where T : new()
        {
            var webberResponse = Post(url, data, contentType, credentials, customHeaders);

            return GetDeserializedResponse<T>(webberResponse);
        }

        public static WebberResponse Get(
                        string url,
                        NameValueCollection customHeaders = null)
        {
            return Invoke(url, null, null, MethodType.Get, null, customHeaders);
        }

        public static WebberResponse<T> Get<T>(
            string url,
            NameValueCollection customHeaders = null) where T : new()
        {
            var webberResponse = Get(url, customHeaders);

            return GetDeserializedResponse<T>(webberResponse);
        }

        public static WebberResponse Invoke(
                                string url,
                                string data = "",
                                string contentType = ContentTypes.Json,
                                string methodType = MethodType.Post,
                                ICredentials credentials = null,
                                NameValueCollection customHeaders = null)
        {
            var webberResponse = new WebberResponse();

            try
            {
                var uri = new Uri(url);
                var request = (HttpWebRequest) WebRequest.Create(uri);

                request.Credentials = credentials;
                request.Method = methodType;
                request.ContentType = contentType;
                request.UserAgent = AppName;
                request.KeepAlive = false;

                if (!string.IsNullOrEmpty(data))
                {
                    request.ContentLength = data.Length;

                    using (Stream writeStream = request.GetRequestStream())
                    {
                        var encoding = new UTF8Encoding();
                        byte[] bytes = encoding.GetBytes(data);
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                if (customHeaders != null) request.Headers.Add(customHeaders);

                var httpWebResponse = (HttpWebResponse)request.GetResponse();
                var responseStream = httpWebResponse.GetResponseStream() ?? new MemoryStream();

                using (var readStream = new StreamReader(responseStream, Encoding.UTF8))
                {
                    webberResponse.RawResult = readStream.ReadToEnd();
                }

                webberResponse.StatusCode = (short)httpWebResponse.StatusCode;
                webberResponse.Success = true;
            }
            catch (Exception exception)
            {
                webberResponse.RawResult = exception.ToString();
                webberResponse.StatusCode = -1;

                OnError(webberResponse);
            }

            return webberResponse;
        }

        private static WebberResponse<T> GetDeserializedResponse<T>(WebberResponse webberResponse) where T : new()
        {
            WebberResponse<T> response = new WebberResponse<T>(webberResponse);

            if (webberResponse.Success)
            {
                try
                {
                    response.Result = JsonConvert.DeserializeObject<T>(webberResponse.RawResult);
                }
                catch (JsonSerializationException ex)
                {
                    OnError(response, ex);

                    response.Result = new T();
                }
            }

            return response;
        }

        private static void OnError(WebberResponse response)
        {
            try
            {
                InvokeOnErrorHandler?.Invoke(response);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                Trace.WriteLine(response.RawResult);
            }
        }

        private static void OnError<T>(WebberResponse response, T exception) where T : Exception
        {
            try
            {
                Trace.WriteLine(exception);

                InvokeOnErrorHandler?.Invoke(response);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                Trace.WriteLine(response.RawResult);
            }
        }
    }
    
    /// <summary>
    /// A generic Webber Response
    /// </summary>
    public class WebberResponse
    {
        public short StatusCode;
        public bool Success;
        public string RawResult;
    }

    public class WebberResponse<T> : WebberResponse
    {
        public WebberResponse()
        {
            
        }

        public WebberResponse(WebberResponse response)
        {
            if (response != null)
            {
                StatusCode = response.StatusCode;
                Success = response.Success;
                RawResult = response.RawResult;
            }
        }

        public T Result;
    }

    /// <summary>
    /// MIME Types for a post request
    /// </summary>
    public static class ContentTypes
    {
        public const string FormEncodedData = "application/x-www-form-urlencoded";
        public const string AtomFeeds = "application/atom+xml";
        public const string Json = "application/json";
        public const string Javascript = "application/javascript";
        public const string Soap = "application/soap+xml";
        public const string Xml = "text/xml";
        public const string Html = "text/html";
    }

    /// <summary>
    /// Method Types for making a Web Request
    /// </summary>
    public static class MethodType
    {
        public const string Post = "POST";
        public const string Get = "GET";
        public const string Put = "PUT";
        public const string Patch = "PATCH";
    }
}
