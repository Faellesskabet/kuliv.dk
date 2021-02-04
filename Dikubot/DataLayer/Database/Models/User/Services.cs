using System;
using Discord.WebSocket;
using System.Collections.Generic;
using Dikubot.Database.Models.Role;
using Dikubot.Database.Models.SubModels;
using Discord;
using Discord.Rest;
using MongoDB.Driver;

namespace Dikubot.Database.Models
{
    /// <summary>
    /// Class for for retrieving information from the User collection.
    /// </summary>
    public class UserServices : Services<UserModel>
    {
        public UserServices() : base("Main", "Users")
        {
        }

        public UserModel Get(SocketUser user)
        {
            UserModel model = this.Get(inmodel => inmodel.DiscordId == user.Id.ToString());
            if (model == null)
            {
                model = new UserModel();
                model.DiscordUser = user;
            }

            return model;
        }

        public bool Exists(SocketUser user)
        {
            return Exists(model => model.DiscordId == user.Id.ToString());
        }

        public bool EmailExists(string email)
        {
            email = email.ToLower();
            return Exists(model => model.Email == email);
        }

        public UserModel GetFromEmail(string email)
        {
            email = email.ToLower();
            return Get(model => model.Email == email);
        }

        /// <Summary>Inserts a UserModel in the collection. If a UserModel with the same ID or discordID
        /// already exists, then we imply invoke Update() on the model instead.</Summary>
        /// <param name="modelIn">The Model one wishes to be inserted.</param>
        /// <return>A Model.</return>
        public new UserModel Upsert(UserModel modelIn)
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
                Update(m => m.DiscordId == modelIn.DiscordId, modelIn, new ReplaceOptions() {IsUpsert = true});
                return modelIn;
            }

            Insert(modelIn);
            return modelIn;
        }

        /// <Summary>Removes a UserModel from the collection by it's unique elements.</Summary>
        /// <param name="modelIn">The Model one wishes to remove.</param>
        /// <return>Void.</return>
        public new void Remove(UserModel modelIn)
        {
            Remove(model => model.Id == modelIn.Id);
            Remove(model => model.DiscordId == modelIn.DiscordId);
        }

        /// <Summary>Converts a SocketGuildUser to a UserModel.</Summary>
        /// <param name="role">The SocketGuildUser model one wishes to be converted.</param>
        /// <return>A UserModel.</return>
        public UserModel SocketToModel(SocketGuildUser user)
        {
            var roleServices = new RoleServices();
            var _user = new UserModel();
            _user.DiscordId = user.Id.ToString();
            _user.IsBot = user.IsBot;
            _user.Username = user.Username;
            _user.JoinedAt = user.JoinedAt?.DateTime ?? DateTime.Now;
            _user.Name = user.Nickname;
            var roles = new List<UserRoleModel>();
            foreach (var role in user.Roles)
                roles.Add(new UserRoleModel(roleServices.Get(role.Id).Id));
            _user.Roles = roles.ToArray();
            return _user;
        }

        /// <Summary>Converts a RestGuildUser to a UserModel.</Summary>
        /// <param name="role">The RestGuildUser model one wishes to be converted.</param>
        /// <return>A UserModel.</return>
        public UserModel RestToModel(RestGuildUser user)
        {
            var roleServices = new RoleServices();
            var _user = new UserModel();
            _user.DiscordId = user.Id.ToString();
            _user.IsBot = user.IsBot;
            _user.Username = user.Username;
            _user.JoinedAt = user.JoinedAt?.DateTime ?? DateTime.Now;
            _user.Name = user.Nickname;
            var roles = new List<UserRoleModel>();
            foreach (var roleId in user.RoleIds)
                roles.Add(new UserRoleModel(roleServices.Get(roleId).Id));
            _user.Roles = roles.ToArray();
            return _user;
        }

        /// <Summary>Converts a UserModel to a GuildUserProperties.</Summary>
        /// <param name="role">The UserModel model one wishes to be converted.</param>
        /// <return>A GuildUserProperties.</return>
        public GuildUserProperties ModelToProperties(SocketGuild guild, UserModel user)
        {
            var properties = new GuildUserProperties();
            properties.Nickname = user.Name;
            var roles = new List<IRole>();
            var roleIds = new List<ulong>();
            foreach (var userRole in user.Roles)
            {
                var id = Convert.ToUInt64(userRole.RoleModel.DiscordId);
                roleIds.Add(id);
                roles.Add(guild.GetRole(id));
            }

            properties.Roles = roles;
            properties.RoleIds = roleIds;
            return properties;
        }
    }
}