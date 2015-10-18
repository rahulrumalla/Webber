using System;
using System.Net;
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

            var request = new
            {
                title = "Webber TestTitle " + DateTime.Now.Ticks,
                body = "Webber TestBody " + DateTime.Now.Ticks,
                userId = 404
            };

            var data = JsonConvert.SerializeObject(request);

            var webberResponse = Webber.Post<SamplePost>(url, data);

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
