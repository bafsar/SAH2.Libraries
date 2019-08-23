using System;
using System.Net.Mail;
using SAH2.Web.Mail.Contracts;

namespace SAH2.Web.Mail.Providers
{
    public class CustomMailProvider : IMailProviderTemplate
    {
        private SmtpClient _SMTPClient;

        public CustomMailProvider(SmtpClient smtpClient, bool enableSSL = true, int smtpPort = 587)
        {
            _SMTPClient = smtpClient;
            SSLAbilityState = enableSSL;
            SMTPPort = smtpPort;
        }

        public CustomMailProvider(string smtpClientString, bool enableSSL = true, int smtpPort = 587)
            : this(new SmtpClient(smtpClientString), enableSSL, smtpPort)
        {
        }

        public SmtpClient SMTPClient
        {
            get
            {
                if (_SMTPClient == null)
                    throw new ArgumentException("Missing SMTP Client Address!");

                return _SMTPClient;
            }
            set { _SMTPClient = value; }
        }

        public bool SSLAbilityState { get; set; }

        public int SMTPPort { get; set; }
    }
}