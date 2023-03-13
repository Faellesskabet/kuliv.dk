using System;
using System.Collections.Generic;
using System.Security.Claims;
using Dikubot.DataLayer.Database.Global.User.DiscordUser;
using Dikubot.DataLayer.Static;
using Dikubot.Webapp.Authentication.Discord.OAuth2;

namespace Dikubot.Webapp.Authentication.Identities;

public sealed class DiscordIdentity : ClaimsIdentity
{
    /// <summary>
    ///     Empty UserIdentity
    /// </summary>
    public DiscordIdentity()
    {
    }

    /// <summary>
    ///     Creates a UserIdentity from a DiscordUserClaim
    /// </summary>
    /// <param name="discordUserClaim"></param>
    /// <param name="discordUserGlobalMongoService"></param>
    public DiscordIdentity(DiscordUserClaim discordUserClaim,
        DiscordUserGlobalMongoService discordUserGlobalMongoService)
    {
        DiscordUserClaim = discordUserClaim;
        DiscordUserGlobalModel = discordUserGlobalMongoService.Get(discordUserClaim.UserId);

        if (DiscordUserGlobalModel == null) return;
        try
        {
            List<Claim> roleClaims = new();
            if (DiscordUserGlobalModel.IsAdmin) roleClaims.Add(new Claim(ClaimTypes.Role, Permissions.GlobalAdmin));

            if (DiscordUserGlobalModel.IsAdmin ||
                Util.IsGuildAdmin(DiscordUserGlobalModel.DiscordIdLong, DiscordUserGlobalModel.SelectedGuild))
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
    public string AuthenticationType => "DiscordUser";

    /// <summary>
    ///     isAuthenticated gets the user specified in the session, if the user exists. A user is authenticated if the
    ///     following holds true:
    ///     The user must have a DiscordId
    ///     The user must be verified by email
    ///     The session may not be expired
    ///     The user may not be banned
    /// </summary>
    public override bool IsAuthenticated =>
        DiscordUserGlobalModel?.DiscordId != null && DiscordUserGlobalModel.Verified && DiscordUserGlobalModel.Name != null &&
        DiscordUserClaim != null && DiscordUserClaim.UserId != 0 && !DiscordUserGlobalModel.IsBanned;


    public string Name => DiscordUserGlobalModel == null ? "Intet navn" : DiscordUserGlobalModel.Name;

    /// <summary>
    ///     Get the UserModel
    /// </summary>
    public DiscordUserGlobalModel DiscordUserGlobalModel { get; set; }

    /// <summary>
    ///     Get the SessionModel
    /// </summary>
    public DiscordUserClaim DiscordUserClaim { get; }

    /// <summary>
    ///     Get all guid for roles the user have
    /// </summary>
    public string DiscordId => DiscordUserGlobalModel?.DiscordId;

    /// <summary>
    ///     Get all guid for roles the user have
    /// </summary>
    public ulong DiscordIdLong => DiscordUserGlobalModel.DiscordIdLong;
}