namespace Dikubot.Webapp.Authentication;

public static class Permissions
{
    /// <summary>
    /// GlobalAdmin will give a user permission to perform everything.
    /// </summary>
    public const string GlobalAdmin = "global_admin";
    
    /// <summary>
    /// GuildAdmin will give a user permission to perform everything on a specific guild
    /// </summary>
    public const string GuildAdmin = "guild_admin";
    
}