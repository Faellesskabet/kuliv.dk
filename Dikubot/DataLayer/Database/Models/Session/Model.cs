using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Dikubot.Database.Models.Session
{
    public class SessionModel : Model
    {
        public SessionModel(UserModel userModel) : this(userModel, DateTime.Now.AddMonths(1)) {}
        public SessionModel(UserModel userModel, DateTime expires) : this(userModel.Id, expires) { }
        public SessionModel(Guid userId, DateTime expires)
        {
            _userId = userId;
            _expires = expires;
        }

        private Guid _userId;
        [BsonElement("UserId")] [BsonRequired]
        public Guid UserId { get => _userId; set => _userId = value; }
        
        [BsonIgnore]
        public UserModel UserModel
        {
            get => new UserServices().Get(this.UserId);
            set => UserId = value.Id;
        }

        private DateTime _expires;
        [BsonElement("Expires")][BsonRepresentation(BsonType.DateTime)] [BsonRequired]
        public DateTime Expires { get => _expires; set => _expires = value; }

        [BsonIgnore]
        public bool IsExpired
        {
            get => DateTime.Now > Expires;
        }
    }
}