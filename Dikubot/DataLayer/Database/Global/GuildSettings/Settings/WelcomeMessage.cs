using System;

namespace Dikubot.DataLayer.Database.Global.GuildSettings.Settings;

public class WelcomeMessage
{
    /// <summary>
    /// What the welcome message contains. This is what is sent out to new users when they join the guild
    /// </summary>
    public string Message { get; set; }
    
    /// <summary>
    /// When was the welcome message created
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// When was the welcome message last updated
    /// </summary>
    public DateTime LastUpdated { get; set; }
    
    /// <summary>
    /// The GUID is the GUID of a GlobalUserModel
    /// </summary>
    public Guid LastUpdatedByUser { get; set; }
}