using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Calendar.Equipment
{
    public class EquipmentServices : GuildServices<EquipmentModel>
    {
        public EquipmentServices(SocketGuild guild) : base("Equipment", guild)
        {
        }

        

    }
}