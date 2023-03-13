using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.User.DiscordUser;
using Dikubot.Discord;
using Dikubot.Webapp.Authentication.Discord.OAuth2;
using Dikubot.Webapp.Authentication.Identities;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;

namespace Dikubot.Webapp.Authentication;

public class Authenticator : AuthenticationStateProvider
{
    private readonly IHttpContextAccessor _httpContextAccssor;
    private readonly DiscordUserGlobalMongoService _discordUserGlobalMongoService;
    private SocketGuild _guild;

    public Authenticator(IHttpContextAccessor httpContextAccssor,
        DiscordUserGlobalMongoService discordUserGlobalMongoService)
    {
        _httpContextAccssor = httpContextAccssor;
        _discordUserGlobalMongoService = discordUserGlobalMongoService;
    }

    /// <summary>
    ///     Get the current UserGlobalModel
    /// </summary>
    /// <returns>Returns a nullable UserGlobalModel based on the current session</returns>
    public async Task<DiscordUserGlobalModel> GetUserGlobal()
    {
        AuthenticationState authState = await GetAuthenticationStateAsync();
        DiscordIdentity discord = (DiscordIdentity)authState.User.Identity!;
        return discord?.DiscordUserGlobalModel;
    }

    /// <summary>
    ///     Gets the user's discord oauth2 access token
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public async Task<string> GetTokenAsync()
    {
        if (_httpContextAccssor?.HttpContext?.User.Identity == null) return null;
        if (!_httpContextAccssor.HttpContext.User.Identity.IsAuthenticated) return null;

        string tk = await _httpContextAccssor.HttpContext.GetTokenAsync("Discord", "access_token");
        return tk;
    }

    /// <summary>
    ///     Get SocketGuild of the current session
    /// </summary>
    /// <returns></returns>
    public async Task<SocketGuild> GetSocketGuild()
    {
        return await GetSocketGuild(await GetUserGlobal());
    }

    private async Task<SocketGuild> GetSocketGuild(DiscordUserGlobalModel discordUser)
    {
        return discordUser == null ? null : DiscordBot.ClientStatic.GetGuild(discordUser.SelectedGuild);
    }


    private DiscordUserClaim GetDiscordUserClaim()
    {
        if (_httpContextAccssor.HttpContext == null) return null;
        return GetInfo(_httpContextAccssor.HttpContext);
    }


    public DiscordUserClaim GetInfo(HttpContext httpContext)
    {
        if (!httpContext.User.Identity?.IsAuthenticated ?? false) return null;

        IEnumerable<Claim> claims = httpContext.User.Claims;
        bool? verified;
        if (bool.TryParse(claims.FirstOrDefault(x => x.Type == "urn:discord:verified")?.Value, out bool _verified))
            verified = _verified;
        else
            verified = null;

        DiscordUserClaim userClaim = new DiscordUserClaim
        {
            UserId = ulong.Parse(claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value),
            Name = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value,
            Discriminator = claims.FirstOrDefault(x => x.Type == "urn:discord:discriminator")?.Value,
            Avatar = claims.FirstOrDefault(x => x.Type == "urn:discord:avatar")?.Value,
            Email = claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
            Verified = verified
        };

        return userClaim;
    }

    /// <summary>
    ///     GetAuthenticationStateAsync is used to set the AuthenticationState based on a UserIdentity.
    /// </summary>
    /// <returns>Task</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        DiscordUserClaim discordUserClaim = GetDiscordUserClaim();
        if (discordUserClaim == null)
            //Returns an empty UserIdentity, meaning the user isn't authorized to do anything
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new DiscordIdentity())));

        //Here we return a UserIdentity with our sessionModel. The system will then check the user connected to the session,
        //to see what authorisation step the user is at.
        return await Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new DiscordIdentity(discordUserClaim, _discordUserGlobalMongoService))));
    }

    /// <summary>
    ///     Updates the session in localStorage as well as tells the AuthenticationState that the session has been updated
    /// </summary>
    /// <param name="sessionModel">The current session will be set to this model</param>
    /// <returns>Task</returns>
    public async Task UpdateSession()
    {
        DiscordUserClaim discordUserClaim = GetDiscordUserClaim();
        DiscordIdentity discordIdentity =
            discordUserClaim == null ? new DiscordIdentity() : new DiscordIdentity(discordUserClaim, _discordUserGlobalMongoService);
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(new ClaimsPrincipal(discordIdentity))));
    }
}