using SendGrid.Helpers.Mail;

namespace Dikubot.DataLayer.Logic.Email.Emails
{
    /// <summary>
    /// The VerificationEmail is used to send out passwords, that can be used to verify whether a person has access to the given email address
    /// </summary>
    public class VerificationEmail : Email
    {
        private EmailAddress _to;
        private string _password;

        /// <summary>
        /// Create a VerificationEmail used to send emails
        /// </summary>
        /// <param name="to">The email address that will receive the VerificationEmail</param>
        /// <param name="password">The password that will be shown in the email</param>
        public VerificationEmail(EmailAddress to, string password)
        {
            _to = to;
            _password = password;
        }

        /// <summary>
        /// The target email address
        /// </summary>
        /// <returns>EmailAddress</returns>
        public EmailAddress GetTo()
        {
            return _to;
        }

        /// <summary>
        /// The content of the email in pure text format
        /// </summary>
        /// <returns>string</returns>
        public string GetTextContent()
        {
            return $"Hej {_to.Email}! Velkommen til kuliv.\n" +
                   $"Her er koden, som du skal bruge til at bekræfte din email adresse: {_password}\n" +
                   $"\n" +
                   $"Vi glæder os til at se dig i en af vores mange fællesskaber\n" +
                   $"\n" +
                   $"Venlig hilsen,\n" +
                   "Holdet bag https://kuliv.dk/";
        }

        /// <summary>
        /// The content of the email, in text format, but with HTML tags to spice the email up
        /// </summary>
        /// <returns>string</returns>
        public string GetHtmlContent()
        {
            return $"<p>Hej {_to.Email}! Velkommen til kuliv.dk</p>" +
                   $"<p>Her er koden, som du skal bruge til at bekræfte din email adresse: <strong>{_password}</strong></p>" +
                   $"<p>Vi glæder os til at se dig i en af vores mange fællesskaber</p>" +
                   $"</br>" +
                   $"<p>Venlig hilsen,</p>" +
                   "<p><em>Holdet bag <a href='https://kuliv.dk/'>https://kuliv.dk/</a></em></p>";
        }

        /// <summary>
        /// The subject / title of the email.
        /// </summary>
        /// <returns>string</returns>
        public string GetSubject()
        {
            return "Bekræft email";
        }
    }
}