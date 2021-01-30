using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dikubot.Database.Models.SubModels;
using Dikubot.Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models
{
    /// <summary>
    /// Class for elements in the User collection.
    /// </summary>
    public class UserModel : Model
    {
        private string _email;
        [BsonElement("Email")] [BsonUnique]
        public string Email { 
            get => _email;
            set => _email = value.ToLower();
        }
        [BsonElement("Name")] public string Name { get; set; }
        
        [BsonElement("DiscordId")] [BsonUnique]
        public string DiscordId { get; set; }
        [BsonIgnore] public SocketUser DiscordUser
        {
            get => DiscordBot.client.GetUser(Convert.ToUInt64(DiscordId));
            set => DiscordId = value.Id.ToString();
        }
        [BsonElement("Verified")] public bool Verified { get; set; } = false;
        [BsonElement("LastMessage")] public string LastMessage { get; set; }

        private HashSet<UserRoleModel> _roles = new HashSet<UserRoleModel>();
        [BsonElement("Roles")]
        public UserRoleModel[] Roles { get => _roles.ToArray(); set => _roles = new HashSet<UserRoleModel>(value); }
        
        /// <summary>
        /// AddRole adds the role to a HashSet of roles. This means no duplicates are allowed. If the role is already present, then it is overwritten by the new role
        /// </summary>
        /// <param name="roleModel">The role to be added</param>
        public void AddRole(RoleModel roleModel)
        {
            AddRole(new UserRoleModel(roleModel));
        }
        
        public bool RemoveRole(RoleModel roleModel)
        {
            return RemoveRole(new UserRoleModel(roleModel));
        }
        
        public bool HasRole(RoleModel roleModel)
        {
            return HasRole(new UserRoleModel(roleModel));
        }
        
        /// <summary>
        /// AddRole adds the role to a HashSet of roles. This means no duplicates are allowed. If the role is already present, then it is overwritten by the new role
        /// </summary>
        /// <param name="userRoleModel">The role to be added</param>
        /// <returns>Returns whether it was successfully added (which should always be true)</returns>
        public bool AddRole(UserRoleModel userRoleModel)
        {
            if (_roles.Contains(userRoleModel))
            {
                RemoveRole(userRoleModel);
            }
            return _roles.Add(userRoleModel);
        }
        
        public bool RemoveRole(UserRoleModel userRoleModel)
        {
            return _roles.Remove(userRoleModel);
        }

        public bool HasRole(UserRoleModel userRoleModel)
        {
            return _roles.Contains(userRoleModel);
        }

        public void ClearRoles()
        {
            _roles.Clear();
        }
        
    }
}