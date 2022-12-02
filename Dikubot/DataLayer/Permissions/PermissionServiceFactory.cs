using System;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Dikubot.DataLayer.Permissions;

public class PermissionServiceFactory : IPermissionServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public PermissionsService Get(SocketGuild guild)
    {
        if (guild == null)
        {
            throw new ArgumentNullException(nameof(guild));
        }
        return ActivatorUtilities.CreateInstance<PermissionsService>(_serviceProvider, new object[] { guild });
    }
}