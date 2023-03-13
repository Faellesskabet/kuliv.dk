using Data;
using Dikubot.DataLayer.Database.Global.User.DiscordUser;

namespace Dikubot.Webapp.Data;

public class LocalizationService
{
    public enum Language
    {
        English,
        Danish
    }

    private readonly NotifyStateService _notifyStateService;
    private readonly UserService _user;
    private readonly DiscordUserGlobalMongoService _discordUserGlobalMongoService;

    public LocalizationService(UserService user, NotifyStateService notifyStateService,
        DiscordUserGlobalMongoService discordUserGlobalMongoService)
    {
        _user = user;
        _notifyStateService = notifyStateService;
        _discordUserGlobalMongoService = discordUserGlobalMongoService;
    }

    public void Change(Language language)
    {
        DiscordUserGlobalModel discordUserGlobalModel = _user.GetUserGlobalModel();
        if (language == Language.Danish)
            discordUserGlobalModel.CultureInfo = "da-DK";
        else
            discordUserGlobalModel.CultureInfo = "en-US";


        if (_user?.GetUserGlobalModel() is not null) _discordUserGlobalMongoService.Upsert(discordUserGlobalModel);

        _notifyStateService.NotifyUserChange(this);
    }
}