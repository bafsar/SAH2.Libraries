using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Internal;

namespace SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Base
{
    public abstract class CustomEmbeddedBundle<TDerivedObject> : EmbeddedBundlesBase
        where TDerivedObject : CustomEmbeddedBundle<TDerivedObject>
    {
        #region Fields

        private static TDerivedObject _current;

        #endregion

        #region Properties

        protected override Assembly CurrentAssembly => Assembly.GetAssembly(Current.GetType());

        protected static TDerivedObject Current
            => _current ??
               (_current = (TDerivedObject)Activator.CreateInstance(typeof(TDerivedObject)));

        #endregion

        #region Methods

        /// <summary>
        ///     Adds resource files definer to the list.
        /// </summary>
        /// <param name="definer"></param>
        /// <param name="resourceAddresses">Resource files addresses. It may more than one.</param>
        protected static void Add(string definer, params string[] resourceAddresses)
        {
            Current.AddResources($"{Current.GetType().Name}_{definer}", resourceAddresses);
        }


        /// <summary>
        ///   Gets the resource file.
        /// </summary>
        /// <param name="definer"></param>
        /// <returns></returns>
        protected static MvcHtmlString Render([CallerMemberName] string definer = null)
        {
            if (string.IsNullOrWhiteSpace(definer))
                throw new ArgumentNullException(nameof(definer));

            return Current.RenderFromBundle($"{Current.GetType().Name}_{definer}");
        }

        protected static MvcHtmlString AutoRender(params string[] resourceAddresses)
        {
            if (!resourceAddresses.Any() || string.IsNullOrWhiteSpace(resourceAddresses.First()))
                throw new ArgumentNullException(nameof(resourceAddresses));

            var definer = string.Join("|", resourceAddresses).SubstringInSafe(0, 2000);

            var value = Render(definer);

            if (value != MvcHtmlString.Empty)
                return value;

            Add(definer, resourceAddresses);
            return Render(definer);
        }



        #endregion
    }
}