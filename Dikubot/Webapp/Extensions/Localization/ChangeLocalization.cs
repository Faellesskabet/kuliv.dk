

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
            UserGlobalModel userGlobalModel = _user.GetUserGlobalModel();
            if (language == Language.Danish)
            {
                userGlobalModel.CultureInfo = "da-DK";
            }
            else
            {
                userGlobalModel.CultureInfo = "en-US";
              
            }
            
            
            if (_user?.GetUserGlobalModel() is not null)
            {
                new UserGlobalServices().Upsert(userGlobalModel);
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