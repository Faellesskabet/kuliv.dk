using System;
using Dikubot.Discord;
using Discord.WebSocket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Database.Models
{
    /// <summary>
    /// Class for elements in the User collection.
    /// </summary>
    public class UserModel : Model
    {

        [BsonElement("Email")] [BsonUnique]
        public string Email { get; set; }
        [BsonElement("Name")] public string Name { get; set; }
        
        [BsonElement("DiscordId")] [BsonUnique]
        public string DiscordId { get; set; }
        [BsonIgnore] public SocketUser DiscordUser
        {
            get => DiscordBot.client.GetUser(Convert.ToUInt64(DiscordId));
            set => DiscordId = value.Id.ToString();
        }
        [BsonElement("Verified")] public bool Verified { get; set; } = false;
        [BsonElement("LastMessage")] public string LastMessage { get; set; }

    }
}