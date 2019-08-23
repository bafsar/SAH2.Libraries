namespace SAH2.Web.Mail.Providers
{
    public class YandexMailProvider : CustomMailProvider
    {
        public YandexMailProvider() : base("smtp.yandex.com", true, 587)
        {
        }
    }
}