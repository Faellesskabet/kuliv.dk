using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.User
{
    public class Model
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }
    
        [BsonElement("Verified")]
        public decimal Verified { get; set; }
        
        [BsonElement("Major")]
        public string Major { get; set; }

        [BsonElement("ProdigyPercentile")]
        public string ProdigyPercentile { get; set; }
    }
}