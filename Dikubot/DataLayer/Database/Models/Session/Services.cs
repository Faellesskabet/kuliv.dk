using System;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Dikubot.Database.Models.Session
{
    public class SessionServices : Services<SessionModel>
    {
        public SessionServices(SocketGuild guild) : base("Main", "UserSessions", guild)
        {
        }
    }
}