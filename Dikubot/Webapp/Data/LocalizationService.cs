using Data;
using Dikubot.DataLayer.Database.Global.User;

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
    private readonly UserGlobalMongoService _userGlobalMongoService;

    public LocalizationService(UserService user, NotifyStateService notifyStateService,
        UserGlobalMongoService userGlobalMongoService)
    {
        _user = user;
        _notifyStateService = notifyStateService;
        _userGlobalMongoService = userGlobalMongoService;
    }

    public void Change(Language language)
    {
        UserGlobalModel userGlobalModel = _user.GetUserGlobalModel();
        if (language == Language.Danish)
            userGlobalModel.CultureInfo = "da-DK";
        else
            userGlobalModel.CultureInfo = "en-US";


        if (_user?.GetUserGlobalModel() is not null) _userGlobalMongoService.Upsert(userGlobalModel);

        _notifyStateService.NotifyUserChange(this);
    }
}