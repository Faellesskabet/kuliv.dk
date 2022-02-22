using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Session.DiscordAuthentication;

public class DiscordAuthenticationModel
{
    [BsonElement("AccessToken")]
    public string AccessToken { get; set; }
    [BsonElement("RefreshToken")]
    public string RefreshToken { get; set; }
}