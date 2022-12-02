using Dikubot.Webapp.Extensions.Discovery.Links;

namespace Dikubot.DataLayer.Database.Global.Union
{
    public class UnionMongoService : GlobalMongoService<UnionModel>
    {
        public UnionMongoService(Database database) : base(database)
        {
        }

        public override string GetCollectionName()
        {
            return "Links";
        }
    }
}
