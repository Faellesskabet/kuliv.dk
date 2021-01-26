using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;

namespace Dikubot.DataLayer.Logic.Email
{
    public interface Email
    {
        string GetTextContent();
        string GetHtmlContent();

        EmailAddress GetTo();

        string GetSubject();
    }
}