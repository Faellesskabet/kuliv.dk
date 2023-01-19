using System;
using Dikubot.DataLayer.Database.Global.Session.DiscordAuthentication;
using Dikubot.DataLayer.Database.Global.User;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Session;

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