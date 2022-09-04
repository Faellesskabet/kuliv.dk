using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.DataProtection;

namespace Data

{ 
    [ApiController]
    public class AccountController : ControllerBase
    {
        
        [HttpGet("/login")]
        [HttpPost("/login")]
        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            return this.Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, DiscordAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet("/logout")]
        [HttpPost("/logout")]
        public async Task<SignOutResult> LogOut(string returnUrl = "/") => this.SignOut(new AuthenticationProperties { RedirectUri = returnUrl },
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
