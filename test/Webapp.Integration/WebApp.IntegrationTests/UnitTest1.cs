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
    public class UnitTest1 : IDisposable
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
                    .UseStartup<Startup>().UseContentRoot(@"..\..\..\..\..\..\webapp"));
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _server.Dispose();
                    _client.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UnitTest1() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
