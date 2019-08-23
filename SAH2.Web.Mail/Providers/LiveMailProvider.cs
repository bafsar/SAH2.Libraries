namespace SAH2.Web.Mail.Providers
{
    public class LiveMailProvider : CustomMailProvider
    {
        public LiveMailProvider() : base("smtp.live.com", true, 587)
        {
        }
    }
}