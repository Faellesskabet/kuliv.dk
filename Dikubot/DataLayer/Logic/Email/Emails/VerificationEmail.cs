using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;

namespace Dikubot.DataLayer.Logic.Email.Emails
{
    public class VerificationEmail : Email
    {
        private EmailAddress _to;
        private string _password;
        public VerificationEmail(EmailAddress to, string password)
        {
            _to = to;
            _password = password;
        }

        public EmailAddress GetTo()
        {
            return _to;
        }

        public string GetTextContent()
        {
            return $"Her er din kode til DIKU's Discord's hjemmeside: {_password}";
        }

        public string GetHtmlContent()
        {
            return $"Her er din kode til DIKU's Discord's hjemmeside: <strong>{_password}</strong>";
        }

        public string GetSubject()
        {
            return "Discord Verification";
        }
    }
}