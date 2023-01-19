using Dikubot.DataLayer.Database.Guild.Models.Role.SubModels.Color;
using Dikubot.DataLayer.Database.Guild.Models.Role.SubModels.GuildPermissions;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild.Models.Role;

/// <summary>
///     Class for for retrieving information from the User collection.
/// </summary>
public class RoleMongoService : GuildMongoService<RoleMainModel>
{
    public RoleMongoService(Database database, SocketGuild guild) : base(database, guild)
    {
    }

    public override string GetCollectionName()
    {
        return "Roles";
    }

    /// <Summary>
    ///     Inserts a Model in the collection. If a RoleModel with the same ID, Name or discordID already
    ///     exists, then we imply invoke Update() on the model instead.
    /// </Summary>
    /// <param name="mainModelIn">The Model one wishes to be inserted.</param>
    /// <return>A Model.</return>
    public new RoleMainModel Upsert(RoleMainModel mainModelIn)
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

    public bool Exists(string name)
    {
        return Exists(model => model.Name.ToLower() == name.ToLower());
    }

    /// <Summary>Checks if a RoleModel is already in the database.</Summary>
    /// <param name="mainModelIn">A boolean which tells if the models is in the database.</param>
    /// <return>A bool, true if the value already exist false if not.</return>
    public bool Exists(RoleMainModel mainModelIn)
    {
        bool idCollision = Exists(model => model.Id == mainModelIn.Id);
        bool discordIdCollision = Exists(model => model.DiscordId == mainModelIn.DiscordId);
        return idCollision || discordIdCollision;
    }

    /// <Summary>Removes a element from the collection by it's unique elements.</Summary>
    /// <param name="mainModelIn">The Model one wishes to remove.</param>
    /// <return>Void.</return>
    public new void Remove(RoleMainModel mainModelIn)
    {
        Remove(model => model.Id == mainModelIn.Id);
        Remove(model => model.DiscordId == mainModelIn.DiscordId);
    }

    /// <Summary>Gets a role by it's discord id.</Summary>
    /// <param name="discordId">The discord id.</param>
    /// <return>A Model.</return>
    public RoleMainModel Get(ulong discordId)
    {
        return Get(model => model.DiscordId == discordId.ToString());
    }

    /// <Summary>Converts a RoleSocket to a RoleModel.</Summary>
    /// <param name="role">The SocketRole model one wishes to be converted.</param>
    /// <return>A RoleModel.</return>
    public RoleMainModel SocketToModel(SocketRole role)
    {
        RoleMainModel roleMain = new RoleMainModel();
        roleMain.Permissions = new GuildPermissionsModel(role.Permissions);
        roleMain.Color = new ColorModel(role.Color);
        roleMain.Name = role.Name;
        roleMain.Position = role.Position;
        roleMain.CreatedAt = role.CreatedAt.DateTime;
        roleMain.IsEveryone = role.IsEveryone;
        roleMain.IsHoisted = role.IsHoisted;
        roleMain.IsManaged = role.IsManaged;
        roleMain.IsMentionable = role.IsMentionable;
        roleMain.DiscordId = role.Id.ToString();
        if (Exists(model => model.DiscordId == roleMain.DiscordId))
            roleMain.Id = Get(rm => role.Id.ToString().Equals(rm.DiscordId)).Id;
        return roleMain;
    }

    /// <Summary>Converts a RoleSocket to a RoleModel.</Summary>
    /// <param name="role">The SocketRole model one wishes to be converted.</param>
    /// <return>A RoleModel.</return>
    public RoleMainModel RestToModel(RestRole role)
    {
        RoleMainModel roleMain = new RoleMainModel();
        roleMain.Permissions = new GuildPermissionsModel(role.Permissions);
        roleMain.Color = new ColorModel(role.Color);
        roleMain.Name = role.Name;
        roleMain.Position = role.Position;
        roleMain.CreatedAt = role.CreatedAt.DateTime;
        roleMain.IsEveryone = role.IsEveryone;
        roleMain.IsHoisted = role.IsHoisted;
        roleMain.IsManaged = role.IsManaged;
        roleMain.IsMentionable = role.IsMentionable;
        roleMain.DiscordId = role.Id.ToString();

        return roleMain;
    }

    /// <Summary>
    ///     Converts a RoleModel to a RoleProperties class which can be used to modify a role or create a role.
    /// </Summary>
    /// <param name="roleMainModel">The RoleModel which will be used to create a RoleProperties.</param>
    /// <return>A RoleProperties.</return>
    public RoleProperties ModelToRoleProperties(RoleMainModel roleMainModel)
    {
        RoleProperties properties = new RoleProperties();
        properties.Name = roleMainModel.Name;
        properties.Hoist = roleMainModel.IsHoisted;
        properties.Mentionable = roleMainModel.IsMentionable;
        properties.Position = roleMainModel.Position;
        properties.Permissions = roleMainModel.Permissions.ToGuildPermission();
        properties.Color = roleMainModel.Color.ToColor();
        return properties;
    }
}