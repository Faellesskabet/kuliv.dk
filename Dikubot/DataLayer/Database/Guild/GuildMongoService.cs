using Discord.WebSocket;

namespace Dikubot.DataLayer.Database.Guild;

/// <summary>
///     Class for for retrieving information from a given collection.
/// </summary>
public abstract class GuildMongoService<TModel> : MongoService<TModel> where TModel : MainModel
{
    protected GuildMongoService(Database database, SocketGuild guild) : base(database,
        $"KULIV_{guild?.Id.ToString() ?? "NULL"}")
    {
        Guild = guild;
    }

    public SocketGuild Guild { get; }
}