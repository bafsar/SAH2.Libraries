using System;
using System.Reflection;
using System.Web.Mvc;
using SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Base;

namespace SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Basics
{
    public class EmbeddedBundles : EmbeddedBundlesBase
    {
        private static EmbeddedBundles _current;

        protected override Assembly CurrentAssembly
        {
            get { throw new InvalidOperationException("invalid assembly name for embedded bundles"); }
        }

        private static EmbeddedBundles Current
        {
            get { return _current ?? (_current = new EmbeddedBundles()); }
        }


        /// <summary>
        ///    Returns embedded files inside from the <see cref="Assembly" />.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="assemblyFolders"></param>
        /// <param name="resourceAddresses"></param>
        /// <returns></returns>
        public static MvcHtmlString Render(string assemblyName, string assemblyFolders, params string[] resourceAddresses)
        {
            return Current.RenderDirectly(assemblyName, assemblyFolders, resourceAddresses);
        }
    }
}