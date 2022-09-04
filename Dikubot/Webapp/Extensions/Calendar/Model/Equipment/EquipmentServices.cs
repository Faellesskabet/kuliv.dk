using System.Linq;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.Discord;
using Dikubot.Extensions.Models.Equipment;
using Discord.WebSocket;

namespace Dikubot.Extensions.Models.Equipment.Equipment
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