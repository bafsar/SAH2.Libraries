using System;
using System.Linq;
using SAH2.Web.Mail.Base;
using SAH2.Web.Mail.Providers;

namespace SAH2.Web.Mail.Services
{
    public class GoogleMail : CustomMailBase
    {
        public GoogleMail(string fromAddress, string fromAddressPass, string fromDisplayName = null,
            string subject = null, string body = null)
            : base(new GoogleMailProvider(), fromAddress, fromAddressPass, fromDisplayName, subject, body)
        {
        }

        public override void CompareDomainAndFromAddress()
        {
            var hostParts = SMTPClient.Host.Split('.');
            var fromAddressProvider = Credential.UserName.Split('@')[1].Split('.')[0].ToLower();

            var isEqual = hostParts.Any(p => p == fromAddressProvider);

            if (!isEqual)
                throw new ArgumentException("Invalid Domain or Service Provider Error!");
        }
    }
}