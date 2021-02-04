using MongoDB.Driver;

namespace Dikubot.Database.Models.Course
{
    public class CourseServices : Services<UserModel>
    {
        public CourseServices() : base("Main", "Courses")
        {
        }
    }
}