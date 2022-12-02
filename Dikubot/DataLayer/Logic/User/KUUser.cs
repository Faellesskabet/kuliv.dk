using System;
using System.Net.Http;

namespace Dikubot.DataLayer.Logic.User
{
    public class KUUser
    {
        private readonly string _username;
        public KUUser(string username)
        {
            _username = username;
        }

        private readonly string KU_SCHEDULE_LINK = "https://personligtskema.ku.dk/ical.asp?id=";
        public string GetName()
        {
            try
            {
                HttpClient client = new HttpClient();
                string content = client.GetStringAsync(KU_SCHEDULE_LINK + _username).Result;
                content = content.Substring(content.IndexOf("X-WR-CALNAME:") + "X-WR-CALNAME:".Length);
                int from = content.IndexOf("-") + "-".Length;
                int to = content.IndexOf("BEGIN:");
                string name = content.Substring(from, to - from).Trim();

                return NameSanityCheck(name) ? name : "";

            }
            catch (Exception e)
            {
                //ignored
            }

            return "";
        }

        
        /// <summary>
        /// We simply check whether or not the name is somewhat reasonable
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Reeturns true if it passes the sanity check</returns>
        private bool NameSanityCheck(string name)
        {
            /*
                Discord enforces the following restrictions for usernames and nicknames:

                Names can contain most valid unicode characters. We limit some zero-width and non-rendering characters.
                Usernames must be between 2 and 32 characters long.
                Nicknames must be between 1 and 32 characters long.
                Names are sanitized and trimmed of leading, trailing, and excessive internal whitespace.

                The following restrictions are additionally enforced for usernames:

                Names cannot contain the following substrings: '@', '#', ':', '```'.
                Names cannot be: 'discordtag', 'everyone', 'here'.
            */
            
            //KU usernames are 6 characters, so we won't allow names under that
            if (name.Length < 6 || name.Length > 32)
            {
                return false;
            }

            if (name.Contains("@") || name.Contains("#") || name.Contains(":") || name.Contains("```"))
            {
                return false;
            }

            return name != "discordtag" && name != "everyone" && name != "here";
        }
        
        
    }
}