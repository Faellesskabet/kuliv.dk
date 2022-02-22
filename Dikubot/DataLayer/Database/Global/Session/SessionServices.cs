using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Interfaces;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Global.Session
{
    public class SessionServices : GlobalServices<SessionModel>, IIndexed<SessionModel>
    {
        public SessionServices() : base("UserSessions")
        {
        }

        public IEnumerable<IndexKeysDefinition<SessionModel>> GetIndexes()
        {
            return new List<IndexKeysDefinition<SessionModel>>
            {
                Builders<SessionModel>.IndexKeys.Ascending(model => model.UserId),
                Builders<SessionModel>.IndexKeys.Ascending(model => model.Expires)
            };
        }
    }
}