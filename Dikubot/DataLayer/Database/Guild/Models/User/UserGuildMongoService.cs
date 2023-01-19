using System;
using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.DataLayer.Database.Guild.Models.User.SubModels;
using Dikubot.DataLayer.Database.Interfaces;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.User;

/// <summary>
///     Class for for retrieving information from the User collection.
/// </summary>
public class UserGuildMongoService : GuildMongoService<UserGuildModel>, IIndexed<UserGuildModel>
{
    private readonly IGuildMongoFactory _guildMongoFactory;

    public UserGuildMongoService(IGuildMongoFactory guildMongoFactory, Database database, SocketGuild guild) : base(
        database, guild)
    {
        _guildMongoFactory = guildMongoFactory;
    }

    public IEnumerable<CreateIndexModel<UserGuildModel>> GetIndexes()
    {
        CreateIndexOptions options = new CreateIndexOptions { Unique = true };
        return new List<CreateIndexModel<UserGuildModel>>
        {
            new(Builders<UserGuildModel>.IndexKeys.Ascending(model => model.DiscordId), options)
        };
    }

    public UserGuildModel Get(SocketUser user)
    {
        return Get(user.Id.ToString());
    }

    public UserGuildModel Get(string discordId)
    {
        UserGuildModel mainModel = Get(inmodel => inmodel.DiscordId == discordId);
        if (mainModel == null)
            mainModel = new UserGuildModel
            {
                DiscordId = discordId
            };

        return mainModel;
    }

    public bool Exists(SocketUser user)
    {
        return Exists(model => model.DiscordId == user.Id.ToString());
    }

    public override string GetCollectionName()
    {
        return "Users";
    }

    /// <Summary>
    ///     Inserts a UserModel in the collection. If a UserModel with the same ID or discordID
    ///     already exists, then we imply invoke Update() on the model instead.
    /// </Summary>
    /// <param name="mainModelIn">The Model one wishes to be inserted.</param>
    /// <return>A Model.</return>
    public new UserGuildModel Upsert(UserGuildModel mainModelIn)
    {
        bool idCollision = Exists(model => model.Id == mainModelIn.Id);
        bool discordIdCollision = Exists(model => model.DiscordId == mainModelIn.DiscordId);

        if (idCollision)
        {
            Update(mainModelIn, new ReplaceOptions { IsUpsert = true });
            return mainModelIn;
        }

        if (discordIdCollision)
        {
            Update(m => m.DiscordId == mainModelIn.DiscordId, mainModelIn, new ReplaceOptions { IsUpsert = true });
            return mainModelIn;
        }

        Insert(mainModelIn);
        return mainModelIn;
    }

    /// <Summary>Removes a UserModel from the collection by it's unique elements.</Summary>
    /// <param name="mainModelIn">The Model one wishes to remove.</param>
    /// <return>Void.</return>
    public new void Remove(UserGuildModel mainModelIn)
    {
        Remove(model => model.Id == mainModelIn.Id);
        Remove(model => model.DiscordId == mainModelIn.DiscordId);
    }

    /// <Summary>Converts a SocketGuildUser to a UserModel.</Summary>
    /// <param name="role">The SocketGuildUser model one wishes to be converted.</param>
    /// <return>A UserModel.</return>
    public UserGuildModel SocketToModel(SocketGuildUser user)
    {
        RoleMongoService roleServices = _guildMongoFactory.Get<RoleMongoService>(Guild);
        UserGuildModel _user = new UserGuildModel();
        _user.DiscordId = user.Id.ToString();
        _user.IsBot = user.IsBot;
        _user.Username = user.Username;
        _user.JoinedAt = user.JoinedAt?.DateTime ?? DateTime.Now;
        _user.Name = user.Nickname;
        List<UserRoleModel> roles = new();
        foreach (SocketRole role in user.Roles)
            roles.Add(new UserRoleModel(roleServices.Get(role.Id)));
        _user.Roles = roles.ToArray();
        return _user;
    }

    /// <Summary>Converts a RestGuildUser to a UserModel.</Summary>
    /// <param name="role">The RestGuildUser model one wishes to be converted.</param>
    /// <return>A UserModel.</return>
    public UserGuildModel RestToModel(RestGuildUser user)
    {
        RoleMongoService roleServices = _guildMongoFactory.Get<RoleMongoService>(Guild);
        UserGuildModel _user = new UserGuildModel();
        _user.DiscordId = user.Id.ToString();
        _user.IsBot = user.IsBot;
        _user.Username = user.Username;
        _user.JoinedAt = user.JoinedAt?.DateTime ?? DateTime.Now;
        _user.Name = user.Nickname;
        List<UserRoleModel> roles = new();
        foreach (ulong roleId in user.RoleIds)
            roles.Add(new UserRoleModel(roleServices.Get(roleId)));
        _user.Roles = roles.ToArray();
        return _user;
    }

    /// <Summary>Converts a UserModel to a GuildUserProperties.</Summary>
    /// <param name="role">The UserModel model one wishes to be converted.</param>
    /// <return>A GuildUserProperties.</return>
    public GuildUserProperties ModelToProperties(SocketGuild guild, UserGuildModel userMain)
    {
        GuildUserProperties properties = new GuildUserProperties();
        properties.Nickname = userMain.Name;
        List<IRole> roles = new();
        List<ulong> roleIds = new();
        foreach (UserRoleModel userRole in userMain.Roles)
        {
            RoleMainModel roleMainModel = _guildMongoFactory.Get<RoleMongoService>(guild).Get(userRole.RoleId);
            if (roleMainModel == null) continue;
            ulong id = Convert.ToUInt64(roleMainModel.DiscordId);
            roleIds.Add(id);
            roles.Add(guild.GetRole(id));
        }

        properties.Roles = roles;
        properties.RoleIds = roleIds;
        return properties;
    }
}