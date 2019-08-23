namespace SAH2.Web.Mail.Providers
{
    public class GoogleMailProvider : CustomMailProvider
    {
        public GoogleMailProvider() : base("smtp.gmail.com", true, 587)
        {
        }
    }
}