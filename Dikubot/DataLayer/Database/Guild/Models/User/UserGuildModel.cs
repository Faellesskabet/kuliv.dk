using System;
using System.Collections.Generic;
using System.Linq;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.DataLayer.Database.Guild.Models.User.SubModels;
using Dikubot.DataLayer.Database.Interfaces;
using Dikubot.Discord;
using Discord.WebSocket;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.User
{
    /// <summary>
    /// Class for elements in the User collection.
    /// </summary>
    public class UserGuildModel : MainModel
    {
        [BsonElement("Name")] public string Name { get; set; }

        [BsonElement("DiscordId")]
        [BsonUnique]
        public string DiscordId { get; set; }

        [BsonIgnore]
        public SocketUser DiscordUser
        {
            get => DiscordBot.ClientStatic.GetUser(Convert.ToUInt64(DiscordId));
            set => DiscordId = value.Id.ToString();
        }
        
        [BsonElement("LastMessage")] public string LastMessage { get; set; }

        private HashSet<UserRoleModel> _roles = new HashSet<UserRoleModel>();

        [BsonElement("Roles")]
        public UserRoleModel[] Roles
        {
            get => _roles.ToArray(); 
            set => _roles = new HashSet<UserRoleModel>(value);
        }
        
        public bool HasRole(ulong roleId, RoleMongoService roleMongoService)
        {
            return _roles.Any(model => Convert.ToUInt64(roleMongoService.Get(model.RoleId).DiscordId) == roleId);
        }
        
        public bool HasRole(Guid guid)
        {
            return _roles.Any(model => model.RoleId == guid);
        }
        
        
        [BsonElement("GroupIds")]
        public HashSet<Guid> GroupIds
        {
            get;
            set;
        }

        [BsonElement("IsBot")] public bool IsBot { get; set; }
        [BsonElement("Username")] public string Username { get; set; }
        [BsonElement("JoinedAt")] public DateTime JoinedAt { get; set; }

        [BsonElement("Banned")] public bool IsBanned { get; set; } = false;
        

        /// <summary>
        /// Tells us whether or not a UserRoleModel is active. By active, it means that has started and not expired. This is only relevant for UserRoles which has specified an end- and or startdate
        /// </summary>
        /// <param name="roleMainModel">It will check if there exists a roleModel with the same ID and that its active</param>
        /// <returns>Returns whether the role is active</returns>
        public bool IsRoleActive(RoleMainModel roleMainModel)
        {
            return IsRoleActive(new UserRoleModel(roleMainModel));
        }
        

        public bool IsRoleActive(UserRoleModel userRoleModel)
        {
            return _roles.TryGetValue(userRoleModel, out userRoleModel) && ((IActiveTimeFrame)userRoleModel).IsActive();
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

        public int RemoveRole(Guid guid)
        {
            return _roles.RemoveWhere(model => model.RoleId == guid);
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