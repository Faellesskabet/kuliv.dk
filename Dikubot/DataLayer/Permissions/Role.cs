using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Dikubot.DataLayer.Database.Guild.Models.User.SubModels;
using Dikubot.DataLayer.Database.Interfaces;
using Dikubot.DataLayer.Static;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions;

public partial class PermissionsService
{
    /// <Summary>Will sync all the roles on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void SetDatabaseRoles()
    {
        List<RoleMainModel> roleModels = _roleMongoService.Get();
        List<SocketRole> socketRoles = guild.Roles.ToList();
        HashSet<ulong> discordRoleIds = new(socketRoles.Select(role => role.Id));
        List<RoleMainModel> toBeRemoved =
            roleModels.Where(model => !discordRoleIds.Contains(ulong.Parse(model.DiscordId))).ToList();

        Func<RoleMainModel, SocketRole, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;

        // Remove all the roles from the database if they are not on the discord server.
        roleModels.RemoveAll(m => socketRoles.Exists(n => inDB(m, n)));

        // Remove the roles from the database that is not on the discord server.
        toBeRemoved.ForEach(m => _roleMongoService.Remove(m));

        // Makes an upsert of the roles on the server so they match the ones in the database.
        socketRoles.ForEach(model => _roleMongoService.Upsert(_roleMongoService.SocketToModel(model)));
    }

    /// <Summary>Will sync all the roles on the database to the discord server.</Summary>
    /// <return>void.</return>
    public async void SetDiscordRoles()
    {
        List<RoleMainModel> roleModels = _roleMongoService.Get();
        List<SocketRole> socketRoles = guild.Roles.ToList();

        List<SocketRole> toBeRemoved = new(socketRoles);

        Func<RoleMainModel, SocketRole, bool> inDB = (m0, m1) => Convert.ToUInt64(m0.DiscordId) == m1.Id;


        // Remove all the roles from the discord server if they are not in the database.
        toBeRemoved.RemoveAll(m => roleModels.Exists(n => inDB(n, m)));

        foreach (SocketRole socketRole in toBeRemoved)
            await socketRole.DeleteAsync();

        // Roles which will not be be changed.
        List<string> unchangeableRoles = new() { "Admin" };

        foreach (RoleMainModel roleModel in roleModels)
        {
            // Skip this role if is in unchangeableRoles.
            if (unchangeableRoles.Contains(roleModel.Name))
                continue;

            // Finds the role discord server role that match the role in the database.
            SocketRole socketRole = socketRoles.Find(socket => Convert.ToUInt64(roleModel.DiscordId) == socket.Id);

            if (socketRole == null)
            {
                // If the role could not be found create it.
                RoleProperties properties = _roleMongoService.ModelToRoleProperties(roleModel);
                await guild.CreateRoleAsync(properties.Name.Value,
                    properties.Permissions.Value,
                    properties.Color.Value,
                    properties.Hoist.Value,
                    properties.Mentionable.Value,
                    RequestOptions.Default);
            }
            else
            {
                // If the role could be found modify it so it matches the database.
                RoleProperties _properties = _roleMongoService.ModelToRoleProperties(roleModel);
                await socketRole.ModifyAsync(properties =>
                {
                    properties.Color = _properties.Color;
                    properties.Hoist = _properties.Hoist;
                    properties.Mentionable = _properties.Mentionable;
                    properties.Name = _properties.Name;
                    properties.Permissions = _properties.Permissions;
                    // Can not get this to work it will not change the role position.
                    // properties.Position = _properties.Position;
                });
            }
        }
    }

    public void UpdateVerifyRole(UserGuildModel userGuildModel, SocketRole role)
    {
        // If the role is not set or does not exist, we return
        if (role == null) return;

        if (!_userGlobalMongoService.Get(userGuildModel.DiscordId).Verified)
        {
            if (userGuildModel.HasRole(role.Id, _roleMongoService))
                userGuildModel.RemoveRole(
                    new UserRoleModel(_roleMongoService.Get(model => model.DiscordId == role.Id.ToString())));
            return;
        }

        if (!userGuildModel.HasRole(role.Id, _roleMongoService))
            userGuildModel.AddRole(
                new UserRoleModel(_roleMongoService.Get(model => model.DiscordId == role.Id.ToString())));
    }

    public Task SetDiscordUserRoles(UserGuildModel userMainModel)
    {
        SetDiscordUserRoles(userMainModel, _guildSettingsMongoService.Get(guild));
        return Task.CompletedTask;
    }

