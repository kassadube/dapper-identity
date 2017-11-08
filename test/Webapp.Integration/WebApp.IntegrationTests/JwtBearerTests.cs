
using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using WebApp.IntegrationTests.Common;
using Xunit;

namespace WebApp.IntegrationTests
{
    public class JwtBearerTests
    {
        SitePipeline _mockPipeline = new SitePipeline();
        
        public JwtBearerTests()
        {
            _mockPipeline.Initialize();
        }
        [Fact]
        public async System.Threading.Tasks.Task VerifySchemeDefaults()
        {
            var services = new ServiceCollection();
            services.AddAuthentication().AddJwtBearer();
            var sp = services.BuildServiceProvider();
            var schemeProvider = sp.GetRequiredService<IAuthenticationSchemeProvider>();
            var scheme = await schemeProvider.GetSchemeAsync(JwtBearerDefaults.AuthenticationScheme);
            Assert.NotNull(scheme);
            Assert.Equal("JwtBearerHandler", scheme.HandlerType.Name);
            Assert.Null(scheme.DisplayName);
        }

        [Fact]
        public async Task SignInThrows()
        {
            var server = CreateServer();
            var transaction = await server.SendAsync("https://example.com/signIn");
            Assert.Equal(HttpStatusCode.OK, transaction.Response.StatusCode);
        }

        private static TestServer CreateServer(Action<JwtBearerOptions> options = null, Func<HttpContext, Func<Task>, Task> handlerBeforeAuth = null)
        {
            var builder = new WebHostBuilder()
                .Configure(app =>
                {
                    if (handlerBeforeAuth != null)
                    {
                        app.Use(handlerBeforeAuth);
                    }

                    app.UseAuthentication();
                    app.Use(async (context, next) =>
                    {
                        if (context.Request.Path == new PathString("/checkforerrors"))
                        {
                            var result = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme); // this used to be "Automatic"
                            if (result.Failure != null)
                            {
                                throw new Exception("Failed to authenticate", result.Failure);
                            }
                            return;
                        }
                        else if (context.Request.Path == new PathString("/oauth"))
                        {
                            if (context.User == null ||
                                context.User.Identity == null ||
                                !context.User.Identity.IsAuthenticated)
                            {
                                context.Response.StatusCode = 401;
                                // REVIEW: no more automatic challenge
                                await context.ChallengeAsync(JwtBearerDefaults.AuthenticationScheme);
                                return;
                            }

                            var identifier = context.User.FindFirst(ClaimTypes.NameIdentifier);
                            if (identifier == null)
                            {
                                context.Response.StatusCode = 500;
                                return;
                            }

                            await context.Response.WriteAsync(identifier.Value);
                        }
                        else if (context.Request.Path == new PathString("/unauthorized"))
                        {
                            // Simulate Authorization failure 
                            var result = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                            await context.ChallengeAsync(JwtBearerDefaults.AuthenticationScheme);
                        }
                        else if (context.Request.Path == new PathString("/signIn"))
                        {
                            await Assert.ThrowsAsync<InvalidOperationException>(() => context.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal()));
                        }
                        else if (context.Request.Path == new PathString("/signOut"))
                        {
                            await Assert.ThrowsAsync<InvalidOperationException>(() => context.SignOutAsync(JwtBearerDefaults.AuthenticationScheme));
                        }
                        else
                        {
                            await next();
                        }
                    });
                })
                .ConfigureServices(services => services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options));

            return new TestServer(builder);
        }
         private static async Task<Transaction> SendAsync(TestServer server, string uri, string authorizationHeader = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                request.Headers.Add("Authorization", authorizationHeader);
            }

            var transaction = new Transaction
            {
                Request = request,
                Response = await server.CreateClient().SendAsync(request),
            };

            transaction.ResponseText = await transaction.Response.Content.ReadAsStringAsync();

            if (transaction.Response.Content != null &&
                transaction.Response.Content.Headers.ContentType != null &&
                transaction.Response.Content.Headers.ContentType.MediaType == "text/xml")
            {
                transaction.ResponseElement = XElement.Parse(transaction.ResponseText);
            }

            return transaction;
        }
    }
}