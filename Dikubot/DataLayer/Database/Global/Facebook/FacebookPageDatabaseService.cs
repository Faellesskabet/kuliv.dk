using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace Dikubot.DataLayer.Database.Global.Facebook;

public class FacebookPageService : GlobalMongoService<FacebookPageModel>
{
    public FacebookPageService(Database database) : base(database)
    {
    }

    public override string GetCollectionName()
    {
        return "FacebookPages";
    }

    
}