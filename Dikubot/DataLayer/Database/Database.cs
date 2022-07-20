using System;
using System.Collections.Generic;
using System.Diagnostics;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database
{
    /// <summary>
    /// Class for safely communicating with a database by using a Double-Checked locking singleton
    /// pattern.
    /// </summary>
    public sealed class Database
    {
        private const string ConnectionString = "mongodb://localhost:27017";
        private static readonly MongoClient MongoClient = new MongoClient(ConnectionString);
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
                        new ConventionPack {new IgnoreIfNullConvention(true)},
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
        public static Dictionary<string, IMongoDatabase> GetDatabaseDictionary()
        {
            return Databases;
        }

        /// <Summary>Method for retrieving a database from the client through the singleton pattern.</Summary>
        /// <param name="name">The name of the database.</param>
        /// <param name="settings">The database settings.</param>
        /// <return>An implementation of a database.</return>
        public IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null)
        {
            if (!Databases.ContainsKey(name))
            {
                Databases[name] = MongoClient.GetDatabase(name, settings);
                Backup(Databases[name]);
            }

            return Databases[name];
        }
        // This is bad and will only work on linux, but I needed to get something out quickly
        public static void Backup(IMongoDatabase database)
        {
            #if DEBUG 
            return;
            #endif
            ProcessStartInfo procStartInfo = new ProcessStartInfo("mongodump",
                $"--out=\"/home/rofudox/Backups/{database.DatabaseNamespace.DatabaseName}/{DateTime.Now.ToUniversalTime():yyyy-MM-dd-HH-mm-ss}\" --db=\"{database.DatabaseNamespace.DatabaseName}\" --gzip");
            Process process = new Process() {StartInfo = procStartInfo};
            process.Start();
        }
    }
}