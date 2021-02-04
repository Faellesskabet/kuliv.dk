using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dikubot.Database.Models.Interfaces;
using Dikubot.Database.Models.Role;
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

        [BsonElement("Email")]
        [BsonUnique]
        public string Email
        {
            get => _email;
            set => _email = value.ToLower();
        }

        [BsonElement("Name")] public string Name { get; set; }

        [BsonElement("DiscordId")]
        [BsonUnique]
        public string DiscordId { get; set; }

        [BsonIgnore]
        public SocketUser DiscordUser
        {
            get => DiscordBot.client.GetUser(Convert.ToUInt64(DiscordId));
            set => DiscordId = value.Id.ToString();
        }

        [BsonElement("Verified")] public bool Verified { get; set; } = false;
        [BsonElement("LastMessage")] public string LastMessage { get; set; }

        private HashSet<UserRoleModel> _roles = new HashSet<UserRoleModel>();

        [BsonElement("Roles")]
        public UserRoleModel[] Roles
        {
            get => _roles.Where(model => model.RoleModel != null).ToArray(); 
            set => _roles = new HashSet<UserRoleModel>(value);
        }

        [BsonElement("IsBot")] public bool IsBot { get; set; }
        [BsonElement("Username")] public string Username { get; set; }

        [BsonElement("JoinedAt")] public DateTime JoinedAt { get; set; }
        
        /// <summary>
        /// AddRole adds the role to a HashSet of roles. This means no duplicates are allowed. If the role is already present, then it is overwritten by the new role
        /// </summary>
        /// <param name="roleModel">The role to be added</param>
        public void AddRole(RoleModel roleModel)
        {
            AddRole(new UserRoleModel(roleModel));
        }

        /// <summary>
        /// Remove an existing role from the user
        /// </summary>
        /// <param name="roleModel">Removes an element with the same role ID</param>
        /// <returns>Whether any elements were removed</returns>
        public bool RemoveRole(RoleModel roleModel)
        {
            return RemoveRole(new UserRoleModel(roleModel));
        }

        /// <summary>
        /// Tells whether or not a user has a role with the same role ID
        /// </summary>
        /// <param name="roleModel">Will check if there is a roleModel with the same ID</param>
        /// <returns>Whether the user has the role</returns>
        public bool HasRole(RoleModel roleModel)
        {
            return HasRole(new UserRoleModel(roleModel));
        }

        /// <summary>
        /// Tells us whether or not a UserRoleModel is active. By active, it means that has started and not expired. This is only relevant for UserRoles which has specified an end- and or startdate
        /// </summary>
        /// <param name="roleModel">It will check if there exists a roleModel with the same ID and that its active</param>
        /// <returns>Returns whether the role is active</returns>
        public bool IsRoleActive(RoleModel roleModel)
        {
            return IsRoleActive(new UserRoleModel(roleModel));
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

        public bool IsRoleActive(UserRoleModel userRoleModel)
        {
            return _roles.TryGetValue(userRoleModel, out userRoleModel) && ((IActiveTimeFrame)userRoleModel).IsActive();
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