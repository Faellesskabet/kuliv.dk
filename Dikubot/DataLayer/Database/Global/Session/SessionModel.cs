using System;
using Dikubot.DataLayer.Database.Global.User.DiscordUser;
using Dikubot.DataLayer.Database.Global.Session.DiscordAuthentication;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Session;

public class SessionModel : MainModel
{
    public SessionModel(DiscordUserGlobalModel discordUserMainModel) : this(discordUserMainModel, DateTime.Now.AddDays(7))
    {
    }

    public SessionModel(DiscordUserGlobalModel discordUserMainModel, DateTime expires) : this(discordUserMainModel.Id, expires)
    {
    }

    public SessionModel(Guid userId, DateTime expires)
    {
        UserId = userId;
        Expires = expires;
    }

    [BsonElement("UserId")] [BsonRequired] public Guid UserId { get; set; }

    [BsonElement("Expires")]
    [BsonRepresentation(BsonType.DateTime)]
    [BsonRequired]
    public DateTime Expires { get; set; }

    [BsonIgnore] public bool IsExpired => DateTime.Now > Expires;

    public DiscordAuthenticationModel DiscordAuthenticationModel { get; set; }
}