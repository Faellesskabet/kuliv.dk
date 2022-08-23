using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Guild.Models.Equipment;
using Dikubot.Discord;
using Discord.WebSocket;


namespace Dikubot.DataLayer.Database.Guild.Models.Equipment
{
    public class EquipmentMongoService : GuildMongoService<EquipmentModel>
    {
        public EquipmentMongoService(SocketGuild guild) : base("Equipment", guild)
        {
        }
        public EquipmentMongoService(string guidId) : base("Equipment", DiscordBot.ClientStatic.Guilds?.FirstOrDefault(g => g.Id.ToString().Equals(guidId)))
        {
        }
        

    }
}