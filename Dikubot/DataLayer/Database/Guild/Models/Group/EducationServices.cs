using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Group
{
    public class EducationServices : GuildServices<GroupMainModel>
    {
        public EducationServices(SocketGuild guild) : base("Education", guild)
        {
        }
        
       
    }
}