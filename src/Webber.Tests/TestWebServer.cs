using Nano.Web.Core;
using Nano.Web.Core.Host.HttpListener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webber.Tests
{
    public class TestWebServer
    {
        public static void Start(string url)
        {
            var nanoConfiguration = new NanoConfiguration();
            nanoConfiguration.AddMethods<Beatles>();

            using(HttpListenerNanoServer.Start(nanoConfiguration, url))
            {

            }
        }
    }

    public class Beatles
    {
        public static string[] GetMembers()
        {
            return new string[] { "Paul", "John", "Ringo", "George" };
        }
    }
}
