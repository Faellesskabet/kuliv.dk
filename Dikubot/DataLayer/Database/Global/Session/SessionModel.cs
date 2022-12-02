using System;
using Dikubot.DataLayer.Database.Global.Session.DiscordAuthentication;
using Dikubot.DataLayer.Database.Global.User;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Session
{
    public class SessionModel : MainModel
    {
        public SessionModel(UserGlobalModel userMainModel) : this(userMainModel, DateTime.Now.AddDays(7))
        {
        }

        public SessionModel(UserGlobalModel userMainModel, DateTime expires) : this(userMainModel.Id, expires)
        {
        }

        public SessionModel(Guid userId, DateTime expires)
        {
            _userId = userId;
            _expires = expires;
        }

        private Guid _userId;

        [BsonElement("UserId")]
        [BsonRequired]
        public Guid UserId
        {
            get => _userId;
            set => _userId = value;
        }
        
        private DateTime _expires;

        [BsonElement("Expires")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonRequired]
        public DateTime Expires
        {
            get => _expires;
            set => _expires = value;
        }

        [BsonIgnore]
        public bool IsExpired
        {
            get => DateTime.Now > Expires;
        }
        
        public DiscordAuthenticationModel DiscordAuthenticationModel { get; set; }
    }
}