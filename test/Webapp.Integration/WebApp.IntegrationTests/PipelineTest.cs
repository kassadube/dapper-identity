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
using Xunit.Abstractions;
using System.Net;

namespace WebApp.IntegrationTests
{
    public class PipelineTest : IDisposable
    {
        const string Category = "Authorize endpoint";
        SitePipeline _mockPipeline = new SitePipeline();
        private readonly ITestOutputHelper output;
        
        public PipelineTest(ITestOutputHelper output)
        {
            this.output = output;
            _mockPipeline.Initialize();
            
        }

        [Fact()]
         [Trait("Category", Category)]
        public async Task HomePage200()
        {
           var response = await _mockPipeline.Client.GetAsync(SitePipeline.HomePage);
           output.WriteLine("This is output from {0}", response.StatusCode);
            response.EnsureSuccessStatusCode();
        }
        
        [Fact()]
         [Trait("Category", Category)]
        public async Task AuthorizePage()
        {
           //var response = await _mockPipeline.Client.GetAsync(SitePipeline.AuthorizeUrl);
          // output.WriteLine("This is output from {0}", response.StatusCode);
          // _mockPipeline.BrowserClient.AllowAutoRedirect = false;
           var response = await _mockPipeline.Client.GetAsync(SitePipeline.AuthorizeUrl);
            response.StatusCode.Should().Be(HttpStatusCode.Redirect);
            var responseString = await response.Content.ReadAsStringAsync();
           
            
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
                    _mockPipeline.Server.Dispose();
                    _mockPipeline.Client.Dispose();
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
