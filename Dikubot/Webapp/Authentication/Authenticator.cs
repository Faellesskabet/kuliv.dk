using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Data;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.Session;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Logic.WebDiscordBridge;
using Dikubot.Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;


namespace Dikubot.Webapp.Authentication
{
    public class Authenticator : AuthenticationStateProvider
    {
        private IHttpContextAccessor _httpContextAccssor;
        private UserService _userService;
        private SocketGuild _guild;

        public Authenticator(IHttpContextAccessor httpContextAccssor)
        {
            _httpContextAccssor = httpContextAccssor;
            _userService = new UserService();
        }
        
        /// <summary>
        /// Get the current UserGlobalModel
        /// </summary>
        /// <returns>Returns a nullable UserGlobalModel based on the current session</returns>
        public async Task<UserGlobalModel> GetUserGlobal()
        {
            
            var authState = await GetAuthenticationStateAsync();
            UserIdentity user = (UserIdentity) authState.User.Identity!;
            return user?.UserGlobalModel;
        }
        
        /// <summary>
        /// Get SocketGuild of the current session
        /// </summary>
        /// <returns></returns>
        public async Task<SocketGuild> GetSocketGuild()
        {
            return await GetSocketGuild(await GetUserGlobal());
        }
        
        private async Task<SocketGuild> GetSocketGuild(UserGlobalModel user)
        {
            return user == null ? null : DiscordBot.Client.GetGuild(user.SelectedGuild);
        }

        /// <summary>
        /// Get the GuildSettingsModel of the current session
        /// </summary>
        /// <returns></returns>
        public async Task<GuildSettingsModel> GetGuildSettings()
        {
            return await GetGuildSettings(await GetSocketGuild());
        }

        private async Task<GuildSettingsModel> GetGuildSettings(SocketGuild guild)
        {
            return new GuildSettingsService().Get(model => model.GuildId == guild.Id) ?? new GuildSettingsModel(guild);
        }

        private UserService.DiscordUserClaim GetDiscordUserClaim()
        {
            if (_httpContextAccssor.HttpContext == null)
            {
                return null;
            }
            return _userService.GetInfo(_httpContextAccssor.HttpContext);
        }

        /// <summary>
        /// GetAuthenticationStateAsync is used to set the AuthenticationState based on a UserIdentity.
        /// </summary>
        /// <returns>Task</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            
            UserService.DiscordUserClaim discordUserClaim = GetDiscordUserClaim();
            if (discordUserClaim == null)
            {
                //Returns an empty UserIdentity, meaning the user isn't authorized to do anything
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity())));
            }
            
            //Here we return a UserIdentity with our sessionModel. The system will then check the user connected to the session,
            //to see what authorisation step the user is at.
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(discordUserClaim))));
        }

        /// <summary>
        /// Updates the session in localStorage as well as tells the AuthenticationState that the session has been updated
        /// </summary>
        /// <param name="sessionModel">The current session will be set to this model</param>
        /// <returns>Task</returns>
        public async Task UpdateSession()
        {
            UserService.DiscordUserClaim discordUserClaim = GetDiscordUserClaim();
            UserIdentity userIdentity =
                discordUserClaim == null ? new UserIdentity() : new UserIdentity(discordUserClaim);
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(new ClaimsPrincipal(userIdentity))));
        }
    }
}