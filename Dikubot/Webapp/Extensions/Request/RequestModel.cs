using Dikubot.DataLayer.Database;
using MongoDB.Bson.Serialization.Attributes;

namespace Dikubot.Webapp.Extensions.Discovery.Request
{
    public class RequestModel<Tmodel> : MainModel where Tmodel : MainModel, new()
    {
        /// <summary>
        /// Contact info on the one who made the request 
        /// </summary>
        public ContactModel Contact { get; set; } = new ContactModel();

        /// <summary>
    /// The Requset Item, 
    /// </summary>
    public Tmodel RequestItem { get; set; } = new Tmodel();

        /// <summary>
        /// The Requestet item status 
        /// </summary>
        public StatusEmun Status { get; set; } = StatusEmun.ongoing;

        public enum StatusEmun
        {
            ongoing,
            change,
            denied,
            approved
        }


    }
}