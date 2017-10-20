
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Net;
using IdentityModel.Client;
using FluentAssertions;
using System.Threading;

namespace WebApp.IntegrationTests.Common
{

    public class SitePipeline
    {
        public const string BaseUrl = "https://server";
        public const string LoginPage = BaseUrl + "/account/login";
        public const string ErrorPage = BaseUrl + "/home/error";
        public TestServer Server { get; set; }
        public HttpMessageHandler Handler { get; set; }
        public BrowserClient BrowserClient { get; set; }
        public HttpClient Client { get; set; }
        
        public event Action<IServiceCollection> OnPreConfigureServices = services => { };
        public event Action<IServiceCollection> OnPostConfigureServices = services => { };
        public event Action<IApplicationBuilder> OnPreConfigure = app => { };
        public event Action<IApplicationBuilder> OnPostConfigure = app => { };

        public BackChannelMessageHandler BackChannelMessageHandler { get; set; } = new BackChannelMessageHandler();

        public void Initialize(string basePath = null)
        {
           // var builder = new WebHostBuilder();
           // builder.ConfigureServices(ConfigureServices);
            

            Server =new TestServer(new WebHostBuilder()
            .UseStartup<Startup>());
            Handler = Server.CreateHandler();
            
            BrowserClient = new BrowserClient(new BrowserHandler(Handler));
            Client = new HttpClient(Handler);
        }

         public void ConfigureServices(IServiceCollection services)
        {
            OnPreConfigureServices(services);

            OnPostConfigureServices(services);
        }

        public void ConfigureApp(IApplicationBuilder app)
        {
            OnPreConfigure(app);

            

            // UI endpoints
               OnPostConfigure(app);
        }

        public bool LoginWasCalled { get; set; }
        
        public ClaimsPrincipal Subject { get; set; }
        public bool FollowLoginReturnUrl { get; set; }
        
        

        
    }

    public class BackChannelMessageHandler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, Task> OnInvoke { get; set; }
        public HttpResponseMessage Response { get; set; } = new HttpResponseMessage(HttpStatusCode.OK);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (OnInvoke != null)
            {
                await OnInvoke.Invoke(request);
            }
            return Response;
        }
    }

    public class MockExternalAuthenticationHandler : 
        IAuthenticationHandler,
        IAuthenticationSignInHandler,
        IAuthenticationRequestHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        HttpContext HttpContext => _httpContextAccessor.HttpContext;

        public Func<HttpContext, Task<bool>> OnFederatedSignout = 
            async context =>
            {
                await context.SignOutAsync();
                return true;
            };

        public MockExternalAuthenticationHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> HandleRequestAsync()
        {
            

            return await Task.FromResult( false);
        }

        public Task<AuthenticateResult> AuthenticateAsync()
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        public Task ChallengeAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            return Task.CompletedTask;
        }

        public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }

        public Task SignOutAsync(AuthenticationProperties properties)
        {
            return Task.CompletedTask;
        }
    }


public class BackChannelHttpClient : HttpClient
    {
        public BackChannelHttpClient()
        {
        }

        public BackChannelHttpClient(HttpMessageHandler handler) : base(handler)
        {
        }
    }
}