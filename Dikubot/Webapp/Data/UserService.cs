using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.Webapp.Authentication;
using Microsoft.AspNetCore.Components.Authorization;

namespace Dikubot.Webapp.Data
{
    public class UserService
    {
        private Authenticator _authenticator;
        private UserIdentity? _user;
        private UserGlobalMongoService _userGlobalMongoService;

        private UserService(AuthenticationStateProvider authenticationStateProvider, UserGlobalMongoService userGlobalMongoService)
        {
            _userGlobalMongoService = userGlobalMongoService;
            _authenticator = ((Authenticator) authenticationStateProvider);
            AuthenticationState authState = _authenticator.GetAuthenticationStateAsync().Result;
            _user = (UserIdentity) authState.User.Identity;
        }

        public bool IsRegistered()
        {
            return _user?.UserGlobalModel != null;
        }

        public UserGlobalModel GetUserGlobalModel()
        {
            return _user?.UserGlobalModel ?? new UserGlobalModel();
        }

        public async Task<string> GetTokenAsync()
        {
            return await _authenticator.GetTokenAsync();
        }
        
        
        
        
    }
}
