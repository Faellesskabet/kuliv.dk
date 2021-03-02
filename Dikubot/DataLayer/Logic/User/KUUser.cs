using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Dikubot.DataLayer.Logic.User
{
    public class KUUser
    {
        private string _username;
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
                return content.Substring(from, to - from).Trim();
            }
            catch (Exception e)
            {
                //ignored
            }

            return "";
        }
        
        
    }
}