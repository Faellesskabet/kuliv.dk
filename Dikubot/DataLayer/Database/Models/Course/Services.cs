using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.Database.Models.Course
{
    public class CourseServices : Services<CourseModel>
    {
        public CourseServices(SocketGuild guild) : base("Main", "Courses", guild)
        {
        }
        public bool NameAlreadyTaken(CourseModel courseModel)
        {
            return Get(model => model.Name == courseModel.Name)?.Id != courseModel.Id;
        }
    }
}