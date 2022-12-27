using System;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Dikubot.Webapp.Authentication;

[ApiController]
public class AccountController : ControllerBase
{
    [HttpGet("/login")]
    [HttpPost("/login")]
    public async Task<IActionResult> Login(string returnUrl = "/")
    {
        return Challenge(new AuthenticationProperties { RedirectUri = returnUrl },
            DiscordAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("/logout")]
    [HttpPost("/logout")]
    public async Task<SignOutResult> LogOut(string returnUrl = "/")
    {
        return SignOut(new AuthenticationProperties { RedirectUri = returnUrl },
            CookieAuthenticationDefaults.AuthenticationScheme);
    }

    [HttpGet("/confirm_email/{uuid}")]
    public RedirectToPageResult ConfirmEmail(Guid uuid)
    {
        return RedirectToPage("/");
    }
}