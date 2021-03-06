using System;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Sockets;
using Dikubot.Database.Models.Role.SubModels;
using Dikubot.Discord;
using Discord;
using Discord.Rest;
using MongoDB.Driver;

namespace Dikubot.Database.Models.Role
{
    /// <summary>
    /// Class for for retrieving information from the User collection.
    /// </summary>
    public class RoleServices : Services<RoleModel>
    {
        public RoleServices(SocketGuild guild) : base("Main", "Roles", guild)
        {
        }

        /// <Summary>Inserts a Model in the collection. If a RoleModel with the same ID, Name or discordID already
        /// exists, then we imply invoke Update() on the model instead.</Summary>
        /// <param name="modelIn">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public new RoleModel Upsert(RoleModel modelIn)
        {
            bool idCollision = Exists(model => model.Id == modelIn.Id);
            bool discordIdCollision = Exists(model => model.DiscordId == modelIn.DiscordId);

            if (idCollision)
            {
                Update(modelIn, new ReplaceOptions() {IsUpsert = true});
                return modelIn;
            }

            if (discordIdCollision)
            {
                Update(m => m.DiscordId == modelIn.DiscordId, modelIn, new ReplaceOptions()
                    {IsUpsert = true});
                return modelIn;
            }

            Insert(modelIn);
            return modelIn;
        }

        public bool Exists(string name)
        {
            return Exists(model => model.Name.ToLower() == name.ToLower());
        }

        /// <Summary>Checks if a RoleModel is already in the database.</Summary>
        /// <param name="modelIn">A boolean which tells if the models is in the database.</param>
        /// <return>A bool, true if the value already exist false if not.</return>
        public new bool Exists(RoleModel modelIn)
        {
            bool idCollision = Exists(model => model.Id == modelIn.Id);
            bool discordIdCollision = Exists(model => model.DiscordId == modelIn.DiscordId);
            return idCollision || discordIdCollision;
        }

        /// <Summary>Removes a element from the collection by it's unique elements.</Summary>
        /// <param name="modelIn">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public new void Remove(RoleModel modelIn)
        {
            Remove(model => model.Id == modelIn.Id);
            Remove(model => model.DiscordId == modelIn.DiscordId);
        }

        /// <Summary>Gets a role by it's discord id.</Summary>
        /// <param name="discordId">The discord id.</param>
        /// <return>A Model.</return>
        public new RoleModel Get(ulong discordId) =>
            Get(model => model.DiscordId == discordId.ToString());

        /// <Summary>Converts a RoleSocket to a RoleModel.</Summary>
        /// <param name="role">The SocketRole model one wishes to be converted.</param>
        /// <return>A RoleModel.</return>
        public RoleModel SocketToModel(SocketRole role)
        {
            RoleModel _role = new RoleModel();
            _role.Permissions = new GuildPermissionsModel(role.Permissions);
            _role.Color = new ColorModel(role.Color);
            _role.Name = role.Name;
            _role.Position = role.Position;
            _role.CreatedAt = role.CreatedAt.DateTime;
            _role.IsEveryone = role.IsEveryone;
            _role.IsHoisted = role.IsHoisted;
            _role.IsManaged = role.IsManaged;
            _role.IsMentionable = role.IsMentionable;
            _role.DiscordId = role.Id.ToString();
            return _role;
        }

        /// <Summary>Converts a RoleSocket to a RoleModel.</Summary>
        /// <param name="role">The SocketRole model one wishes to be converted.</param>
        /// <return>A RoleModel.</return>
        public RoleModel RestToModel(RestRole role)
        {
            RoleModel _role = new RoleModel();
            _role.Permissions = new GuildPermissionsModel(role.Permissions);
            _role.Color = new ColorModel(role.Color);
            _role.Name = role.Name;
            _role.Position = role.Position;
            _role.CreatedAt = role.CreatedAt.DateTime;
            _role.IsEveryone = role.IsEveryone;
            _role.IsHoisted = role.IsHoisted;
            _role.IsManaged = role.IsManaged;
            _role.IsMentionable = role.IsMentionable;
            _role.DiscordId = role.Id.ToString();
            return _role;
        }

        /// <Summary>Converts a RoleModel to a RoleProperties class which can be used to modify a role or create a role.
        /// </Summary>
        /// <param name="roleModel">The RoleModel which will be used to create a RoleProperties.</param>
        /// <return>A RoleProperties.</return>
        public RoleProperties ModelToRoleProperties(RoleModel roleModel)
        {
            var properties = new RoleProperties();
            properties.Name = roleModel.Name;
            properties.Hoist = roleModel.IsHoisted;
            properties.Mentionable = roleModel.IsMentionable;
            properties.Position = roleModel.Position;
            properties.Permissions = roleModel.Permissions.ToGuildPermission();
            properties.Color = roleModel.Color.ToColor();
            return properties;
        }
    }
}