using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Webber
{
    public static class Webber
    {
        public static Func<bool> PreInvokeHanadler;
        public static Func<bool> PostInvokeHandler;
        public static Func<bool> InvokeOnErrorHandler;

        public static WebberResponse POST(
                                string url,
                                string data = "",
                                string contentType = ContentTypes.JSON,
                                ICredentials credentials = null,
                                NameValueCollection customHeaders = null)
        {
            return Invoke(url, data, contentType, MethodType.POST, credentials, customHeaders);
        }

        public static WebberResponse GET(
                        string url,
                        NameValueCollection customHeaders = null)
        {
            return Invoke(url, null, null, MethodType.GET, null, customHeaders);
        }

        public static WebberResponse Invoke(
                                string url,
                                string data = "",
                                string contentType = ContentTypes.JSON,
                                string methodType = MethodType.POST,
                                ICredentials credentials = null,
                                NameValueCollection customHeaders = null)
        {
            var webberResponse = new WebberResponse();
            HttpWebResponse httpWebResponse = null;
            Stream responseStream = null;

            try
            {
                var uri = new Uri(url);
                var request = (HttpWebRequest) WebRequest.Create(uri);

                request.Credentials = credentials;
                request.Method = methodType;
                request.ContentType = contentType;
                request.UserAgent = "Webber";

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

                httpWebResponse = (HttpWebResponse)request.GetResponse();
                responseStream = httpWebResponse.GetResponseStream() ?? new MemoryStream();

                using (var readStream = new StreamReader(responseStream, Encoding.UTF8))
                {
                    webberResponse.RawResult = readStream.ReadToEnd();
                }

                webberResponse.StatusCode = (short)httpWebResponse.StatusCode;
                webberResponse.Success = httpWebResponse.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception exception)
            {
                webberResponse.RawResult = exception.ToString();
                webberResponse.StatusCode = -1;
            }
            finally
            {
            }

            return webberResponse;
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

    /// <summary>
    /// MIME Types for a post request
    /// </summary>
    public static class ContentTypes
    {
        public const string FormEncodedData = "application/x-www-form-urlencoded";
        public const string AtomFeeds = "application/atom+xml";
        public const string JSON = "application/json";
        public const string Javascript = "application/javascript";
        public const string SOAP = "application/soap+xml";
        public const string Xml = "text/xml";
        public const string Html = "text/html";
    }

    /// <summary>
    /// Method Types for making a Web Request
    /// </summary>
    public static class MethodType
    {
        public const string POST = "POST";
        public const string GET = "GET";
        public const string PUT = "PUT";
        public const string PATCH = "PATCH";
    }
}
