/******************************************************************************************************************
*******************************************************************************************************************
*******************************************************************************************************************
*****                                                                                                         *****
*****                                                                                                         *****
*****   Based on http://stackoverflow.com/a/11848460 and improved.                                            *****
*****                                                                                                         *****
*****                                                                                                         *****
*******************************************************************************************************************
*******************************************************************************************************************
******************************************************************************************************************/


using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Definitions;
using SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Internal;

namespace SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Basics
{
    internal static class EmbeddedResource
    {
        public static FileStreamResult Get(string assemblyName, string resourceAddress, string folders)
        {
            var resourceName = string.Empty;

            try
            {
                resourceName = Common.GetCorrectedResourceName(resourceAddress);
                var pluginAssemblyAddress = GetCorrectedFullAssemblyAddress(assemblyName, folders);
                var physicalPath = HttpContext.Current.Server.MapPath(pluginAssemblyAddress);
                var stream = ResourceHelper.GetEmbeddedResource(physicalPath, resourceName);
                return new FileStreamResult(stream, MediaTypes.GetMediaType(resourceName));
            }
            catch (Exception)
            {
                return new FileStreamResult(new MemoryStream(), MediaTypes.GetMediaType(resourceName));
            }
        }

        private static string GetCorrectedFullAssemblyAddress(string assemblyName, string assemblyFolders)
        {
            if (string.IsNullOrWhiteSpace(assemblyFolders)
                && !assemblyName.Contains($"bin/{assemblyName}")
                && !assemblyName.Contains($@"bin\{assemblyName}"))
                assemblyFolders = "bin";

            if (!assemblyName.EndsWith(".dll") && !assemblyName.EndsWith(".exe"))
                assemblyName = $"{assemblyName}.dll";

            var rV = $"{assemblyFolders.GetDecryptedFolderAddress()}/{assemblyName}";


            return rV[0] == '/'
                ? rV
                : $"/{rV}";
        }
    }

    internal static class PluginHelper
    {
        public static T LoadPluginByPathName<T>(string pathName)
        {
            var pathKey = pathName.Replace("~", "").Replace("/", "-");

            if (HttpRuntime.Cache[pathKey] != null && HttpRuntime.Cache[pathKey] is T)
                return (T) HttpRuntime.Cache[pathKey];

            object plugin = Assembly.LoadFrom(pathName);

            //Cache this object as we want to only load this assembly into memory once.
            HttpRuntime.Cache.Insert(pathKey, plugin);
            return (T) plugin;
        }
    }

    internal static class ResourceHelper
    {
        public static Stream GetEmbeddedResource(string physicalPath, string resourceName)
        {
            try
            {
                var assembly = PluginHelper.LoadPluginByPathName<Assembly>(physicalPath);

                if (assembly != null)
                {
                    var resourceList = assembly.GetManifestResourceNames().ToList();
                    var tempResourceName = resourceList.FirstOrDefault(f => f.EndsWith(resourceName)) ??
                                           resourceList.FirstOrDefault(f => f.Contains(resourceName));

                    if (tempResourceName != null)
                        return assembly.GetManifestResourceStream(tempResourceName);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }
    }
}