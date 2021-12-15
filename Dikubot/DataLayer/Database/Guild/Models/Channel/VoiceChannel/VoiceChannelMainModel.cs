using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel
{
    /// <summary>
    /// Class for elements in the VoiceChannel collection.
    /// </summary>
    public class VoiceChannelMainModel : ChannelMainModel
    {
        [BsonElement("Bitrate")] public int Bitrate { get; set; } = 64000;
        [BsonElement("UserLimit")] public int? UserLimit { get; set; }
        [BsonElement("DeleteOnLeave")] public bool DeleteOnLeave { get; set; }
        [BsonElement("ExpandId")] public string ExpandId { get; set; }
        [BsonElement("ExpandOnJoin")] public bool ExpandOnJoin { get; set; }
        [BsonElement("DiscordCategoryId")] public string DiscordCategoryId { get; set; }
    }
}