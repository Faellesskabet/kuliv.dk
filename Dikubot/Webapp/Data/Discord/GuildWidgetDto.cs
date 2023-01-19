using Dikubot.DataLayer.Database.Global.GuildSettings;

namespace Dikubot.Webapp.Data.Discord;

public class GuildWidgetDto
{
    public GuildWidgetDto(GuildSettingsModel guildSettingsModel, bool isUserMember)
    {
        GuildSettingsModel = guildSettingsModel;
        IsUserMember = isUserMember;
    }

    public GuildSettingsModel GuildSettingsModel { get; }

    public bool IsUserMember { get; }
}