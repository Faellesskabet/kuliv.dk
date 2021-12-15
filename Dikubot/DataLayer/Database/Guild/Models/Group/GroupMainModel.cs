using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Group
{
    public class GroupMainModel : MainModel
    {
        
        /// <summary>
        /// The name is displayed in the Group list
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }
        
        /// <summary>
        /// The roleid is the role the user will be assigned upon joining the group
        /// </summary>
        [BsonElement("RoleId")]
        public Guid RoleId { get; set; }
        
        /// <summary>
        /// Is used to indicate whether or not any one can join the group
        /// </summary>
        [BsonElement("Public")]
        public bool Public { get; set; }

        [BsonElement("Type")]
        public GroupType Type { get; set; }
        

        public enum GroupType
        {
            Education
        }
    }
}