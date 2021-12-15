using System;

namespace Dikubot.Discord.Interactive
{
    public class InteractiveOptions
    {
        public static InteractiveOptions Default = new InteractiveOptions();
        
        public TimeSpan? Timeout = null;
        
    }
}