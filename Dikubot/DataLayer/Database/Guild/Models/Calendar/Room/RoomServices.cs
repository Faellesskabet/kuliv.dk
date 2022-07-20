using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Guild.Models.Equipment;
using Dikubot.Discord;
using Discord.WebSocket;


namespace Dikubot.DataLayer.Database.Guild.Models.Room
{
    public class RoomServices : GuildServices<RoomModel>
    {
        public RoomServices(SocketGuild guild) : base("Room", guild)
        {
        }
        public RoomServices(string guidId) : base("Room", DiscordBot.Client.Guilds?.FirstOrDefault(g => g.Id.ToString().Equals(guidId)))
        {
        }
        

    }
}