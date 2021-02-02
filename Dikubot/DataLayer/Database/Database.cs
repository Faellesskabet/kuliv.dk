using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dikubot.Database.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
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
        private static readonly Dictionary<string, IMongoDatabase> Databases = new Dictionary<string, IMongoDatabase>();
        public static Database GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Database();
                    
                    // This is very important for our "Unique" elements to work
                    // It ensures that unique empty elements won't collide and cause errors
                    // What is this is basically it doesn't add an element if it is equal to null
                    ConventionRegistry.Register("IgnoreIfNull", 
                        new ConventionPack { new IgnoreIfDefaultConvention(true) }, 
                        t => true);
                    
                    //This is used to tell mongodb to store our IDs as GUIDs
                    BsonSerializer.RegisterIdGenerator(
                        typeof(Guid),
                        GuidGenerator.Instance
                    );
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
            if (!Databases.ContainsKey(name))
                Databases[name] = MongoClient.GetDatabase(name, settings);
            
            return Databases[name];
        }
        
    }
}