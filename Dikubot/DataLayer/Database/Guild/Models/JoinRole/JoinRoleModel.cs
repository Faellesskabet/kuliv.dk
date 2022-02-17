using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.Discord;
using Discord;
using Discord.WebSocket;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.JoinRole
{
    public class JoinRoleCategoryMainModel : MainModel
    {
        /// <summary>
        /// The name is displayed in the Group list
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The description of the category
        /// </summary>
        [BsonElement("Decs")]
        public string Decs { get; set; }
        
        /// <summary>
        /// TextRoles is a misleading name. This is the list of role objects in the category.
        /// </summary>
        [BsonElement("TextRoles")]
        public List<JoinRoleMainModel> TextRoles { get; set; }

        /// <summary>
        /// Users with roles in this list will be able to view the category.
        /// </summary>
        [BsonElement("Permission")]
        public HashSet<Guid> Permission { get; set; }
        
        public IEnumerable<RoleMainModel> GetPermissionRoles(RoleServices roleServices)
        {
            if (Permission.IsNullOrEmpty())
            {
                return new List<RoleMainModel>();
            }
            return Permission.Select(guid => roleServices.Get(model => model.Id == guid));
        }
        
        /// <summary>
        /// Color is the color used to represent the category in the frontend.
        /// Colors are hexadecimal
        /// </summary>
        [BsonElement("Color")]
        public string Color { get; set; }
        
        /// <summary>
        /// Obsolete, don't use this
        /// </summary>
        [BsonElement("Listeners")] [Obsolete("Does nothing.")]
        public Dictionary<string, string> Listeners { get; set; } = new Dictionary<string, string>();

    }
    
    public class JoinRoleMainModel : MainModel
    {
        
        /// <summary>
        /// The name is displayed in the Group list
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The roleid is the role the user will be assigned upon joining the group
        /// </summary>
        [BsonElement("Desc")]
        public string Desc  { get;  set; }

        /// <summary>
        /// The roleid is the role the user will be assigned upon joining the group
        /// </summary>
        [BsonElement("RoleId")] public string RoleId  { get;  set; }
        public SocketRole GetDiscordRole(SocketGuild guild)
        {
            return guild.GetRole(UInt64.Parse(RoleId));
        }

        /// <summary>
        /// The Emoji is showen for the goupe.
        /// </summary>
        [BsonElement("Emoji")]
        public string Emoji { get;  set; }
        
        /// <summary>
        /// Users with roles in this list will be join the role, if they also have access to the category.
        /// </summary>
        [BsonElement("Permission")] public HashSet<Guid> Permission { get; set; }
        public IEnumerable<RoleMainModel> GetPermissionRoles(RoleServices roleServices)
        {
            return Permission.Select(guid => roleServices.Get(model => model.Id == guid));
        }
        
        /// <summary>
        /// Obsolete, replaced by Permissions.
        /// Is used to indicate whether or not any one can join the group
        /// </summary>
        [BsonElement("Public")] [Obsolete("Replaced by permissions")]
        public bool Public { get; set; }
        
        
        
        
    }
}