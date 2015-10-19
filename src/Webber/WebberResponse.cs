namespace Webber
{
    /// <summary>
    ///     A generic Webber Response
    /// </summary>
    public class WebberResponse
    {
        /// <summary>
        ///     ContentType of the response
        /// </summary>
        public string ContentType;

        /// <summary>
        ///     The raw non-derialized response from the request
        /// </summary>
        public string RawResult;

        /// <summary>
        ///     HTTP Status Code of the response
        /// </summary>
        public short StatusCode;

        /// <summary>
        ///     A flag that indicates that a valid responses was received and that there were no exceptions
        /// </summary>
        public bool Success;
    }

    public class WebberResponse<T> : WebberResponse
    {
        public T Result;

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
    }
}