    /// <summary>
    ///     This functions downloads a user's roles from the Database and adds them to their Discord profile. The user's roles
    ///     will match exactly what is in the database,
    ///     which means roles not specified in the database will be removed from the user. Expired roles will also be removed
    ///     from the user
    /// </summary>
    /// <param name="userMainModel"></param>
    public async void SetDiscordUserRoles(UserGuildModel userMainModel, GuildSettingsModel guildSettingsModel)
    {
        SocketUser discordUser = userMainModel.DiscordUser;
        if (discordUser == null) return;

        //We get the user in the context of the current guilds
        SocketGuildUser guildUser = guild.GetUser(discordUser.Id);
        if (guildUser == null) return;

        //Give the user the verified role if they're verified
        UpdateVerifyRole(userMainModel, guild.GetRole(guildSettingsModel.VerifiedRole));

        //We get all the user's roles in the database
        HashSet<UserRoleModel> userRoleModels = new(userMainModel.Roles);

        // We get the user's roles and remove all the roles not in the database.
        // We also remove the role if it has expired

        IReadOnlyCollection<SocketRole> discordRoles = guildUser.Roles;


        // Removes roles if they aren't active
        IEnumerable<IRole> removeRoles = discordRoles.Where(role =>
            userRoleModels
                .FirstOrDefault(usr =>
                    new UserRoleModel(_roleMongoService.SocketToModel(role)).RoleId.Equals(usr.RoleId)) ==
            null //Kigger på om brugeren har rollen med samme RoleID.
            || !userMainModel.IsRoleActive(_roleMongoService.SocketToModel(role)));

        foreach (IRole role in removeRoles)
            try
            {
                Logger.Debug($"Removing {role.Name} from {guildUser.Username}");
                await guildUser.RemoveRoleAsync(role);
            }
            catch (Exception)
            {
                Logger.Debug($"Could not remove {role.Name} from {guildUser.Username}");
            }

        //We add the roles in the database to the user, but only if the role is currently active and the user don't have the role.
        IEnumerable<IRole> addRoles =
            userRoleModels.Where(model => ((IActiveTimeFrame)model).IsActive() &&
                                          discordRoles.FirstOrDefault(role =>
                                              new UserRoleModel(_roleMongoService.SocketToModel(role)).RoleId.Equals(
                                                  model.RoleId)) == null) //Se om brugeren ikke har rollen.
                .Select(model => guild.GetRole(Convert.ToUInt64(_roleMongoService.Get(model.RoleId)?.DiscordId)))
                .Where(role => role != null);

        foreach (IRole role in addRoles)
            try
            {
                Logger.Debug($"Adding {role.Name} to {guildUser.Username}");
                await guildUser.AddRoleAsync(role, new RequestOptions { RetryMode = RetryMode.RetryRatelimit });
            }
            catch (Exception)
            {
                Logger.Debug($"Could not add {role.Name} to {guildUser.Username}");
            }
    }

    public void SetDiscordUserRoles(SocketUser user, GuildSettingsModel guildSettingsModel)
    {
        SetDiscordUserRoles(_userMongoService.Get(user), guildSettingsModel);
    }

    public void SetDiscordUserRoles(SocketUser user)
    {
        SetDiscordUserRoles(_userMongoService.Get(user));
    }

    /// <Summary>Add a role on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void AddOrUpdateDatabaseRole(SocketRole role)
    {
        _roleMongoService.Upsert(_roleMongoService.SocketToModel(role));
    }

    /// <Summary>Add a role on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void AddOrUpdateDatabaseRole(RestRole role)
    {
        _roleMongoService.Upsert(_roleMongoService.RestToModel(role));
    }

    /// <Summary>Add a role on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void AddOrUpdateDatabaseRole(RoleMainModel roleMain)
    {
        _roleMongoService.Upsert(roleMain);
    }

    /// <Summary>Removes a role on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void RemoveDatabaseRole(SocketRole role)
    {
        _roleMongoService.Remove(_roleMongoService.SocketToModel(role));
    }

    /// <Summary>Removes a role on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void RemoveDatabaseRole(RestRole role)
    {
        _roleMongoService.Remove(_roleMongoService.RestToModel(role));
    }

    /// <Summary>Removes a role on the discord server to the database.</Summary>
    /// <return>void.</return>
    public void RemoveDatabaseRole(RoleMainModel roleMain)
    {
        _roleMongoService.Remove(roleMain);
    }
}