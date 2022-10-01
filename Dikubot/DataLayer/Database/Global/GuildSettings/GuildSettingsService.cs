using System.Collections.Generic;
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

    public List<GuildSettingsModel> Get()
    {
        var list = new List<GuildSettingsModel>();
        foreach (var VARIABLE in DiscordBot.ClientStatic.Guilds)
        {
            list.Add(Get(VARIABLE));
        }

        return list;
    }
    

    public IEnumerable<CreateIndexModel<GuildSettingsModel>> GetIndexes()
    {
        var options = new CreateIndexOptions() { Unique = true };
        return new List<CreateIndexModel<GuildSettingsModel>>
        {
            new CreateIndexModel<GuildSettingsModel>(Builders<GuildSettingsModel>.IndexKeys.Ascending(model => model.GuildId), options),
        };
    }
}