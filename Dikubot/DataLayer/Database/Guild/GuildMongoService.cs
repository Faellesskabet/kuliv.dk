using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild
{
    /// <summary>
    /// Class for for retrieving information from a given collection.
    /// </summary>
    public abstract class GuildMongoService<TModel> : MongoService<TModel> where TModel : MainModel
    {
        public SocketGuild Guild { get; private set; }

        /// <summary>
        /// Constructor for the services.
        /// </summary>
        protected GuildMongoService(DatabaseService databaseService, string collectionName, SocketGuild guild,
            MongoDatabaseSettings databaseSettings = null,
            MongoCollectionSettings collectionSettings = null) : base(databaseService,$"KULIV_{guild?.Id.ToString() ?? "NULL"}", collectionName,
            databaseSettings, collectionSettings)
        {
            Guild = guild;
        }
    }
}