using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel
{
    /// <summary>
    /// Class for elements in the TextChannel collection.
    /// </summary>
    public class TextChannelMainModel : ChannelMainModel
    {
        [BsonElement("Topic")] public string Topic { get; set; }
        [BsonElement("IsNsfw")] public bool IsNsfw { get; set; }
        [BsonElement("SlowModeInterval")] public int SlowModeInterval { get; set; }
        [BsonElement("DiscordCategoryId")] public string DiscordCategoryId { get; set; }
        [BsonElement("IsQuoteChannel")] public bool IsQuoteChannel { get; set; }
    }
}