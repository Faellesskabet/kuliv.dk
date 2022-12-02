using System;
using System.ComponentModel.DataAnnotations;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Dikubot.DataLayer.Database.Guild.Models.Course.SubModels;
using Dikubot.DataLayer.Database.Interfaces;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Course
{
    public class CourseMainModel : MainModel, IActiveTimeFrame
    {
        /// <summary>
        /// This is the name of the course
        /// </summary>
        [BsonElement("Name")]
        [Required]
        public string Name { get; set; }
        
        [BsonElement("FullName")]
        [Required]
        public string FullName { get; set; }
        
        [Required]
        [BsonElement("CourseUrl")]
        [DataType(DataType.Html)]
        public string CourseUrl { get; set; }
        
        /// <summary>
        /// This specifies the course's main channel - a channel which everyone enrolled in the course will have access to.
        /// </summary>
        
        [Required]
        [BsonElement("MainTextChannelModelId")]
        public Guid MainTextChannelModelId { get; set; }
        

        /// <summary>
        /// This is the team category list. Each team in a course will have their own category. Only the Admin will be able to access all the categories
        /// </summary>
        [BsonElement("TeamCategoryList")]
        public TeamCategory[] TeamCategoryList { get; set; }
        
        /// <summary>
        /// This is used to store the amount of teams, and will be helpful to know whether or not TeamCategoryIdList is up to date
        /// </summary>
        [Required]
        [BsonElement("AmountOfTeams")]
        [Range(typeof(int), "0", "100", ErrorMessage="{0} needs to be between {1} and {2}")]
        public int TeamLength { get; set; }

        /// <summary>
        /// MemberRoleModel is the role we will assign to the students enrolled in the course
        /// </summary>
        [BsonElement("MemberRoleModelId")]
        public Guid MemberRoleModelId { get; set; }

        /// <summary>
        /// AdminRoleModel is the role we will assign to the TA
        /// </summary>
        [Display(Name = "TA's Role")]
        [BsonElement("AdminRoleModelId")]
        public Guid AdminRoleModelId { get; set; }
        
        /// <summary>
        /// The startdate of the course
        /// </summary>
        [BsonElement("StartDate")][BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        [Required]
        public DateTime StartDate { get; set; } = DateTime.Today;
        
        /// <summary>
        /// The enddate of the course
        /// </summary>
        [BsonElement("EndDate")][BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime EndDate { get; set; } = DateTime.Today;
        
        /// <summary>
        /// How often does the course repeat
        /// </summary>
        [BsonElement("RepeatEveryNthYear")]
        [Required]
        [Range(typeof(double), "0", "50", ErrorMessage="{0} needs to be between {1} and {2}")]
        public double RepeatEveryNthYear { get; set; }
    }
}