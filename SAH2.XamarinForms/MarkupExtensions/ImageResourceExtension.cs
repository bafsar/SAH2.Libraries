/***********************************************************************************************************
 ***********************************************************************************************************
 ***********************************************************************************************************
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***    Example Usage:                                                                                   ***
 ***                                                                                                     ***
 ***    {ext:ImageResource Namespace.Folder.ImageName.ImgExt,                                            ***
 ***                        ContainerAssemblyRelatedType={x:Type ContainerAssemblyXmlns:AnyClass}}       ***
 ***                                                                                                     ***
 ***    Note: Any image resource to embed should be set as Embedded Resource                             ***
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***********************************************************************************************************
 ***********************************************************************************************************
 **********************************************************************************************************/

using System;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SAH2.XamarinForms.MarkupExtensions
{
    /// <summary>
    /// Gets <see cref="ImageSource"/> from Assembly according to given address.
    /// </summary>
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public Type ContainerAssemblyRelatedType { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null || ContainerAssemblyRelatedType == null)
            {
                return null;
            }

            var containerAssembly = Assembly.GetAssembly(ContainerAssemblyRelatedType);
            var imageSource = ImageSource.FromResource(Source, containerAssembly);

            return imageSource;
        }
    }
}
