using Dikubot.DataLayer.Database;
using Dikubot.DataLayer.Database.Global;
using Dikubot.DataLayer.Database.Global.Event;
using Dikubot.DataLayer.Database.Global.Union.Request;

namespace Dikubot.Webapp.Extensions.Discovery.Links;

public class EventRequestMongoService : GlobalMongoService<RequestModel<EventModel>>
{
    public EventRequestMongoService(Database database) : base(database)
    {
    }

    public override string GetCollectionName()
    {
        return "EventsRequests";
    }
}