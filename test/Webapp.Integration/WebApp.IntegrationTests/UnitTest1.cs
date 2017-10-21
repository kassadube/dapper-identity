using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using WebApp.IntegrationTests.Common;
using Xunit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WebApp.IntegrationTests
{
    public class UnitTest1
    {
        SitePipeline _mockPipeline = new SitePipeline();
         private readonly TestServer _server;
private readonly HttpClient _client;
        public UnitTest1()
        {
            var config = new ConfigurationBuilder()            
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory().ToString(), @"..\..\..\..\..\..\webapp"))
            .AddJsonFile("appsettings.testing.json", optional: true)
            .Build();
            _mockPipeline.Initialize();
             _server = new TestServer(
                 new WebHostBuilder()
                 .UseConfiguration(config)
                    .UseStartup<StartupTest>().UseContentRoot(@"..\..\..\..\..\..\webapp"));
            _client = _server.CreateClient();
        }

        [Fact()]
        public async Task ttt()
        {
            Console.WriteLine( _client.BaseAddress);
            var response = await _client.GetAsync("/home/index2");
            response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();
        responseString.Should().Be("{\"id\":3,\"url\":\"http://localhost/home/index2\",\"conn\":\"Data Source=database.test.db\",\"env\":\"Development\"}");
        //return await response.Content.ReadAsStringAsync();
        }
        [Fact(DisplayName="tttt")]
        public void PassingTest()
        {
            Assert.Equal(4, Add(2, 2));
        }

        [Fact(Skip="reason")]
        public void FailingTest()
        {
            Assert.Equal(5, Add(2, 2));
        }

        int Add(int x, int y)
        {
            return x + y;
        }
    }
}
