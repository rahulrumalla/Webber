namespace Xizmark.Webber
{
    /// <summary>
    ///     MIME Types for a post request
    /// </summary>
    public static class ContentType
    {
        public const string ApplicationFormEncodedData = "application/x-www-form-urlencoded";
        public const string ApplicationAtomFeeds = "application/atom+xml";
        public const string ApplicationJson = "application/json";
        public const string ApplicationJavascript = "application/javascript";
        public const string ApplicationSoap = "application/soap+xml";
        public const string ApplicationOctetStream = "application/octet-stream";

        public const string TextXml = "text/xml";
        public const string TextHtml = "text/html";
        public const string TextCsv = "text/csv";

        public const string MultiPartFormData = "multipart/form-data";
    }
}