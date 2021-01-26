using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Dikubot.Database.Models.Session
{
    public class SessionServices : Services<SessionModel>
    {
        public SessionServices() :base("Main", "UserSessions") { }
    }
}