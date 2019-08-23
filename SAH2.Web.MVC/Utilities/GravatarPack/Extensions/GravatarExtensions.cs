/***********************************************************************************************************
 ***********************************************************************************************************
 ***********************************************************************************************************
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***                                   Based on the codes on                                             ***
 ***                             http://stackoverflow.com/a/3561841                                      ***
 ***                                     address and improved.                                           ***
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***********************************************************************************************************
 ***********************************************************************************************************
 **********************************************************************************************************/

using System.Web.Mvc;
using System.Web.Routing;
using SAH2.Web.Utilities.GravatarPack.Enums;

namespace SAH2.Web.MVC.Utilities.GravatarPack.Extensions
{
    /// <summary>
    ///     MVC için Gravatar Extension'larını sunar.
    /// </summary>
    public static class GravatarExtensions
    {
        /// <summary>
        ///     Creates HTML for an <c>img</c> element that presents a Gravatar icon.
        /// </summary>
        /// <param name="html">The <see cref="HtmlHelper" /> upon which this extension method is provided.</param>
        /// <param name="email">The email address used to identify the icon.</param>
        /// <param name="size">An optional parameter that specifies the size of the square image in pixels.</param>
        /// <param name="rating">An optional parameter that specifies the safety level of allowed images.</param>
        /// <param name="defaultImage">
        ///     An optional parameter that controls what image is displayed for email addresses that don't
        ///     have associated Gravatar icons.
        /// </param>
        /// <param name="htmlAttributes">
        ///     An optional parameter holding additional attributes to be included on the <c>img</c>
        ///     element.
        /// </param>
        /// <returns>An HTML string of the <c>img</c> element that presents a Gravatar icon.</returns>
        public static MvcHtmlString Gravatar(this HtmlHelper html,
            string email,
            int? size = null,
            GravatarRating rating = GravatarRating.Default,
            GravatarDefaultImage defaultImage = GravatarDefaultImage.MysteryMan,
            object htmlAttributes = null)
        {
            var url = Web.Utilities.GravatarPack.Gravatar.GetImageAddress(email, size, rating, defaultImage);

            var tag = new TagBuilder("img");
            tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            tag.Attributes.Add("src", url);

            if (size != null)
            {
                tag.Attributes.Add("width", size.ToString());
                tag.Attributes.Add("height", size.ToString());
            }

            return new MvcHtmlString(tag.ToString());
        }
    }
}