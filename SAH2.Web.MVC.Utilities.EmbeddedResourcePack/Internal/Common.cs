using System;
using System.Linq;
using System.Text;

namespace SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Internal
{
    internal static class Common
    {
        public const string Slash = "/";
        public const string SlashPlaceholder = "!";

        public static string GetCryptedFolderAddress(this string folders)
        {
            return folders.Replace(Slash, SlashPlaceholder)
                .Replace("\\", SlashPlaceholder)
                .Replace(@"\", SlashPlaceholder);
        }

        public static string GetDecryptedFolderAddress(this string folders)
        {
            return folders.Replace(SlashPlaceholder, Slash);
        }


        public static bool IsExternalResourceAddress(string resourceAddress)
        {
            if (resourceAddress.StartsWith("//", StringComparison.InvariantCulture) ||
                resourceAddress.StartsWith("http:", StringComparison.InvariantCulture) ||
                resourceAddress.StartsWith("https:", StringComparison.InvariantCulture) ||
                resourceAddress.StartsWith("ftp:", StringComparison.InvariantCulture) ||
                resourceAddress.StartsWith("ftps:", StringComparison.InvariantCulture) ||
                resourceAddress.StartsWith("sftp:", StringComparison.InvariantCulture))
                return true;

            return false;
        }

        public static string GetCorrectedResourceName(string resourceAddress)
        {
            resourceAddress = resourceAddress.Trim();

            if (IsExternalResourceAddress(resourceAddress))
                return resourceAddress;

            var parts = resourceAddress.Split('/');
            var fileName = parts.Last();

            var sb = new StringBuilder();
            for (var i = 0; i < parts.Length - 1; i++)
            {
                if (i < parts.Length - 1)
                    parts[i] = parts[i].Replace("-", "_");

                sb.Append(parts[i]);

                if (i < parts.Length - 2)
                    sb.Append(".");
            }

            resourceAddress = sb.ToString();

            sb.Clear();
            parts = resourceAddress.Split('.');

            for (var i = 0; i < parts.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(parts[i]))
                    continue;

                var p = parts[i];

                if (int.TryParse(p[0].ToString(), out _))
                    sb.Append("_");

                sb.Append(parts[i]);

                if (i < parts.Length - 1)
                    sb.Append(".");
            }

            var foldersName = sb.ToString();

            var divider = string.IsNullOrWhiteSpace(foldersName) ? string.Empty : ".";

            var correctedFullResourceName = $"{sb}{divider}{fileName}".Trim();

            return correctedFullResourceName;
        }

        public static string[] GetCorrectedResourceNames(params string[] resourceRelativeAddresses)
        {
            for (var i = 0; i < resourceRelativeAddresses.Length; i++)
            {
                resourceRelativeAddresses[i] = GetCorrectedResourceName(resourceRelativeAddresses[i]);
            }

            return resourceRelativeAddresses;
        }
    }
}