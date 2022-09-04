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
            IEnumerable<RoleMainModel> roles = Permission.IsNullOrEmpty() ? new List<RoleMainModel>() : 
                Permission.Select(guid => roleServices.Get(model => model.Id == guid)).Where(model => model != null);
            
            // Replace by ForiegnKey system
            var roleMainModels = roles as RoleMainModel[] ?? roles.ToArray();
            Permission = new HashSet<Guid>(roleMainModels.Select(model => model.Id));
            return roleMainModels;
        }
        
        [BsonIgnore]
        public IEnumerable<Guid> PermissionEnumerable { get => Permission; set => Permission = new HashSet<Guid>(value); }

        /// <summary>
        /// Color is the color used to represent the category in the frontend.
        /// Colors are hexadecimal
        /// </summary>
        [BsonElement("Color")]
        public string Color { get; set; }
        

    }
    
    public class JoinRoleMainModel
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
        /// The roleid is the internal RoleModel id the user will be assigned upon joining the group.
        /// </summary>
        [BsonElement("RoleId")] public Guid RoleId  { get;  set; }

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
            IEnumerable<RoleMainModel> roles = Permission.IsNullOrEmpty() ? new List<RoleMainModel>() : 
                Permission.Select(guid => roleServices.Get(model => model.Id == guid)).Where(model => model != null);
            
            // Replace by ForiegnKey system
            var roleMainModels = roles as RoleMainModel[] ?? roles.ToArray();
            Permission = new HashSet<Guid>(roleMainModels.Select(model => model.Id));
            return roleMainModels;
        }
        
        [BsonIgnore]
        public IEnumerable<Guid> PermissionEnumerable { get => Permission; set => Permission = new HashSet<Guid>(value); }
    }
}