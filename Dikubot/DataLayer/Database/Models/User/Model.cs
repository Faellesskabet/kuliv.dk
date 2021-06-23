using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dikubot.Database.Models.Education;
using Dikubot.Database.Models.Group;
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

        [BsonElement("Verified")] [BsonIgnoreIfDefault] public bool Verified { get; set; }
        [BsonElement("LastMessage")] public string LastMessage { get; set; }

        private HashSet<UserRoleModel> _roles = new HashSet<UserRoleModel>();

        [BsonElement("Roles")]
        public UserRoleModel[] Roles
        {
            get => _roles.ToArray(); 
            set => _roles = new HashSet<UserRoleModel>(value);
        }

        public bool HasRole(string name, SocketGuild guild)
        {
            RoleServices services = new RoleServices(guild);
            return _roles.Any(model =>
                String.Equals(services.Get(model.RoleId)?.Name, name, StringComparison.CurrentCultureIgnoreCase));
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