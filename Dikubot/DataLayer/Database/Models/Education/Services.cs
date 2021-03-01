using Discord.WebSocket;

namespace Dikubot.Database.Models.Education
{
    public class EducationServices : Services<EducationModel>
    {
        public EducationServices(SocketGuild guild) : base("Main", "Education", guild)
        {
        }
    }
}