using System.Globalization;

namespace SAH2.Web.Extensions
{
    public static class HtmlInputExtensions
    {
        public static string AdaptDoubleToHtmlNumberInput(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture).AdaptDoubleToHtmlNumberInput();
        }

        public static string AdaptDoubleToHtmlNumberInput(this string value)
        {
            return value.Replace(',', '.');
        }
    }
}
