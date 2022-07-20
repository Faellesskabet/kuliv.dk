using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Dikubot.DataLayer.Database.Guild.Models.Equipment;
using Dikubot.Discord;
using Discord.WebSocket;


namespace Dikubot.DataLayer.Database.Guild.Models.Equipment
{
    public class EquipmentServices : GuildServices<EquipmentModel>
    {
        public EquipmentServices(SocketGuild guild) : base("Equipment", guild)
        {
        }
        public EquipmentServices(string guidId) : base("Equipment", DiscordBot.Client.Guilds?.FirstOrDefault(g => g.Id.ToString().Equals(guidId)))
        {
        }
        

    }
}