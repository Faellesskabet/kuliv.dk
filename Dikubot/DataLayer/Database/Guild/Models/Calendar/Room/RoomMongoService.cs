using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Guild.Models.Equipment;
using Dikubot.Discord;
using Discord.WebSocket;


namespace Dikubot.DataLayer.Database.Guild.Models.Room
{
    public class RoomMongoService : GuildMongoService<RoomModel>
    {
        public RoomMongoService(SocketGuild guild) : base("Room", guild)
        {
        }
        public RoomMongoService(string guidId) : base("Room", DiscordBot.ClientStatic.Guilds?.FirstOrDefault(g => g.Id.ToString().Equals(guidId)))
        {
        }
        

    }
}