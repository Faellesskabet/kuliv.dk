using System;
using System.Collections.Generic;
using System.Security.Claims;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Static;
using Dikubot.Webapp.Authentication.Discord.OAuth2;

namespace Dikubot.Webapp.Authentication;

public sealed class UserIdentity : ClaimsIdentity
{
    /// <summary>
    ///     Empty UserIdentity
    /// </summary>
    public UserIdentity()
    {
    }

    /// <summary>
    ///     Creates a UserIdentity from a DiscordUserClaim
    /// </summary>
    /// <param name="discordUserClaim"></param>
    /// <param name="userGlobalMongoService"></param>
    public UserIdentity(DiscordUserClaim discordUserClaim,
        UserGlobalMongoService userGlobalMongoService)
    {
        DiscordUserClaim = discordUserClaim;
        UserGlobalModel = userGlobalMongoService.Get(discordUserClaim.UserId);

        if (UserGlobalModel == null) return;
        try
        {
            List<Claim> roleClaims = new();
            if (UserGlobalModel.IsAdmin) roleClaims.Add(new Claim(ClaimTypes.Role, Permissions.GlobalAdmin));

            if (UserGlobalModel.IsAdmin ||
                Util.IsGuildAdmin(UserGlobalModel.DiscordIdLong, UserGlobalModel.SelectedGuild))
                roleClaims.Add(new Claim(ClaimTypes.Role, Permissions.GuildAdmin));
            AddClaims(roleClaims);
        }
        catch (Exception e)
        {
            Logger.Debug(e.Message);
        }
    }


    /// <Summary>
    ///     This is simply just a name and it has no purposes except for us to differentiate between AuthenticationTypes /
    ///     reasons for authentication
    /// </Summary>
    /// <return>"User" as a string</return>
    public string AuthenticationType => "User";

    /// <summary>
    ///     isAuthenticated gets the user specified in the session, if the user exists. A user is authenticated if the
    ///     following holds true:
    ///     The user must have a DiscordId
    ///     The user must be verified by email
    ///     The session may not be expired
    ///     The user may not be banned
    /// </summary>
    public override bool IsAuthenticated =>
        UserGlobalModel?.DiscordId != null && UserGlobalModel.Verified && UserGlobalModel.Name != null &&
        DiscordUserClaim != null && DiscordUserClaim.UserId != 0 && !UserGlobalModel.IsBanned;


    public string Name => UserGlobalModel == null ? "Intet navn" : UserGlobalModel.Name;

    /// <summary>
    ///     Get the UserModel
    /// </summary>
    public UserGlobalModel UserGlobalModel { get; set; }

    /// <summary>
    ///     Get the SessionModel
    /// </summary>
    public DiscordUserClaim DiscordUserClaim { get; }

    /// <summary>
    ///     Get all guid for roles the user have
    /// </summary>
    public string DiscordId => UserGlobalModel?.DiscordId;

    /// <summary>
    ///     Get all guid for roles the user have
    /// </summary>
    public ulong DiscordIdLong => UserGlobalModel.DiscordIdLong;
}