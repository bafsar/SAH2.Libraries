using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using SAH2.Web.Mail.Contracts;

namespace SAH2.Web.Mail.Base
{
    public abstract class CustomMailBase : IMailTemplate
    {
        protected CustomMailBase(IMailProviderTemplate provider, string fromAddress, string fromAddressPass,
            string fromDisplayName = null, string subject = null, string body = null)
        {
            SMTPClient = provider.SMTPClient;
            SSLAbilityState = provider.SSLAbilityState;
            SMTPPort = provider.SMTPPort;

            Credential = new NetworkCredential(fromAddress, fromAddressPass);
            FromAddress = new MailAddress(fromAddress, fromDisplayName);
            if (!string.IsNullOrEmpty(subject)) MessageSubject = subject;
            if (!string.IsNullOrEmpty(body)) MessageBody = body;
        }

        public MailMessage MailMessage => new MailMessage();

        public bool IsBodyHtml => true;

        public string MessageBody { get; set; }

        public string MessageSubject { get; set; }

        public NetworkCredential Credential { get; set; }

        public MailAddress FromAddress { get; set; }

        public List<MailAddress> ToAddresses { get; set; }

        public SmtpClient SMTPClient { get; }

        public bool SSLAbilityState { get; }

        public int SMTPPort { get; }

        public virtual void CompareDomainAndFromAddress()
        {
        }
    }
}