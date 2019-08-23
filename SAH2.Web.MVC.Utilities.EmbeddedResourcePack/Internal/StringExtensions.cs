using System.Linq;

namespace SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Internal
{
    internal static class StringExtensions
    {
        public static string SubstringInSafe(this string value, int startIndex, int length)
        {
            return new string((value ?? string.Empty).Skip(startIndex).Take(length).ToArray());
        }
    }
}