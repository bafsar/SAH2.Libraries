using SAH2.Web.Mail.Base;
using SAH2.Web.Mail.Contracts;

namespace SAH2.Web.Mail.Services
{
    public class CustomMail : CustomMailBase
    {
        public CustomMail(IMailProviderTemplate provider, string fromAddress, string fromAddressPass,
            string fromDisplayName = null, string subject = null, string body = null)
            : base(provider, fromAddress, fromAddressPass, fromDisplayName, subject, body)
        {
        }
    }
}