using System;
using System.Collections.Generic;
using System.Security.Claims;
using Dikubot.DataLayer.Database.Global.Session;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Static;
using Dikubot.Webapp.Authentication.Discord.OAuth2;
using Dikubot.Webapp.Data;

namespace Dikubot.Webapp.Authentication
{
    public sealed class UserIdentity : ClaimsIdentity
    {
        private readonly DiscordUserClaim _discordUserClaim;

        /// <summary>
        /// Empty UserIdentity
        /// </summary>
        public UserIdentity()
        {
            
        }

        /// <summary>
        /// Creates a UserIdentity from a DiscordUserClaim
        /// </summary>
        /// <param name="discordUserClaim"></param>
        public UserIdentity(DiscordUserClaim discordUserClaim)
        {
            _discordUserClaim = discordUserClaim;
            UserGlobalModel = new UserGlobalMongoService().Get(discordUserClaim.UserId);
            if (UserGlobalModel == null)
            {
                return;
            }
            try
            {
                List<Claim> roleClaims = new List<Claim>();
                if (UserGlobalModel.IsAdmin)
                {
                    roleClaims.Add(new Claim(ClaimTypes.Role, Permissions.GlobalAdmin));
                }
                    
                if (UserGlobalModel.IsAdmin || Util.IsGuildAdmin(UserGlobalModel.DiscordIdLong, UserGlobalModel.SelectedGuild))
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

        /// <Summary>This is simply just a name and it has no purposes except for us to differentiate between AuthenticationTypes / reasons for authentication</Summary>
        /// <return>"User" as a string</return>
        public string AuthenticationType => "User";

        /// <summary>
        /// isAuthenticated gets the user specified in the session, if the user exists. A user is authenticated if the following holds true:
        /// The user must have a DiscordId
        /// The user must be verified by email
        /// The session may not be expired
        /// The user may not be banned
        /// The user must have selected a guild
        /// </summary>
        public override bool IsAuthenticated =>
            UserGlobalModel?.DiscordId != null && UserGlobalModel.Verified && UserGlobalModel.Name != null &&
            _discordUserClaim != null && _discordUserClaim.UserId != 0 && !UserGlobalModel.IsBanned && UserGlobalModel.SelectedGuild != 0;

        public string Name => UserGlobalModel == null ? "Intet navn" : UserGlobalModel.Name;

        /// <summary>
        /// Get the UserModel
        /// </summary>
        public UserGlobalModel UserGlobalModel { get; set; }

        /// <summary>
        /// Get the SessionModel
        /// </summary>
        public DiscordUserClaim DiscordUserClaim => _discordUserClaim;
    }
}