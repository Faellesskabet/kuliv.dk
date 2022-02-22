using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Global.Session;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Logic.WebDiscordBridge;
using Dikubot.Discord;
using Discord.WebSocket;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Dikubot.Webapp.Authentication
{
    public class Authenticator : AuthenticationStateProvider
    {
        private ILocalStorageService _localStorageService;
        private SessionServices _sessionServices;
        private SocketGuild _guild;
        
        public Authenticator(ILocalStorageService localStorageService)
        {
            _sessionServices = new SessionServices();
            _localStorageService = localStorageService;
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

        /// <summary>
        /// GetAuthenticationStateAsync is used to set the AuthenticationState based on a UserIdentity.
        /// It will use the session key in localstorage, to determine whether a user is authorized or not
        /// </summary>
        /// <returns>Task</returns>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //Get session key
            string sessionStringKey = await _localStorageService.GetItemAsStringAsync("session");
            
            // It includes the quotation marks from the cookie when loading, so we replace them.
            if (!string.IsNullOrWhiteSpace(sessionStringKey))
            {
                sessionStringKey = sessionStringKey.Replace("\"", string.Empty);   
            }

            //We convert the sessionKey to a Guid, as that's our universal Id format - But we only do so if the sessionStringKey is a valid guid
            if (!Guid.TryParse(sessionStringKey, out Guid sessionKey))
            {
                //Returns an empty UserIdentity, meaning the user isn't authorized to do anything
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity())));
            }

            if (!_sessionServices.Exists(sessionKey))
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity())));
            }

            SessionModel sessionModel = _sessionServices.Get(sessionKey);
            if (sessionModel.IsExpired)
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity())));
            }

            //Here we return a UserIdentity with our sessionModel. The system will then check the user connected to the session,
            //to see what authorisation step the user is at.
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(sessionModel))));
        }

        /// <summary>
        /// Updates the session in localStorage as well as tells the AuthenticationState that the session has been updated
        /// </summary>
        /// <param name="sessionModel">The current session will be set to this model</param>
        /// <returns>Task</returns>
        public async Task UpdateSession(SessionModel sessionModel)
        {
            _sessionServices.Upsert(sessionModel);
            await _localStorageService.SetItemAsync("session", sessionModel.Id.ToString());
            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(sessionModel)))));
        }
    }
}