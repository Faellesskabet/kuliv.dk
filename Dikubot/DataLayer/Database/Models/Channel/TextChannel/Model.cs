using Dikubot.Database.Models.Channel;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models.TextChannel
{
    /// <summary>
    /// Class for elements in the TextChannel collection.
    /// </summary>
    public class TextChannelModel : ChannelModel
    {
        [BsonElement("Topic")] public string Topic { get; set; }
        [BsonElement("IsNsfw")] public bool IsNsfw { get; set; }
        [BsonElement("SlowModeInterval")] public int SlowModeInterval { get; set; }
        [BsonElement("DiscordCategoryId")] public string DiscordCategoryId { get; set; }
        [BsonElement("IsQuoteChannel")] public bool IsQuoteChannel { get; set; }
    }
}