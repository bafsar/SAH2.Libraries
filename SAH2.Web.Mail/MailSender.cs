using System;
using System.Collections.Generic;
using System.Net.Mail;
using SAH2.Web.Mail.Contracts;

namespace SAH2.Web.Mail
{
    public class MailSender
    {
        public MailSender(IMailTemplate template, string subject = null, string body = null,
            params MailAddress[] toAddresses)
            : this(template, new List<MailAddress>(), subject, body)
        {
            MailTemplate.ToAddresses.AddRange(toAddresses);
        }

        public MailSender(IMailTemplate template, MailAddress toAddress, string subject = null, string body = null)
            : this(template, new List<MailAddress>(), subject, body)
        {
            MailTemplate.ToAddresses.Add(toAddress);
        }

        public MailSender(IMailTemplate template, List<MailAddress> toAddresses, string subject = null,
            string body = null)
        {
            MailTemplate = template;
            MailTemplate.ToAddresses = toAddresses;
            if (!string.IsNullOrEmpty(subject)) MailTemplate.MessageSubject = subject;
            if (!string.IsNullOrEmpty(body)) MailTemplate.MessageBody = body;
        }

        private IMailTemplate MailTemplate { get; }


        public void SendMail(bool sendWithBcc = true)
        {
            try
            {
                using (var msg = MailTemplate.MailMessage)
                {
                    msg.From = MailTemplate.FromAddress;
                    foreach (var i in MailTemplate.ToAddresses)
                        if (sendWithBcc)
                            msg.Bcc.Add(i);
                        else
                            msg.To.Add(i);

                    msg.IsBodyHtml = MailTemplate.IsBodyHtml;
                    msg.Subject = MailTemplate.MessageSubject;
                    msg.Body = MailTemplate.MessageBody;
                    using (var smtp = MailTemplate.SMTPClient)
                    {
                        smtp.Port = MailTemplate.SMTPPort;
                        smtp.Credentials = MailTemplate.Credential;
                        smtp.EnableSsl = MailTemplate.SSLAbilityState;

                        MailTemplate.CompareDomainAndFromAddress();
                        smtp.Send(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}