namespace Dikubot.DataLayer.Database.Global.Facebook;

public class FacebookPageService : GlobalMongoService<FacebookPage>
{
    public FacebookPageService(Database database) : base(database)
    {
    }

    public override string GetCollectionName()
    {
        return "FacebookPages";
    }
}