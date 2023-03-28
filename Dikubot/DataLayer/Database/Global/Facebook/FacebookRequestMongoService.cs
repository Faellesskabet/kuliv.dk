using Dikubot.DataLayer.Database.Global.Request;

namespace Dikubot.DataLayer.Database.Global.Facebook;

public class FacebookRequestMongoService: GlobalMongoService<RequestModel<FacebookPageModel>>
{
    public FacebookRequestMongoService(Database database) : base(database)
    {
    }

    public override string GetCollectionName()
    {
        return "FacebookPageRequests";
    }
}

