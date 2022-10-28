using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.Discord;
using Dikubot.Webapp.Authentication;
using Discord.WebSocket;
using Microsoft.AspNetCore.Components.Authorization;

namespace Data
{
    public class UserService
    {
        
        private Authenticator _authenticator;
        private UserIdentity? _user;
        private DiscordBot _discordBot;

        public UserService(AuthenticationStateProvider authenticationStateProvider, DiscordBot discordBot)
        {
            _authenticator = ((Authenticator) authenticationStateProvider);
            AuthenticationState authState = _authenticator.GetAuthenticationStateAsync().Result;
            _user = (UserIdentity) authState.User.Identity;
            _discordBot = discordBot;
        }

        public string id => _user?.DiscordId;
        
        public bool IsRegistered()
        {
            return _user?.UserGlobalModel != null;
        }

        public UserGlobalModel GetUserGlobalModel()
        {
            return _user?.UserGlobalModel ?? new UserGlobalModel();
        }

        public SocketGuild getGuild()
        {
            return _discordBot.Client.GetGuild(GetUserGlobalModel().SelectedGuild);
        }

        public async Task<string> GetTokenAsync()
        {
            return await _authenticator.GetTokenAsync();
        }

        public IEnumerable<Claim> Claims()
        {
            return _user?.Claims ?? new List<Claim>();
        }
 
        

    }
}
