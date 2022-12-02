namespace Dikubot.DataLayer.Database.Global.Tags
{
    public class TagMongoService : GlobalMongoService<TagsMainModel>
    {
        public TagMongoService(Database database) : base(database)
        {
        }

        public override string GetCollectionName()
        {
            return "TagsModel";
        }
    }
}