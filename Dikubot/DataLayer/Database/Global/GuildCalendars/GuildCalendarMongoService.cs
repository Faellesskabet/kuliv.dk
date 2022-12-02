using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Global.GuildCalendars;

public class GuildCalendarMongoService : GlobalMongoService<GuildCalendarModel>
{
    public GuildCalendarMongoService(Database database) : base(database)
    {
    }

    public override string GetCollectionName()
    {
        return "GuildCalendars";
    }

    public GuildCalendarModel Get(SocketGuild guild)
    {
        return Get(model => model.GuildId == guild.Id);
    }
}