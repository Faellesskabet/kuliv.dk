using SendGrid.Helpers.Mail;

namespace Dikubot.DataLayer.Logic.Email.Emails
{
    public class DebugEmail : Email
    {
        private string errorMessage;
        public DebugEmail(string errorMessage)
        {
            this.errorMessage = errorMessage;
        }
        public string GetTextContent()
        {
            return errorMessage;
        }

        public string GetHtmlContent()
        {
            return errorMessage;
        }

        public EmailAddress GetTo()
        {
            return new EmailAddress("lukiwokimc@gmail.com");
        }

        public string GetSubject()
        {
            return "KUliv ERROR";
        }
    }
}