using System.Collections.Generic;
using System.Linq;
using Dikubot.DataLayer.Database.Global;
using Dikubot.DataLayer.Database.Interfaces;
using Dikubot.Discord;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Global.GuildSettings;

public class GuildSettingsService : GlobalServices<GuildSettingsModel>, IIndexed<GuildSettingsModel>
{
    public GuildSettingsService() : base("GuildSettings") { }

    public GuildSettingsModel Get(SocketGuild guild)
    {
        GuildSettingsModel model = this.Get(model => model.GuildId == guild.Id);
        return model ?? new GuildSettingsModel(guild);
    }

    public GuildSettingsModel Get(string guildId) =>
        Get(DiscordBot.Client.Guilds?.FirstOrDefault(g => g.Id.ToString().Equals(guildId)));


    public IEnumerable<IndexKeysDefinition<GuildSettingsModel>> GetIndexes()
    {
        return new List<IndexKeysDefinition<GuildSettingsModel>>
        {
            Builders<GuildSettingsModel>.IndexKeys.Ascending(model => model.GuildId),
        };
    }
}