using System.Security.Claims;
using System.Security.Principal;
using Dikubot.Database.Models;
using Dikubot.Database.Models.Session;

namespace Dikubot.Webapp.Logic
{
    public class UserIdentity : ClaimsIdentity
    {
        private UserModel _userModel;
        private SessionModel _sessionModel;
        public UserIdentity() { }

        public UserIdentity(SessionModel sessionModel)
        {
            _sessionModel = sessionModel;
            _userModel = new UserServices().Get(sessionModel.UserId);
        }
        
        public string AuthenticationType
        {
            get => "User";
        }

        public bool IsAuthenticated
        {
            get => _userModel?.DiscordId != null && _userModel.Email != null && !_sessionModel.IsExpired;
        }
        public string Name { get => _userModel == null? "Intet navn" : _userModel.Name; }

        public UserModel UserModel
        {
            get => _userModel;
        }

        public SessionModel SessionModel
        {
            get => _sessionModel;
        }
    }
}