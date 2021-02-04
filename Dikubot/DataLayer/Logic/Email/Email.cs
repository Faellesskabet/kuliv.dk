using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;

namespace Dikubot.DataLayer.Logic.Email
{
    public interface Email
    {
        /// <summary>
        /// The content of the email in pure text format
        /// </summary>
        /// <returns>string</returns>
        string GetTextContent();

        /// <summary>
        /// The content of the email, in text format, but with HTML tags to spice the email up
        /// </summary>
        /// <returns>string</returns>
        string GetHtmlContent();

        /// <summary>
        /// The target email address
        /// </summary>
        /// <returns>EmailAddress</returns>
        EmailAddress GetTo();

        /// <summary>
        /// The subject / title of the email.
        /// </summary>
        /// <returns>string</returns>
        string GetSubject();
    }
}