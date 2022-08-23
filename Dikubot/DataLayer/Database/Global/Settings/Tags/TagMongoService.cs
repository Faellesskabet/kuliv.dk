namespace Dikubot.DataLayer.Database.Global.Settings.Tags
{
    public class TagMongoService : GlobalMongoService<TagsMainModel>
    {
        public TagMongoService() : base("TagsModel")
        {
        }
    }
}