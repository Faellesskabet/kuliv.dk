using System.Text.RegularExpressions;

namespace Dikubot.DataLayer.Static
{
    public class Util
    {
        
        /// <summary>
        /// Tells is whether an email is a valid email
        /// Email address: RFC 2822 Format
        /// Matches a normal email address. Does not check the top-level domain.
        /// </summary>
        /// <param name="email">string</param>
        /// <returns>bool</returns>
        public static bool isEmail(string email)
        {
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public static bool IsKUEmail(string email)
        {
            email = email.ToLower();
            return isEmail(email) && (email.EndsWith(".ku.dk") || email.EndsWith("@ku.dk"));
        }
    }
}