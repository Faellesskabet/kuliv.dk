using System;
using System.Collections.Generic;
using System.Diagnostics;
using MongoDB.Driver;

namespace Dikubot.DataLayer.Database;

/// <summary>
///     Class for safely communicating with a database by using a Double-Checked locking singleton
///     pattern.
/// </summary>
public sealed class Database
{
    private const string ConnectionString = "mongodb://localhost:27017";
    private readonly MongoClient _mongoClient = new(ConnectionString);
    private readonly Dictionary<string, IMongoDatabase> Databases = new();

    /// <Summary>Method for retrieving a database from the client through the singleton pattern.</Summary>
    /// <param name="name">The name of the database.</param>
    /// <param name="settings">The database settings.</param>
    /// <return>An implementation of a database.</return>
    public Dictionary<string, IMongoDatabase> GetDatabaseDictionary()
    {
        return Databases;
    }

    /// <Summary>Method for retrieving a database from the client through the singleton pattern.</Summary>
    /// <param name="name">The name of the database.</param>
    /// <param name="settings">The database settings.</param>
    /// <return>An implementation of a database.</return>
    public IMongoDatabase GetDatabase(string name, MongoDatabaseSettings settings = null)
    {
        if (!Databases.ContainsKey(name)) Databases[name] = _mongoClient.GetDatabase(name, settings);

        return Databases[name];
    }

    // This is bad and will only work on linux, but I needed to get something out quickly
    public void Backup(IMongoDatabase database)
    {
#if DEBUG
        return;
#endif
        ProcessStartInfo procStartInfo = new ProcessStartInfo("mongodump",
            $"--out=\"/home/rofudox/Backups/{database.DatabaseNamespace.DatabaseName}/{DateTime.Now.ToUniversalTime():yyyy-MM-dd-HH-mm-ss}\" --db=\"{database.DatabaseNamespace.DatabaseName}\" --gzip");
        Process process = new Process { StartInfo = procStartInfo };
        process.Start();
    }
}