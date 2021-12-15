using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Discord;
using Discord.WebSocket;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.JoinRole
{
    
    public class JoinRoleCategoryCollectionMainModel : MainModel
    {
        
        [BsonElement("TextRoles")]
        public List<JoinRoleCategoryMainModel> TextCategory { get; set; }
    }
    

    public class JoinRoleCategoryMainModel : MainModel
    {
        /// <summary>
        /// The name is displayed in the Group list
        /// </summary>
        [BsonElement("Name")]
        [Required]
        public string Name { get; set; }
        
        [Required]
        [BsonElement("Decs")]
        public string Decs { get; set; }
        
        [BsonElement("TextRoles")]
        public List<JoinRoleMainModel> TextRoles { get; set; }

        public string GuildId;

        
        [BsonElement("Permission")] public HashSet<Guid> Permission
        {
            get;
            set;
        }
        


        /// <summary>
        /// TODO: Implementer
        /// Alle dem som skal listenes, hvis serveren genstart, ændre hvilke kanaler man kan joine etc.
        /// </summary>
        [BsonElement("Listeners")]
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
        
        
        /// <summary>
        /// The Emoji is showen for the goupe.
        /// </summary>
        [BsonElement("Emoji")]
        public string Emoji  { get;  set; }
        
        
        
        /// <summary>
        /// Is used to indicate whether or not any one can join the group
        /// </summary>
        [BsonElement("Public")]
        public bool Public { get; set; }
        
        
        public EmbedFieldBuilder Field(SocketGuild socketGuild)
        {
            
            IRole role = socketGuild.Roles.FirstOrDefault(x => x.Id.ToString().Equals(RoleId));
            var field = new EmbedFieldBuilder();
            field.Name = $"{Emoji} - {role?.Name}";
            field.Value = $" - "; //[🟢](https://satyr.dk)
            field.IsInline = true;
            return field;
        }

        public JoinRoleMainModel()
        {
        }
        public JoinRoleMainModel(string roleId, string emoji, string desc = "-")
        {
            RoleId = roleId;
            Emoji = emoji;
            Desc = desc;
        }
        
        public void JoinRoleDate( List<RoleMainModel> roles){
            var role= roles.Find(t => RoleId != null && t.DiscordId == RoleId);
            Name = role?.Name;
            if (role != null) Id = role.Id;
        }
        
        
        
    }
}