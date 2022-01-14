using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Blazorise.Extensions;
using Dikubot.DataLayer.Database.Global.Session;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Static;
using Dikubot.Discord;
using Discord.WebSocket;

namespace Dikubot.Webapp.Authentication
{
    public sealed class UserIdentity : ClaimsIdentity
    {
        private UserGlobalModel _userGlobalModel;
        private readonly SessionModel _sessionModel;

        /// <summary>
        /// Empty UserIdentity
        /// </summary>
        public UserIdentity()
        {
        }

        /// <Summary>Creates a UserIdentity based on a session.</Summary>
        /// <param name="sessionModel">A session consists of a key and a user.</param>
        public UserIdentity(SessionModel sessionModel)
        {
            _sessionModel = sessionModel;
            _userGlobalModel = sessionModel.GetUserModel();
            if (_userGlobalModel != null)
            {
                try
                {
                    List<Claim> roleClaims = new List<Claim>();
                    if (_userGlobalModel.IsAdmin)
                    {
                        roleClaims.Add(new Claim(ClaimTypes.Role, Permissions.GlobalAdmin));
                    }
                    
                    if (_userGlobalModel.IsAdmin || Util.IsGuildAdmin(_userGlobalModel.DiscordIdLong, _userGlobalModel.SelectedGuild))
                    {
                        roleClaims.Add(new Claim(ClaimTypes.Role, Permissions.GuildAdmin));
                    }
                    AddClaims(roleClaims);
                }
                catch (Exception e)
                {
                    Logger.Debug(e.Message);
                }
            }
        }
        
        /// <Summary>This is simply just a name and it has no purposes except for us to differentiate between AuthenticationTypes / reasons for authentication</Summary>
        /// <return>"User" as a string</return>
        public string AuthenticationType
        {
            get => "User";
        }

        /// <summary>
        /// isAuthenticated gets the user specified in the session, if the user exists. A user is authenticated if the following holds true:
        /// The user must have a DiscordId
        /// The user must be verified by email
        /// The session may not be expired
        /// The user may not be banned
        /// The user must have selected a guild
        /// </summary>
        public override bool IsAuthenticated
        {
            get => _userGlobalModel?.DiscordId != null && _userGlobalModel.Verified && _userGlobalModel.Name != null &&
                   !_sessionModel.IsExpired && !_userGlobalModel.IsBanned && _userGlobalModel.SelectedGuild != 0;
        }

        public string Name
        {
            get => _userGlobalModel == null ? "Intet navn" : _userGlobalModel.Name;
        }

        /// <summary>
        /// Get the UserModel
        /// </summary>
        public UserGlobalModel UserGlobalModel
        {
            get => _userGlobalModel;
            set => _userGlobalModel = value;
        }

        /// <summary>
        /// Get the SessionModel
        /// </summary>
        public SessionModel SessionModel
        {
            get => _sessionModel;
        }
    }
}