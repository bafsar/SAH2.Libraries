using System.Net.Mail;

namespace SAH2.Web.Mail.Contracts
{
    public interface IMailProviderTemplate
    {
        SmtpClient SMTPClient { get; }

        bool SSLAbilityState { get; }

        int SMTPPort { get; }
    }
}