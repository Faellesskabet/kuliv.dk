using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.User;

public class UserModel : MainModel
{
    
    [BsonElement("WayfId")]
    public string WayfId { get; set; }
    
}