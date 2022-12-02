using System;
using System.Collections.Generic;
using Dikubot.DataLayer.Database.Guild.Models.Guild;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Dikubot.DataLayer.Database.Guild;

public class GuildMongoFactory : IGuildMongoFactory
{
    private readonly Dictionary<Type, Dictionary<SocketGuild, object>> _guildMongoServices =
        new Dictionary<Type, Dictionary<SocketGuild, object>>();

    private Database _database;
    private readonly IServiceProvider _serviceProvider;
    
    public GuildMongoFactory(IServiceProvider serviceProvider, Database database)
    {
        _serviceProvider = serviceProvider;
        _database = database;
    }

    public T Get<T>(SocketGuild guild)
    {
        if (guild == null)
        {
            throw new ArgumentNullException(nameof(guild));
        }
        
        if (!_guildMongoServices.ContainsKey(typeof(T)))
        {
            _guildMongoServices[typeof(T)] = new Dictionary<SocketGuild, object>();
        }

        if (!_guildMongoServices[typeof(T)].ContainsKey(guild))
        {
            /*
             * Using our service provider here allows us to solve our service's dependencies if they have any.
             */
            _guildMongoServices[typeof(T)][guild] =
                ActivatorUtilities.CreateInstance<T>(_serviceProvider, new object[] { guild });
        }

        return (T) _guildMongoServices[typeof(T)][guild];
    }
    
    
    
}