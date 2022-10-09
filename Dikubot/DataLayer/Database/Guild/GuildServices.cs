using Discord.WebSocket;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database.Guild
{
    /// <summary>
    /// Class for for retrieving information from a given collection.
    /// </summary>
    public abstract class GuildServices<TModel> : Services<TModel> where TModel : MainModel
    {
        public SocketGuild Guild { get; private set; }

        /// <summary>
        /// Constructor for the services.
        /// </summary>
        protected GuildServices(string collectionName, SocketGuild guild,
            MongoDatabaseSettings databaseSettings = null,
            MongoCollectionSettings collectionSettings = null) : base($"KULIV_{guild?.Id.ToString() ?? "NULL"}", collectionName,
            databaseSettings, collectionSettings)
        {
            Guild = guild;
        }
        
        
        
    }
}