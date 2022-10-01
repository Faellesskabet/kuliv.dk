using Dikubot.DataLayer.Database.Global;
using Dikubot.Webapp.Extensions.Discovery.Request;

namespace Dikubot.Webapp.Extensions.Discovery.Links
{
    public class UnionRequestService : GlobalServices<RequestModel<UnionModel>>
    {
        public UnionRequestService() : base("LinksRequests")
        {
        }
    }
}