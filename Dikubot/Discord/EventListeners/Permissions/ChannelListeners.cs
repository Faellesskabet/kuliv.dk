using System.Threading.Tasks;
using Dikubot.DataLayer.Permissions;
using Discord.WebSocket;

namespace Dikubot.Discord.EventListeners.Permissions;

public partial class PermissionListeners
{
    /// <Summary>When a channel gets created it will be added to the database.</Summary>
    /// <param name="channel">The channel that has been created.</param>
    /// <return>Task.</return>
    public async Task ChannelCreated(SocketChannel channel)
    {
        Util util = new Util();

        PermissionsService permissionsService;
        if (channel is SocketVoiceChannel) // Determines which type of channel has bee created.
        {
            SocketVoiceChannel voiceChannel = channel as SocketVoiceChannel;

            if (await util.IsBotFirstEntryInAuditLog(voiceChannel.Guild))
                return;

            permissionsService = _permissionServiceFactory.Get(voiceChannel.Guild);
            permissionsService.AddOrUpdateDatabaseVoiceChannel(voiceChannel);
        }
        else if (channel is SocketTextChannel)
        {
            SocketTextChannel textChannel = channel as SocketTextChannel;

            if (await util.IsBotFirstEntryInAuditLog(textChannel.Guild))
                return;

            permissionsService = _permissionServiceFactory.Get(textChannel.Guild);
            permissionsService.AddOrUpdateDatabaseTextChannel(textChannel);
        }
        else if (channel is SocketCategoryChannel)
        {
            SocketCategoryChannel categoryChannel = channel as SocketCategoryChannel;

            if (await util.IsBotFirstEntryInAuditLog(categoryChannel.Guild))
                return;

            permissionsService = _permissionServiceFactory.Get(categoryChannel.Guild);
            permissionsService.AddOrUpdateDatabaseCategoryChannel(categoryChannel);
        }
    }

    /// <Summary>When a channel gets destroyed it will be removed to the database.</Summary>
    /// <param name="channel">The channel which will be destroyed.</param>
    /// <return>Task.</return>
    public async Task ChannelDestroyed(SocketChannel channel)
    {
        Util util = new Util();

        PermissionsService permissionsService;
        if (channel is SocketVoiceChannel)
        {
            SocketVoiceChannel voiceChannel = channel as SocketVoiceChannel;

            if (await util.IsBotFirstEntryInAuditLog(voiceChannel.Guild))
                return;

            permissionsService = _permissionServiceFactory.Get(voiceChannel.Guild);
            permissionsService.RemoveDatabaseVoiceChannel(voiceChannel);
        }
        else if (channel is SocketTextChannel)
        {
            SocketTextChannel textChannel = channel as SocketTextChannel;

            if (await util.IsBotFirstEntryInAuditLog(textChannel.Guild))
                return;

            permissionsService = _permissionServiceFactory.Get(textChannel.Guild);
            permissionsService.RemoveDatabaseTextChannel(textChannel);
        }
        else if (channel is SocketCategoryChannel)
        {
            SocketCategoryChannel categoryChannel = channel as SocketCategoryChannel;

            if (await util.IsBotFirstEntryInAuditLog(categoryChannel.Guild))
                return;

            permissionsService = _permissionServiceFactory.Get(categoryChannel.Guild);
            permissionsService.RemoveDatabaseCategoryChannel(categoryChannel);
        }
    }

    /// <Summary>When a channel gets edited it will be updated in the database.</Summary>
    /// <param name="channelBefore">The channel before it got edited.</param>
    /// <param name="channelAfter">The channel after it got edited.</param>
    /// <return>Task.</return>
    public async Task ChannelUpdated(SocketChannel channelBefore, SocketChannel channelAfter)
    {
        Util util = new Util();

        PermissionsService permissionsService;
        if (channelAfter is SocketVoiceChannel)
        {
            SocketVoiceChannel voiceChannel = channelAfter as SocketVoiceChannel;

            if (await util.IsBotFirstEntryInAuditLog(voiceChannel.Guild))
                return;

            permissionsService = _permissionServiceFactory.Get(voiceChannel.Guild);
            permissionsService.AddOrUpdateDatabaseVoiceChannel(voiceChannel);
        }
        else if (channelAfter is SocketTextChannel)
        {
            SocketTextChannel textChannel = channelAfter as SocketTextChannel;

            if (await util.IsBotFirstEntryInAuditLog(textChannel.Guild))
                return;

            permissionsService = _permissionServiceFactory.Get(textChannel.Guild);
            permissionsService.AddOrUpdateDatabaseTextChannel(textChannel);
        }
        else if (channelAfter is SocketCategoryChannel)
        {
            SocketCategoryChannel categoryChannel = channelAfter as SocketCategoryChannel;

            if (await util.IsBotFirstEntryInAuditLog(categoryChannel.Guild))
                return;

            permissionsService = _permissionServiceFactory.Get(categoryChannel.Guild);
            permissionsService.AddOrUpdateDatabaseCategoryChannel(categoryChannel);
        }
    }
}