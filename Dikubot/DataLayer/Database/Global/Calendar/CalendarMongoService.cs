using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Global.Calendar;

public class CalendarMongoService : GlobalMongoService<CalendarModel>
{
    private readonly DiscordSocketClient _discordBotClient;
    private readonly IGuildMongoFactory _guildMongoFactory;

    public CalendarMongoService(Database database, DiscordSocketClient discordBotClient,
        IGuildMongoFactory guildMongoFactory) : base(database)
    {
        _discordBotClient = discordBotClient;
        _guildMongoFactory = guildMongoFactory;
    }
    
    public override string GetCollectionName()
    {
        return "Calendar";
    }
}