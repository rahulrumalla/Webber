using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Xizmark.Webber
{
    public class Webber
    {
        /// <summary>
        ///     Callback that gets invoked if and when an error is occured during the request
        /// </summary>
        public static Action<WebberResponse> InvokeOnErrorHandler;

        /// <summary>
        ///     Assign the Applciation's name. User Agent of the request is populated witht this field
        /// </summary>
        public static string AppName;

        /// <summary>
        ///     Set defaults
        /// </summary>
        static Webber()
        {
            AppName = "Webber";
        }

        /// <summary>
        ///     Perform a HTTP POST request
        /// </summary>
        /// <param name="url">Url of the request</param>
        /// <param name="data">Request payload</param>
        /// <param name="contentType">Content Type of the request. Default is application/json</param>
        /// <param name="encoding">Encoding Type of the request. Default is UTF-8</param>
        /// <param name="credentials">ICredential. Default is NULL</param>
        /// <param name="customHeaders">Additional headers to append to the request. Default is NULL</param>
        /// <returns>Returns a WebberReponse</returns>
        public static WebberResponse Post(
            string url,
            string data = "",
            string contentType = ContentType.ApplicationJson,
            EncodingType encoding = EncodingType.Utf8,
            ICredentials credentials = null,
            NameValueCollection customHeaders = null)
        {
            return Invoke(url, data, contentType, MethodType.Post, encoding, credentials, customHeaders);
        }

        /// <summary>
        ///     Perform a HTTP POST request
        /// </summary>
        /// <typeparam name="T">Type to which the response gets deserialized to</typeparam>
        /// <param name="url">Url of the request</param>
        /// <param name="data">Request payload</param>
        /// <param name="contentType">Content Type of the request. Default is application/json</param>
        /// <param name="encoding">Encoding Type of the request. Default is UTF-8</param>
        /// <param name="credentials">ICredential. Default is NULL</param>
        /// <param name="customHeaders">Additional headers to append to the request. Default is NULL</param>
        /// <returns>Returns a WebberReponse</returns>
        public static WebberResponse<T> Post<T>(
            string url,
            string data = "",
            string contentType = ContentType.ApplicationJson,
            EncodingType encoding = EncodingType.Utf8,
            ICredentials credentials = null,
            NameValueCollection customHeaders = null) where T : new()
        {
            var webberResponse = Post(url, data, contentType, encoding, credentials, customHeaders);

            return GetDeserializedResponse<T>(webberResponse);
        }

        /// <summary>
        ///     Perform a HTTP GET request
        /// </summary>
        /// <param name="url">Url of the request</param>
        /// <param name="encoding">Encoding Type of the request. Default is UTF-8</param>
        /// <param name="customHeaders">Additional headers to append to the request. Default is NULL</param>
        /// <returns>Returns a WebberReponse</returns>
        public static WebberResponse Get(
            string url,
            EncodingType encoding = EncodingType.Utf8,
            NameValueCollection customHeaders = null)
        {
            return Invoke(url, null, null, MethodType.Get, encoding, null, customHeaders);
        }

        /// <summary>
        ///     Perform a HTTP GET request
        /// </summary>
        /// <typeparam name="T">Type to which the response gets deserialized to</typeparam>
        /// <param name="url">Url of the request</param>
        /// <param name="encoding">Encoding Type of the request. Default is UTF-8</param>
        /// <param name="customHeaders">Additional headers to append to the request. Default is NULL</param>
        /// <returns>Returns a WebberReponse</returns>
        public static WebberResponse<T> Get<T>(
            string url,
            EncodingType encoding = EncodingType.Utf8,
            NameValueCollection customHeaders = null) where T : new()
        {
            var webberResponse = Get(url, encoding, customHeaders);

            return GetDeserializedResponse<T>(webberResponse);
        }

        /// <summary>
        ///     Perform a HTTP request
        /// </summary>
        /// <param name="url">Url of the request</param>
        /// <param name="methodType">HTTP Verb type of the reqeust</param>
        /// <param name="data">Request payload</param>
        /// <param name="contentType">Content Type of the request. Default is application/json</param>
        /// <param name="encodingType">Encoding Type of the request. Default is UTF-8</param>
        /// <param name="credentials">ICredential. Default is NULL</param>
        /// <param name="customHeaders">Additional headers to append to the request. Default is NULL</param>
        /// <returns></returns>
        public static WebberResponse Invoke(
            string url,
            string data = "",
            string contentType = ContentType.ApplicationJson,
            string methodType = MethodType.Post,
            EncodingType encodingType = EncodingType.Utf8,
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

                    using (var writeStream = request.GetRequestStream())
                    {
                        var bytes = GetBytes(encodingType, data);
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                if (customHeaders != null) request.Headers.Add(customHeaders);

                var httpWebResponse = (HttpWebResponse) request.GetResponse();
                var responseStream = httpWebResponse.GetResponseStream() ?? new MemoryStream();

                using (var readStream = new StreamReader(responseStream, Encoding.UTF8))
                {
                    webberResponse.RawResult = readStream.ReadToEnd();
                }

                webberResponse.StatusCode = (short) httpWebResponse.StatusCode;
                webberResponse.ContentType = httpWebResponse.ContentType;
                webberResponse.Success = true;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);

                webberResponse.RawResult = exception.ToString();
                webberResponse.StatusCode = -1;

                OnError(webberResponse);
            }

            return webberResponse;
        }

        public static byte[] GetBytes(EncodingType encodingType, string data)
        {
            var encoding = GetEncoding(encodingType);

            return encoding.GetBytes(data);
        }

        public static Encoding GetEncoding(EncodingType encodingType)
        {
            switch (encodingType)
            {
                case EncodingType.Unicode:
                    return new UnicodeEncoding();

                case EncodingType.Ascii:
                    return new ASCIIEncoding();

                case EncodingType.Utf7:
                    return new UTF7Encoding();

                case EncodingType.Utf32:
                    return new UTF32Encoding();

                default:
                    return new UTF8Encoding();
            }
        }

        public static WebberResponse<T> GetDeserializedResponse<T>(WebberResponse webberResponse) where T : new()
        {
            var contentType = webberResponse.ContentType.Split(';')[0];

            if (contentType?.ToLower() != ContentType.ApplicationJson)
                throw new NotSupportedException($"{webberResponse.ContentType} is not supported. " +
                                                "Only JSON is supported for auto-deserialization. Use ");

            WebberResponse<T> response = null;

            if (webberResponse.Success)
            {
                response = new WebberResponse<T>(webberResponse);

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
}