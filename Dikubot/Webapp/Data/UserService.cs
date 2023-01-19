using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.Webapp.Authentication;
using Discord.WebSocket;
using Microsoft.AspNetCore.Components.Authorization;

namespace Data;

public class UserService
{
    private readonly Authenticator _authenticator;
    private readonly DiscordSocketClient _discordSocketClient;

    public UserService(AuthenticationStateProvider authenticationStateProvider, DiscordSocketClient discordSocketClient)
    {
        _authenticator = (Authenticator)authenticationStateProvider;
        _discordSocketClient = discordSocketClient;
    }

    private UserIdentity User => GetUserIdentity();

    public string Id => User?.DiscordId;

    public UserIdentity GetUserIdentity()
    {
        AuthenticationState authState = _authenticator.GetAuthenticationStateAsync().Result;
        return (UserIdentity)authState.User.Identity;
    }


    /// <summary>
    ///     This do only check if the use is Registered, hence
    ///     if _user?.UserGlobalModel != null
    /// </summary>
    public bool IsRegistered()
    {
        return User?.UserGlobalModel != null;
    }

    public UserGlobalModel GetUserGlobalModel()
    {
        return User?.UserGlobalModel ?? new UserGlobalModel();
    }

    /// <summary>
    ///     Returns the currently selected guild
    /// </summary>
    /// <returns></returns>
    public SocketGuild GetGuild()
    {
        return _discordSocketClient.GetGuild(GetUserGlobalModel().SelectedGuild);
    }

    /// <summary>
    ///     Returns a list of mutual guilds between the user and the bot
    /// </summary>
    /// <returns></returns>
    public List<SocketGuild> GetGuilds()
    {
        return _discordSocketClient.GetUser(GetUserGlobalModel().DiscordIdLong)?.MutualGuilds?.ToList() ??
               new List<SocketGuild>();
    }

    public async Task<string> GetTokenAsync()
    {
        return await _authenticator.GetTokenAsync();
    }

    public IEnumerable<Claim> Claims()
    {
        return User?.Claims ?? new List<Claim>();
    }
}