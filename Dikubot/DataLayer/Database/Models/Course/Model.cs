using System;
using Dikubot.Database.Models.Course.SubModels;
using Dikubot.Database.Models.Interfaces;
using Dikubot.Database.Models.TextChannel;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Course
{
    public class CourseModel : Model, IActiveTimeFrame
    {
        /// <summary>
        /// This is the name of the course
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }
        
        [BsonElement("Area")]
        public string FullName { get; set; }
        
        [BsonElement("CourseUrl")]
        public string CourseUrl { get; set; }
        
        /// <summary>
        /// This specifies the course's main channel - a channel which everyone enrolled in the course will have access to.
        /// </summary>
        [BsonElement("MainTextChannelModelId")]
        public Guid MainTextChannelModelId { get; set; }

        public TextChannelModel GetMainTextChannel(SocketGuild guild)
        {
            return new TextChannelServices(guild).Get(MainTextChannelModelId);
        }

        /// <summary>
        /// This is the team category list. Each team in a course will have their own category. Only the Admin will be able to access all the categories
        /// </summary>
        [BsonElement("TeamCategoryList")]
        public TeamCategory[] TeamCategoryList { get; set; }
        
        /// <summary>
        /// This is used to store the amount of teams, and will be helpful to know whether or not TeamCategoryIdList is up to date
        /// </summary>
        [BsonElement("AmountOfTeams")]
        public int TeamLength { get; set; }

        /// <summary>
        /// MemberRoleModel is the role we will assign to the students enrolled in the course
        /// </summary>
        [BsonElement("MemberRoleModelId")]
        public Guid MemberRoleModelId { get; set; }

        /// <summary>
        /// AdminRoleModel is the role we will assign to the TA
        /// </summary>
        [BsonElement("AdminRoleModelId")]
        public Guid AdminRoleModelId { get; set; }
        
        /// <summary>
        /// The startdate of the course
        /// </summary>
        [BsonElement("StartDate")][BsonRepresentation(BsonType.DateTime)]
        public DateTime StartDate { get; set; }
        
        /// <summary>
        /// The enddate of the course
        /// </summary>
        [BsonElement("EndDate")][BsonRepresentation(BsonType.DateTime)]
        public DateTime EndDate { get; set; }
        
        /// <summary>
        /// How often does the course repeat
        /// </summary>
        [BsonElement("RepeatEveryNthYear")][BsonRepresentation(BsonType.DateTime)]
        public double RepeatEveryNthYear { get; set; }
    }
}