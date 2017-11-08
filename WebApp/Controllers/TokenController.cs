using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebApp.Models;
using WebApp.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [Route("token")]
    [AllowAnonymous]
    public class TokenController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        
        public TokenController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<TokenController> logger)
        {
              _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]LoginInputModel inputModel)
        {
            _logger.LogInformation("TRY LOGIN TOKEN");
            var result = await _signInManager.PasswordSignInAsync
                (inputModel.Username, inputModel.Password, false,  false);
               if (!result.Succeeded)
                {
                    _logger.LogInformation("User NOT logged in.");
                    return Unauthorized();
                }

          //  if (inputModel.Username != "james" && inputModel.Password != "bond")
          //      return Unauthorized();

            var token = new JwtTokenBuilder()
                                .AddSecurityKey(JwtSecurityKey.Create("fiver-secret-key"))
                                .AddSubject("james bond")
                                .AddIssuer("Fiver.Security.Bearer")
                                .AddAudience("Fiver.Security.Bearer")
                                .AddClaim("MembershipId", "111")
                                .AddExpiry(1)
                                .Build();

            //return Ok(token);
            return Ok(token.Value);
        }
    }
}
