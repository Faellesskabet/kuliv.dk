using Dikubot.Database.Models.Group;
using Discord.WebSocket;

namespace Dikubot.Database.Models.Education
{
    public class EducationServices : Services<GroupModel>
    {
        public EducationServices(SocketGuild guild) : base("Main", "Education", guild)
        {
        }
    }
}