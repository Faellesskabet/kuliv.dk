using System;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Dikubot.DataLayer.Logic.Email
{
    public class EmailService
    {
        private static readonly SendGridClient client = new SendGridClient(Environment.GetEnvironmentVariable("SENDGRID_API"));
        private static readonly EmailAddress from = new EmailAddress(Environment.GetEnvironmentVariable("SENDGRID_EMAIL"), "Discord Bot'");

        public async Task SendEmail(Email email)
        {
            await client.SendEmailAsync(MailHelper.CreateSingleEmail(from, email.GetTo(), email.GetSubject(),
                email.GetTextContent(), email.GetHtmlContent()));
        }
    }
}