using System;
using System.IO;
using System.Net;

namespace Dikubot.DataLayer.Logic.User
{
    public class KUUser
    {
        private string _username;
        public KUUser(string username)
        {
            _username = username;
            fetchData();
        }

        private readonly string KU_SCHEDULE_LINK = "https://personligtskema.ku.dk/ical.asp?objectclass=student&id=";
        private void fetchData()
        {
            WebRequest request = WebRequest.Create(KU_SCHEDULE_LINK + _username);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse();
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                string data = reader.ReadToEnd();
                Console.WriteLine(data);
            }
        }
        
        
    }
}