using Dikubot.DataLayer.Database.Global.User.DiscordUser;
using Dikubot.DataLayer.Database.Global.GuildSettings;
using Dikubot.DataLayer.Database.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Channel.CategoryChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.TextChannel;
using Dikubot.DataLayer.Database.Guild.Models.Channel.VoiceChannel;
using Dikubot.DataLayer.Database.Guild.Models.Guild;
using Dikubot.DataLayer.Database.Guild.Models.Role;
using Dikubot.DataLayer.Database.Guild.Models.User;
using Discord.WebSocket;

namespace Dikubot.DataLayer.Permissions;

/// <Summary>Class for talking between the database and Discord.</Summary>
public partial class PermissionsService
{
    private readonly CategoryChannelMongoService _categoryChannelMongoService;
    private readonly GuildMongoService _guildMongoService;
    private readonly GuildSettingsMongoService _guildSettingsMongoService;

    private readonly RoleMongoService _roleMongoService;
    private readonly TextChannelMongoService _textChannelMongoService;
    private readonly DiscordUserGlobalMongoService _discordUserGlobalMongoService;
    private readonly UserGuildMongoService _userMongoService;
    private readonly VoiceChannelMongoService _voiceChannelMongoService;

    /// <Summary>The constructor of PermissionServices.</Summary>
    /// <param name="guild">The guild for which the PermissionService is being executed in.</param>
    private readonly SocketGuild guild; // This can not be made private.

    private IGuildMongoFactory _guildMongoFactory;

    public PermissionsService(IGuildMongoFactory guildMongoFactory, GuildSettingsMongoService guildSettingsMongoService,
        DiscordUserGlobalMongoService discordUserGlobalMongoService, SocketGuild guild)
    {
        this.guild = guild;
        _guildMongoFactory = guildMongoFactory;
        _roleMongoService = guildMongoFactory.Get<RoleMongoService>(guild);
        _voiceChannelMongoService = guildMongoFactory.Get<VoiceChannelMongoService>(guild);
        _textChannelMongoService = guildMongoFactory.Get<TextChannelMongoService>(guild);
        _categoryChannelMongoService = guildMongoFactory.Get<CategoryChannelMongoService>(guild);
        _userMongoService = guildMongoFactory.Get<UserGuildMongoService>(guild);
        _guildMongoService = guildMongoFactory.Get<GuildMongoService>(guild);
        _guildSettingsMongoService = guildSettingsMongoService;
        _discordUserGlobalMongoService = discordUserGlobalMongoService;
    }

    /// <Summary>Takes channels, roles and users information from discord and saves it in the database.</Summary>
    /// <returns>Void.</returns>
    public void SetDatabase()
    {
        SetDatabaseRoles();
        SetDatabaseUsers();
        SetDatabaseCategoryChannels();
        SetDatabaseVoiceChannels();
        SetDatabaseTextChannels();
        SetDatabaseGuild();
    }

    /// <Summary>Takes channels, roles and users information from database and saves it in the discord.</Summary>
    /// <returns>Void.</returns>
    public void SetDiscord()
    {
        SetDiscordRoles();
        SetDiscordUsers();
        SetDiscordCategoryChannels();
        SetDiscordVoiceChannels();
        SetDiscordTextChannels();
    }

    public GuildSettingsMongoService GetGuildSettingsService()
    {
        return _guildSettingsMongoService;
    }
}