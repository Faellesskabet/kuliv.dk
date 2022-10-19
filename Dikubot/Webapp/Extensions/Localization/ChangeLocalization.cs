

using Data;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.Webapp.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph;


namespace Dikubot.Extensions.Localization
{
    public class ChangeLocalization
    {
        private UserService _user;
        private NotifyStateService _notifyStateService;
        private UserGlobalServices _services;
        
        public ChangeLocalization(UserService user, NotifyStateService notifyStateService)
        {
            this._user = user;
            _notifyStateService = notifyStateService;
        }
        
        public void Change(Language language)
        {
            if (language == Language.Danish)
            {
                _user.GetUserGlobalModel().CultureInfo = "da-DK";
            }
            else
            {
                _user.GetUserGlobalModel().CultureInfo = "en-US";
              
            }

            var test = _user.Claims();
            
            if (_user.IsRegistered())
            {
                new UserGlobalServices().Upsert(_user.GetUserGlobalModel());
            }
            
            _notifyStateService.NotifyUserChange(this);
            
        }

        public enum Language
        {
            English,
            Danish,
            
        }

    }
}