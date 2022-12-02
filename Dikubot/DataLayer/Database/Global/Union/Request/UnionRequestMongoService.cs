using Dikubot.Webapp.Extensions.Discovery.Links;
using Dikubot.Webapp.Extensions.Discovery.Request;

namespace Dikubot.DataLayer.Database.Global.Union.Request
{
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
}