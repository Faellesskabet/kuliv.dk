using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Dikubot.Database.Models;
using Dikubot.Database.Models.Interfaces;
using Dikubot.Database.Models.Role;
using Dikubot.Database.Models.Session;
using Dikubot.DataLayer.Static;
using Discord.WebSocket;

namespace Dikubot.Webapp.Logic
{
    public sealed class UserIdentity : ClaimsIdentity
    {
        private UserModel _userModel;
        private SessionModel _sessionModel;
        private SocketGuild _guild;

        public UserIdentity(SocketGuild guild)
        {
            _guild = guild;
        }

        /// <Summary>Creates a UserIdentity based on a session.</Summary>
        /// <param name="sessionModel">A session consists of a key and a user.</param>
        public UserIdentity(SessionModel sessionModel, SocketGuild guild)
        {
            _sessionModel = sessionModel;
            _userModel = sessionModel.GetUserModel(guild);
            if (_userModel != null)
            {
                try
                {
                    IEnumerable<Claim> roleClaims =
                        _userModel.Roles.Where(model => ((IActiveTimeFrame)model).IsActive()).Select(model =>
                            new Claim(ClaimTypes.Role, model.GetRoleModel(guild)?.Name.ToUpper() ?? "DELETED_ROLE"));
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
        /// </summary>
        public override bool IsAuthenticated
        {
            get => _userModel?.DiscordId != null && _userModel.Verified && _userModel.Name != null &&
                   !_sessionModel.IsExpired;
        }

        public string Name
        {
            get => _userModel == null ? "Intet navn" : _userModel.Name;
        }

        /// <summary>
        /// Get the UserModel
        /// </summary>
        public UserModel UserModel
        {
            get => _userModel;
            set => _userModel = value;
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