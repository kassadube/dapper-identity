using System;
using System.Threading.Tasks;
using Alba;
using SiteForTest;
using Xunit;

namespace WebStoryteller
{
    
    public class Class1
    {
        
        [Fact]
        public Task setting_request_headers()
        {
            using (var system = SystemUnderTest.ForStartup<Startup>())
            {
                // This runs an HTTP request and makes an assertion
                // about the expected content of the response
                return system.Scenario(_ =>
                {
                    _.Get.Url("/");
                    _.ContentShouldBe("Hello, World!");
                    _.StatusCodeShouldBeOk();
                });
            }
        }
    }
}
