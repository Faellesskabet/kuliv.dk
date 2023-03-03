﻿using System;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WayfJwtConnector;
using WayfJwtConnector.Models;

namespace Dikubot.Webapp.Authentication;

[ApiController]
public class AccountController : ControllerBase
{

    private WayfClient _wayfClient;
    public AccountController(WayfClient wayfClient)
    {
        _wayfClient = wayfClient;
    }
    
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
        [HttpGet("/login/wayf")]
        [HttpPost("/login/wayf")]
        public async Task<IActionResult> WayfLogin()
        {
            string url = await _wayfClient.RedirectUrl();
            return Redirect(url);
        }
    
        [HttpPost("/wayf/login")]
        public async Task<IActionResult> ValidateWayfLogin()
        {
            WayfClaims data = await _wayfClient.ValidateAsync(Request.Body);
            return Ok(data);
        }
        
            
        [HttpPost("/wayf/ls")]
        public async Task<IActionResult> ValidateWayf()
        {
            WayfClaims data = await _wayfClient.ValidateAsync(Request.Body);
            return Ok(data);
        }
}