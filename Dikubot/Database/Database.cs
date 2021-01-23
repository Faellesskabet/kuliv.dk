using MongoDB.Driver;

namespace Dikubot.Database
{
    /// <summary>
    /// Class for safely communicating with a database by using a Double-Checked locking singleton
    /// pattern.
    /// </summary>
    public sealed class Database
    {
        private static readonly MongoClient MongoClient = new MongoClient("mongodb://localhost:27017");
        private static Database instance = null;
        public static Database GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Database();
                }
                return instance;
            }
        }

        /// <Summary>Method for retrieving a database from the client through the singleton pattern.</Summary>
        /// <param name="name">The name of the database.</param>
        /// <param name="settings">The database settings.</param>
        /// <return>An implementation of a database.</return>
        public IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null)
        {
            return MongoClient.GetDatabase(name, settings);
        }
    }
}