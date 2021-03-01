using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Dikubot.Database.Models.Session;
using Dikubot.DataLayer.Logic.WebDiscordBridge;
using Discord.WebSocket;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Dikubot.Webapp.Logic
{
    public class Authenticator : AuthenticationStateProvider
    {
        private ILocalStorageService _localStorageService;
        private SessionServices _sessionServices;
        private NavigationManager _navigationManager;
        private SocketGuild _guild;
        
        public Authenticator(ILocalStorageService localStorageService, NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            _guild = SubDomainConnector.GetGuildFromDomain(navigationManager.Uri);
            _sessionServices = new SessionServices(_guild);
            _localStorageService = localStorageService;
        }

        /// <summary>
        /// Get the guild connectected to this authentication instance
        /// </summary>
        /// <returns>A Discord guild</returns>
        public SocketGuild GetGuild()
        {
            return _guild;
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

            //We convert the sessionKey to a Guid, as that's our universal Id format - But we only do so if the sessionStringKey is a valid guid
            if (!Guid.TryParse(sessionStringKey, out Guid sessionKey))
            {
                //Returns an empty UserIdentity, meaning the user isn't authorized to do anything
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(_guild))));
            }

            if (!_sessionServices.Exists(sessionKey))
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(_guild))));
            }

            SessionModel sessionModel = _sessionServices.Get(sessionKey);
            if (sessionModel.IsExpired)
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(_guild))));
            }

            //Here we return a UserIdentity with our sessionModel. The system will then check the user connected to the session,
            //to see what authorisation step the user is at.
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(sessionModel, _guild))));
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
                Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(sessionModel, _guild)))));
        }
    }
}