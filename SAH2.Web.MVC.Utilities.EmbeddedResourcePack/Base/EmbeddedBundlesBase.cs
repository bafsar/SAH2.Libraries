using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SAH2.Core.Extensions;
using SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Internal;

namespace SAH2.Web.MVC.Utilities.EmbeddedResourcePack.Base
{
    public abstract class EmbeddedBundlesBase
    {
        #region Abstract Properties

        protected abstract Assembly CurrentAssembly { get; }

        #endregion


        #region Private Static Properties

        private static ExpandoObject Bundles { get; } = new ExpandoObject();

        #endregion


        #region Protected Methods

        protected void AddResources(string definer, params string[] resourceAddresses)
        {
            lock (Bundles)
            {
                // check null state check in definer 
                CheckNullReferenceState(definer, nameof(definer));

                CheckResourceAddressesState(resourceAddresses);

                // check are resource types valid with together
                CheckResourceTypesValidWithTogether(resourceAddresses);

                definer = GetFullDefinerName(definer);

                if (Bundles.GetValue(definer) != null)
                    throw new ArgumentException($"that named resource already added before => {definer}");

                // add addresses to the array
                Bundles.Add(definer, Common.GetCorrectedResourceNames(resourceAddresses));
            }
        }

        protected MvcHtmlString RenderDirectly(string assemblyName, string assemblyFolders, params string[] resourceAddresses)
        {
            CheckNullReferenceState(assemblyName, nameof(assemblyName));
            CheckResourceAddressesState(resourceAddresses);
            CheckResourceTypesValidWithTogether(resourceAddresses);

            var fileTypeExt = GetFileTypeExtension(resourceAddresses[0]);
            var tagFormat = GetTagFormat(fileTypeExt);

            if (string.IsNullOrWhiteSpace(tagFormat))
                return null;


            var sb = new StringBuilder();

            resourceAddresses = Common.GetCorrectedResourceNames(resourceAddresses);

            foreach (var address in resourceAddresses)
            {
                if (sb.Length > 0)
                    sb.Append(Environment.NewLine + "\t");

                var embeddedAddress = $"/Embedded/{assemblyName}/{address}/";

                if (!string.IsNullOrWhiteSpace(assemblyFolders) && assemblyFolders != DefaultBinFolder)
                    embeddedAddress = $"{embeddedAddress}{assemblyFolders.GetCryptedFolderAddress()}/";

                sb.Append(string.Format(tagFormat, embeddedAddress));
            }

            return new MvcHtmlString(sb.ToString());
        }


        protected MvcHtmlString RenderFromBundle(string definer)
        {
            // check null state check in definer 
            CheckNullReferenceState(definer, nameof(definer));

            var fullDefiner = GetFullDefinerName(definer);

            if (Bundles.GetValue(fullDefiner) is string[] bundle)
            {
                var fileTypeExt = GetFileTypeExtension(bundle[0]);
                var tagFormat = GetTagFormat(fileTypeExt);
                if (tagFormat != null)
                {
                    var sb = new StringBuilder();

                    foreach (var address in bundle)
                    {
                        if (sb.Length > 0)
                            sb.Append(Environment.NewLine + "\t");

                        if (Common.IsExternalResourceAddress(address))
                            sb.Append(string.Format(tagFormat, address));
                        else
                        {
                            var embeddedAddress = $"/Embedded/{AssemblyName}/{address}/";

                            if (AssemblyRelativeDirectory != DefaultBinFolder)
                                embeddedAddress =
                                    $"{embeddedAddress}{AssemblyRelativeDirectory.GetCryptedFolderAddress()}/";

                            sb.Append(string.Format(tagFormat, embeddedAddress));
                        }
                    }

                    return new MvcHtmlString(sb.ToString());
                }
            }

            return MvcHtmlString.Empty;
        }

        #endregion


        #region Private Const Members

        private const string AssemblyBundleDivider = "►";
        private const string DefaultBinFolder = "bin";

        private const string CssTagFormat = "<link rel=\"stylesheet\" href=\"{0}\" />";
        private const string JavaScriptTagFormat = "<script src=\"{0}\"></script>";
        private const string ImageTagFormat = "<img src=\"{0}\" />";

        #endregion


        #region Private Members

        private string _assemblyName;

        private string _assemblyRelativeDirectory;

        #endregion


        #region Protected Properties

        protected string AssemblyRelativeDirectory
        {
            get
            {
                if (_assemblyRelativeDirectory != null)
                    return _assemblyRelativeDirectory;

                var serverRoot = HttpContext.Current.Server.MapPath("~");
                var assemblyDirectory = CurrentAssembly.GetAssemblyDirectory();
                var relativePath = assemblyDirectory.Replace(serverRoot, "");
                return _assemblyRelativeDirectory = relativePath;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(_assemblyRelativeDirectory))
                    _assemblyRelativeDirectory = value;
            }
        }

        protected string AssemblyName
        {
            get => _assemblyName ?? (_assemblyName = CurrentAssembly.GetName().Name);
            set
            {
                if (string.IsNullOrWhiteSpace(_assemblyName))
                    _assemblyName = value;
            }
        }

        #endregion


        #region Private Methods

        private void CheckResourceAddressesState(params string[] resourceAddresses)
        {
            // check null state in resource addresses
            if (resourceAddresses.Any(string.IsNullOrWhiteSpace))
                throw new NullReferenceException(nameof(resourceAddresses));
        }

        private void CheckNullReferenceState(string obj, string nameOfObj)
        {
            if (string.IsNullOrWhiteSpace(obj))
                throw new NullReferenceException(nameOfObj);
        }

        private void CheckResourceTypesValidWithTogether(params string[] resourceAddresses)
        {
            // determine first resource address' extension
            var firstFileExt = GetFileTypeExtension(resourceAddresses[0]);
            var firstBundleType = GetBundleType(firstFileExt);

            if (firstBundleType == null)
                throw new ArgumentException("first resource type is invalid");

            for (var i = 1; i < resourceAddresses.Length; i++)
            {
                var fileExt = GetFileTypeExtension(resourceAddresses[i]);
                var bundleType = GetBundleType(fileExt);

                if (bundleType == null || bundleType != firstBundleType)
                    throw new ArgumentException("different resource types");
            }
        }

        private string GetFullDefinerName(string definer)
        {
            return $"{AssemblyName}{AssemblyBundleDivider}{definer}";
        }


        private string GetFileTypeExtension(string resourceAddress)
        {
            var lastDotIndex = resourceAddress.LastIndexOf(".", StringComparison.InvariantCulture);
            var fileExt = resourceAddress.Substring(lastDotIndex + 1);
            return fileExt;
        }

        private string GetTagFormat(string fileTypeExtension)
        {
            var bundleType = GetBundleType(fileTypeExtension);
            switch (bundleType)
            {
                case BundleTypes.JavaScript:
                    return JavaScriptTagFormat;
                case BundleTypes.StyleSheet:
                    return CssTagFormat;
                case BundleTypes.Image:
                    return ImageTagFormat;
                case null:
                    return null;
                default:
                    return null;
            }
        }

        private BundleTypes? GetBundleType(string fileTypeExtension)
        {
            switch (fileTypeExtension)
            {
                case "js":
                    return BundleTypes.JavaScript;
                case "css":
                case "less":
                    return BundleTypes.StyleSheet;
                case "jpeg":
                case "jpg":
                case "gif":
                case "png":
                case "bmp":
                case "tiff":
                case "tif":
                    return BundleTypes.Image;
                default:
                    return null;
            }
        }

        #endregion
    }
}