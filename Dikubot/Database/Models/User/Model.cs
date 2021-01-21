using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models
{
    /// <summary>
    /// Class for elements in the User collection.
    /// </summary>
    public class UserModel : Model
    {
        [BsonElement("Email")]
        public string Email { get; set; }
    
        [BsonElement("Verified")]
        public string Verified { get; set; }
        
        [BsonElement("Major")]
        public string Major { get; set; }

        [BsonElement("ProdigyPercentile")]
        public string ProdigyPercentile { get; set; }
    }
}