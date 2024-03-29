namespace Dikubot.DataLayer.Database.Global.Session;

public class SessionMongoService : GlobalMongoService<SessionModel>
{
    public SessionMongoService(Database database) : base(database)
    {
    }

    public override string GetCollectionName()
    {
        return "UserSessions";
    }
}