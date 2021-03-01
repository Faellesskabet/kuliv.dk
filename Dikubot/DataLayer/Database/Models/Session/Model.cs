using System;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Dikubot.Database.Models.Session
{
    public class SessionModel : Model
    {
        public SessionModel(UserModel userModel) : this(userModel, DateTime.Now.AddMonths(1))
        {
        }

        public SessionModel(UserModel userModel, DateTime expires) : this(userModel.Id, expires)
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
        
        public UserModel GetUserModel(SocketGuild guild)
        {
            return new UserServices(guild).Get(this.UserId);
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
    }
}