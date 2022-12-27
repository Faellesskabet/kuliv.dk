using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild.Models.JoinRole;

public class JoinRoleMongoService : GuildMongoService<JoinRoleCategoryMainModel>
{
    public JoinRoleMongoService(Database database, SocketGuild guild) : base(database, guild)
    {
    }

    public override string GetCollectionName()
    {
        return "JoinRole";
    }
}