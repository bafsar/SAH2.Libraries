using System;
using System.IO;
using System.Reflection;

namespace SAH2.Core.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Gets the folder name that the <see cref="Assembly"/> inside in.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetAssemblyDirectory(this Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
