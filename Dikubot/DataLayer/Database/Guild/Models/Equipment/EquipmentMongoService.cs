using Dikubot.Extensions.Models.Equipment;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.Equipment
{
    public class EquipmentMongoService : GuildMongoService<EquipmentModel>
    {
        public EquipmentMongoService(Database database, SocketGuild guild) : base(database, guild)
        {
        }


        public override string GetCollectionName()
        {
            return "Equipment";
        }
    }
}