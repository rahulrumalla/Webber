using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Webber.Tests
{
    [TestClass]
    public class GetTests
    {
        [TestMethod]
        public void Can_make_Get_Request()
        {
            string url = "https://api.github.com/zen";

            WebberResponse webberResponse = Webber.GET(url);

            Console.WriteLine(webberResponse.RawResult);

            Assert.IsNotNull(webberResponse);
            Assert.IsTrue(webberResponse.Success);
            Assert.IsTrue(webberResponse.StatusCode == 200);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(webberResponse.RawResult));
        }
    }
}
