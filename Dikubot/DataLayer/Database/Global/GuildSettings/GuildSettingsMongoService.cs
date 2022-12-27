using System.Collections.Generic;
using Dikubot.DataLayer.Database.Interfaces;
using Dikubot.Discord;
using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Global.GuildSettings;

public class GuildSettingsMongoService : GlobalMongoService<GuildSettingsModel>, IIndexed<GuildSettingsModel>
{
    public GuildSettingsMongoService(Database database) : base(database)
    {
    }


    public IEnumerable<CreateIndexModel<GuildSettingsModel>> GetIndexes()
    {
        CreateIndexOptions options = new CreateIndexOptions { Unique = true };
        return new List<CreateIndexModel<GuildSettingsModel>>
        {
            new(Builders<GuildSettingsModel>.IndexKeys.Ascending(model => model.GuildId), options)
        };
    }

    public GuildSettingsModel Get(SocketGuild guild)
    {
        GuildSettingsModel model = Get(model => model.GuildId == guild.Id);
        return model ?? new GuildSettingsModel(guild);
    }

    public override string GetCollectionName()
    {
        return "GuildSettings";
    }

    public List<GuildSettingsModel> Get()
    {
        List<GuildSettingsModel> list = new();
        foreach (SocketGuild socketGuild in DiscordBot.ClientStatic.Guilds) list.Add(Get(socketGuild));

        return list;
    }
}