
using System.Net.Http;
using Microsoft.AspNetCore.TestHost;

namespace WebApp.IntegrationTests.Common
{

    public class SitePipeline
    {
        public const string BaseUrl = "https://server";
        public const string LoginPage = BaseUrl + "/account/login";
        public const string ErrorPage = BaseUrl + "/home/error";
        public TestServer Server { get; set; }
        public HttpMessageHandler Handler { get; set; }
    }
}