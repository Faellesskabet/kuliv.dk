using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.Session
{
    public class SessionModel : Model
    {
        public SessionModel(UserModel userModel) : this(userModel, DateTime.Now.AddMonths(1)) {}
        public SessionModel(UserModel userModel, DateTime expires) : this(userModel.Id, expires) { }
        public SessionModel(string userId, DateTime expires)
        {
            _userId = userId;
            _expires = expires;
        }
        
        private string _userId;
        [BsonElement("UserId")][BsonRepresentation(BsonType.ObjectId)] [BsonRequired]
        public string UserId { get => _userId; set => _userId = value; }
        
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