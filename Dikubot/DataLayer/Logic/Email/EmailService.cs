using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Dikubot.DataLayer.Logic.Email
{
    /// <summary>
    /// The EmaiLService is used to send emails
    /// </summary>
    public class EmailService
    {
        private static readonly SendGridClient client = new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API"));
        private static readonly EmailAddress from = new EmailAddress(Environment.GetEnvironmentVariable("SENDGRID_EMAIL"), "Discord Bot'");

        /// <summary>
        /// Send Email
        /// </summary>
        /// <param name="email">The email to be sent</param>
        /// <returns>Task</returns>
        public async Task SendEmail(Email email)
        {
            await client.SendEmailAsync(MailHelper.CreateSingleEmail(from, email.GetTo(), email.GetSubject(),
                email.GetTextContent(), email.GetHtmlContent()));
        }
    }
}