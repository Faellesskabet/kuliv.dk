using MongoDB.Driver;
using MongoDB.Bson;

namespace Dikubot.Database
{
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

        public IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null)
        {
            return MongoClient.GetDatabase(name, settings);
        }
    }
}