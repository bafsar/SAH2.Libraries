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
#if (NET472 || NET48)
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using SAH2.Web.Utilities.GravatarPack.Enums;

namespace SAH2.Web.Utilities.GravatarPack
{
    public static class Gravatar
    {
        /// <summary>
        ///    Gets image url that depends on the given e-mail address.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="size"></param>
        /// <param name="rating"></param>
        /// <param name="defaultImage"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static string GetImageAddress(string email, int? size = null,
            GravatarRating rating = GravatarRating.Default,
            GravatarDefaultImage defaultImage = GravatarDefaultImage.MysteryMan)
        {
            var url = new StringBuilder("//www.gravatar.com/avatar/", 90);
            url.Append(GetEmailHash(email));

            var isFirst = true;
            Action<string, string> addParam = (p, v) =>
            {
                url.Append(isFirst ? '?' : '&');
                isFirst = false;
                url.Append(p);
                url.Append('=');
                url.Append(v);
            };

            if (size != null)
            {
                if (size < 1 || size > 512)
                    throw new ArgumentOutOfRangeException("size", size,
                        "Must be null or between 1 and 512, inclusive.");
                addParam("s", size.Value.ToString());
            }

            if (rating != GravatarRating.Default)
                addParam("r", rating.ToString().ToLower());

            if (defaultImage != GravatarDefaultImage.Default)
                if (defaultImage == GravatarDefaultImage.Http404)
                    addParam("d", "404");
                else if (defaultImage == GravatarDefaultImage.Identicon)
                    addParam("d", "identicon");
                else if (defaultImage == GravatarDefaultImage.MonsterId)
                    addParam("d", "monsterid");
                else if (defaultImage == GravatarDefaultImage.MysteryMan)
                    addParam("d", "mm");
                else if (defaultImage == GravatarDefaultImage.Wavatar)
                    addParam("d", "wavatar");

            return url.ToString();
        }

        /// <summary>
        ///    Gets image that depends on the given e-mail address.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="size"></param>
        /// <param name="rating"></param>
        /// <param name="defaultImage"></param>
        /// <returns></returns>
        public static Image GetImage(string email, int? size = null, GravatarRating rating = GravatarRating.Default,
            GravatarDefaultImage defaultImage = GravatarDefaultImage.MysteryMan)
        {
            try
            {
                // When protocol doesn't specificated, WebRequest.Create throws an exception.
                var imageUrl = $"http:{GetImageAddress(email, size, rating, defaultImage)}";
                var request = WebRequest.Create(imageUrl);

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        return stream != null
                            ? Image.FromStream(stream)
                            : null;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///    Gets image as base64 formatted that depends on the given e-mail address.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="size"></param>
        /// <param name="rating"></param>
        /// <param name="defaultImage"></param>
        /// <returns></returns>
        public static string GetImageAsBase64(string email, int? size = null,
            GravatarRating rating = GravatarRating.Default,
            GravatarDefaultImage defaultImage = GravatarDefaultImage.MysteryMan)
        {
            try
            {
                var image = GetImage(email, size, rating, defaultImage);
                return
                    $"data:image/jpeg;base64,{Convert.ToBase64String(ConvertToByte(image, ImageFormat.Jpeg))}";
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Gets e-mail as hashed string.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static string GetEmailHash(string email)
        {
            if (email == null)
                return new string('0', 32);

            email = email.Trim().ToLower();

            var emailBytes = Encoding.ASCII.GetBytes(email);
            var hashBytes = new MD5CryptoServiceProvider().ComputeHash(emailBytes);

            Debug.Assert(hashBytes.Length == 16);

            var hash = new StringBuilder();
            foreach (var b in hashBytes)
                hash.Append(b.ToString("x2"));
            return hash.ToString();
        }

        /// <summary>
        ///    Converts the <see cref="Image"/> to an <see cref="byte" /> array and returns it.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageFormat"></param>
        /// <returns></returns>
        internal static byte[] ConvertToByte(Image image, ImageFormat imageFormat)
        {
            using (var stream = new MemoryStream())
            {
                if (imageFormat == null)
                    imageFormat = ImageFormat.Jpeg;

                image.Save(stream, imageFormat);
                return stream.ToArray();
            }
        }
    }
}
#endif