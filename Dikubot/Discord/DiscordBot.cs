using System;
using System.Linq;
using System.Threading.Tasks;
using Dikubot.DataLayer.Cronjob;
using Dikubot.DataLayer.Cronjob.Cronjobs;
using Dikubot.Discord.EventListeners;
using Dikubot.Discord.EventListeners.Permissions;
using Discord;
using Discord.WebSocket;

namespace Dikubot.Discord;

public class DiscordBot
{
    /// <summary>
    ///     DO NOT USE THIS!!! DO ABSOLUTELY NOT USE THIS
    /// </summary>
    [Obsolete] public static DiscordSocketClient ClientStatic;

    private readonly DiscordSocketClient _discordSocketClient;
    private readonly ExpandableVoiceChatListener _expandableVoiceChatListener;
    private readonly GreetingListener _greetingListener;
    private readonly GuildDownloadListeners _guildDownloadListeners;
    private readonly InteractionHandler _interactionHandler;
    private readonly MessageListener _messageListener;
    private readonly PermissionListeners _permissionListeners;

    public DiscordBot(DiscordSocketClient discordSocketClient, InteractionHandler interactionHandler,
        PermissionListeners permissionListeners,
        CacheNewsMessagesTask cacheNewsMessagesTask,
        ExpandableVoiceChatListener expandableVoiceChatListener,
        GuildDownloadListeners guildDownloadListeners,
        MessageListener messageListener,
        GreetingListener greetingListener,
        Scheduler scheduler,
        BackupDatabaseTask backupDatabaseTask,
        ForceNameChangeTask forceNameChangeTask,
        UpdateVerifiedTask updateVerifiedTask)
    {
        _discordSocketClient = discordSocketClient;
        _interactionHandler = interactionHandler;
        _expandableVoiceChatListener = expandableVoiceChatListener;
        _guildDownloadListeners = guildDownloadListeners;
        _messageListener = messageListener;
        _greetingListener = greetingListener;
        _permissionListeners = permissionListeners;
    }

    private Task Log(LogMessage msg)
    {
#if DEBUG
        Console.WriteLine(msg.ToString());
#endif
        return Task.CompletedTask;
    }

    private async Task Ready()
    {
        await _guildDownloadListeners.DownloadGuildOnBoot(this);

        // minus 1 so it doesn't include itself (also too lazy to care about dupes)
        int users = _discordSocketClient.Guilds.Sum(guild => guild.MemberCount - 1);
        await _discordSocketClient.SetGameAsync($"{users.ToString()} users", null, ActivityType.Watching);
    }

    public async Task Run()
    {
#if DEBUG
        _discordSocketClient.Log += Log;
#endif

        ClientStatic = _discordSocketClient;

        _discordSocketClient.RoleCreated += _permissionListeners.RoleCreated;
        _discordSocketClient.RoleDeleted += _permissionListeners.RoleDeleted;
        _discordSocketClient.RoleUpdated += _permissionListeners.RoleUpdated;
        _discordSocketClient.ChannelCreated += _permissionListeners.ChannelCreated;
        _discordSocketClient.ChannelDestroyed += _permissionListeners.ChannelDestroyed;
        _discordSocketClient.ChannelUpdated += _permissionListeners.ChannelUpdated;
        _discordSocketClient.UserVoiceStateUpdated += _expandableVoiceChatListener.VoiceChannelExpand;
        _discordSocketClient.UserJoined += _permissionListeners.UserJoined;
        _discordSocketClient.UserLeft += _permissionListeners.UserLeft;
        _discordSocketClient.GuildMemberUpdated += _permissionListeners.UserUpdated;
        _discordSocketClient.Ready += Ready;
        _discordSocketClient.JoinedGuild += _guildDownloadListeners.DownloadGuildOnJoin;
        _discordSocketClient.LeftGuild += _guildDownloadListeners.DropGuildOnLeave;
        _discordSocketClient.GuildUpdated += _guildDownloadListeners.UpdateGuildOnChange;

        _discordSocketClient.MessageReceived += _messageListener.OnMessageReceived;
        _discordSocketClient.MessageDeleted += _messageListener.OnMessageRemoved;
        _discordSocketClient.UserJoined += _greetingListener.UserJoined;

        await _discordSocketClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"));
        await _discordSocketClient.StartAsync();

        await _interactionHandler.InitializeAsync();
        // Keeps the thread running
        await Task.Delay(-1);
    }
}