using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Course
{
    public class CourseModel : Model
    {

        [BsonElement("Name")] [BsonUnique]
        public string Name
        {
            get;
            set;
        }
        
        [BsonElement("MainCategoryId")] 
        public string MainCategoryId { get; set; }
        
        [BsonElement("TeamCategoryIdList")]
        public string[] TeamCategoryIdList { get; set; }
        
        //MemberRoleModel is the role we will assign to the students enrolled in the course
        [BsonElement("MemberRoleModelId")]
        public string MemberRoleModelId { get; set; }
        
        
        //AdminRoleModel is the role we will assign to the TA
        [BsonElement("AdminRoleModelId")]
        public string AdminRoleModelId { get; set; }
        
    }
}