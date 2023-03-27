using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Global.Request;

public class ContactModel : MainModel
{
    /// <summary>
    ///     Name on the one who made the request, Contact infomation
    /// </summary>
    [BsonElement("Name")]
    public string Name { get; set; }

    /// <summary>
    ///     Telefon number on the person who made the request
    /// </summary>
    [BsonElement("Number")]
    public string Number { get; set; }

    /// <summary>
    ///     Email on the one who made the request, Contact infomation
    /// </summary>
    [BsonElement("Mail")]
    public string Mail { get; set; }
}