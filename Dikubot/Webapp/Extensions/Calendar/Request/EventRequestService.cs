using Dikubot.DataLayer.Database.Global;
using Dikubot.DataLayer.Database.Guild.Models.Calendar.Events;
using Dikubot.Webapp.Extensions.Discovery.Request;

namespace Dikubot.Webapp.Extensions.Discovery.Links
{
    public class EventRequestService : GlobalServices<RequestModel<EventModel>>
    {
        public EventRequestService() : base("EventsRequests")
        {
        }
    }
}