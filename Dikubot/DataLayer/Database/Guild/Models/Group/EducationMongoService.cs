using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Group
{
    public class EducationMongoService : GuildMongoService<GroupMainModel>
    {
        public EducationMongoService(SocketGuild guild) : base("Education", guild)
        {
        }
        
       
    }
}