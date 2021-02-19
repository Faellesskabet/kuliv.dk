using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Course.SubModels
{
    public class TeamCategory
    {
        
        /// <summary>
        /// Name of the team
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// This is the team category. Each team in a course will have their own category. Only the Admin will be able to access all the categories
        /// </summary>
        [BsonElement("TeamCategoryId")]
        public string TeamCategoryId { get; set; }
        
        /// <summary>
        /// This role has access to the TeamCategory
        /// </summary>
        [BsonElement("TeamRoleModelId")]
        public Guid[] TeamRoleModelId { get; set; }
    }
}