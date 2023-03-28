using Dikubot.DataLayer.Database.Global.Request;

namespace Dikubot.DataLayer.Database.Global.Union;

public class UnionRequestMongoService : GlobalMongoService<RequestModel<UnionModel>>
{
    public UnionRequestMongoService(Database database) : base(database)
    {
    }

    public override string GetCollectionName()
    {
        return "LinkRequests";
    }
}