using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace SAH2.Web.Mail.Contracts
{
    public interface IMailTemplate : IMailProviderTemplate
    {
        MailMessage MailMessage { get; }

        MailAddress FromAddress { get; set; }

        List<MailAddress> ToAddresses { get; set; }

        bool IsBodyHtml { get; }

        string MessageBody { get; set; }

        string MessageSubject { get; set; }

        NetworkCredential Credential { get; set; }

        void CompareDomainAndFromAddress();
    }
}
