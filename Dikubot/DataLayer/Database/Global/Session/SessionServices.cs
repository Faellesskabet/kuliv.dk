using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Interfaces;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Global.Session
{
    public class SessionServices : GlobalServices<SessionModel>
    {
        public SessionServices() : base("UserSessions")
        {
        }
        
    }
}