using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Webber.Tests
{
    [TestClass]
    public class PostTests
    {
        [TestMethod]
        public void CanDeserializeSuccessfulPostResponse()
        {
            string url = "http://jsonplaceholder.typicode.com/posts";

            var data = new
            {
                title = "Webber TestTitle " + DateTime.Now.Ticks,
                body = "Webber TestBody " + DateTime.Now.Ticks,
                userId = 404
            };

            var request = JsonConvert.SerializeObject(data);

            var webberResponse = Webber.Post<SamplePost>(url, request);

            Console.WriteLine(webberResponse.RawResult);

            Assert.IsNotNull(webberResponse);
            Assert.IsTrue(webberResponse.Success);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(webberResponse.RawResult));
            Assert.IsNotNull(webberResponse.Result);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(webberResponse.Result.Body));
            Assert.IsTrue(!string.IsNullOrWhiteSpace(webberResponse.Result.Title));
            Assert.IsTrue(webberResponse.Result.UserId > 0);
            Assert.IsTrue(webberResponse.Result.Id > 0);
        }

        public class SamplePost
        {
            public int UserId;
            public int Id;
            public string Title;
            public string Body;
        }
    }
}
