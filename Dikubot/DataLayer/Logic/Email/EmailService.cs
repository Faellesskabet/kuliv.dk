using System;
using System.Threading.Tasks;
using Dikubot.DataLayer.Static;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Dikubot.DataLayer.Logic.Email;

/// <summary>
///     The EmaiLService is used to send emails
/// </summary>
public class EmailService
{
    private static readonly SendGridClient client = new(Environment.GetEnvironmentVariable("SENDGRID_API_KEY"));

    private static readonly EmailAddress from = new(Environment.GetEnvironmentVariable("SENDGRID_EMAIL"), "KULiv");

    /// <summary>
    ///     Send Email
    /// </summary>
    /// <param name="email">The email to be sent</param>
    /// <returns>Task</returns>
    public static async Task SendEmail(Email email)
    {
        await client.SendEmailAsync(MailHelper.CreateSingleEmail(from, email.GetTo(), email.GetSubject(),
            email.GetTextContent(), email.GetHtmlContent()));
#if DEBUG
        Logger.Debug(email.GetTextContent());
#endif
    }
}