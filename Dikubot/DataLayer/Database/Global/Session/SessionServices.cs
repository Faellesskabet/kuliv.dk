using Dikubot.DataLayer.Database.Guild;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Global.Session
{
    public class SessionServices : GlobalServices<SessionModel>
    {
        public SessionServices(SocketGuild guild) : base("UserSessions")
        {
        }
    }
}