using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Webber.Tests
{
    [TestClass]
    public class GetTests
    {
        [TestMethod]
        public void CanGet()
        {
            string url = "https://api.github.com/zen";

            WebberResponse webberResponse = Webber.Get(url);

            Console.WriteLine(webberResponse.RawResult);

            ValidateResponse(webberResponse);
        }

        [TestMethod]
        public void CanDeserializeSuccessfulGetResponse()
        {
            string url = "http://jsonplaceholder.typicode.com/posts/1";

            var webberResponse = Webber.Get<SamplePost>(url);

            ValidateResponse(webberResponse);

            Assert.IsNotNull(webberResponse.Result);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(webberResponse.Result.Body));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(webberResponse.Result.Title));
            Assert.IsTrue(webberResponse.Result.UserId > 0);
            Assert.IsTrue(webberResponse.Result.Id > 0);
        }

        private void ValidateResponse(WebberResponse webberResponse)
        {
            Assert.IsNotNull(webberResponse);
            Assert.IsTrue(webberResponse.Success);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(webberResponse.RawResult));
        }
    }

    public class SamplePost
    {
        public int UserId;
        public int Id;
        public string Title;
        public string Body;
    }
}
