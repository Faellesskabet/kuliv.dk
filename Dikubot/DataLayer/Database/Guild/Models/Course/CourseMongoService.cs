using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Course
{
    public class CourseMongoService : GuildMongoService<CourseMainModel>
    {
        public CourseMongoService(Database database, SocketGuild guild) : base(database, guild)
        {
        }
        public bool NameAlreadyTaken(CourseMainModel courseMainModel)
        {
            return Get(model => model.Name == courseMainModel.Name)?.Id != courseMainModel.Id;
        }

        public override string GetCollectionName()
        {
            return "Course";
        }
    }
}