using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Dikubot.Database.Models.Session;
using Microsoft.AspNetCore.Components.Authorization;

namespace Dikubot.Webapp.Logic
{
    public class Authenticator : AuthenticationStateProvider
    {
        private ILocalStorageService _localStorageService;
        private SessionServices _sessionServices = new SessionServices();
        public Authenticator(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string sessionKey = await _localStorageService.GetItemAsStringAsync("session");
            SessionServices sessionServices = new SessionServices();
            if (!sessionServices.Exists(sessionKey))
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity())));
            }

            SessionModel sessionModel = sessionServices.Get(sessionKey);
            if (sessionModel.IsExpired)
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity())));
            }
            
            return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(sessionModel))));
        }
        
        public async Task UpdateSession(SessionModel sessionModel)
        {
            _sessionServices.Upsert(sessionModel);
            await _localStorageService.SetItemAsync("session", sessionModel.Id);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new UserIdentity(sessionModel)))));
        }
    }
}