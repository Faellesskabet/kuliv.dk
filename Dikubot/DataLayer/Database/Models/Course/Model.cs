using System;
using Dikubot.Database.Models.TextChannel;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Course
{
    public class CourseModel : Model
    {

        private static TextChannelServices _textChannelServices = new TextChannelServices();

        [BsonElement("Name")] [BsonUnique]
        public string Name
        {
            get;
            set;
        }
        
        [BsonElement("MainTextChannelModelId")] 
        public Guid MainTextChannelModelId { get; set; }

        [BsonIgnore]
        public TextChannelModel MainTextChannelModel
        {
            get => _textChannelServices.Get(MainTextChannelModelId);
            set => MainTextChannelModelId = value.Id;
        }
        
        [BsonElement("TeamCategoryIdList")]
        public string[] TeamCategoryIdList { get; set; }
        
        //MemberRoleModel is the role we will assign to the students enrolled in the course
        [BsonElement("MemberRoleModelId")]
        public Guid MemberRoleModelId { get; set; }

        //AdminRoleModel is the role we will assign to the TA
        [BsonElement("AdminRoleModelId")]
        public Guid AdminRoleModelId { get; set; }
        
    }
}