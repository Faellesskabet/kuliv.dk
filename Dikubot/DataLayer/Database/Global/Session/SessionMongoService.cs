using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Interfaces;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Global.Session
{
    public class SessionMongoService : GlobalMongoService<SessionModel>
    {
        public SessionMongoService(Database database) : base(database)
        {
        }

        public override string GetCollectionName()
        {
            return "UserSessions";
        }
    }
}