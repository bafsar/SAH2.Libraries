/***********************************************************************************************************
 ***********************************************************************************************************
 ***********************************************************************************************************
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***                                   Based on the example on                                           ***
 ***                              http://stackoverflow.com/a/353222                                      ***
 ***                                     address and improved.                                           ***
 ***                                                                                                     ***
 ***                          Also, the improved version has been added                                  ***
 ***                             http://stackoverflow.com/a/25343437                                     ***
 ***                                           address.                                                  ***
 ***                                                                                                     ***
 ***                                                                                                     ***
 ***********************************************************************************************************
 ***********************************************************************************************************
 **********************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;

namespace SAH2.Utilities
{
    /// <summary>
    ///     Provides various image untilities, such as high quality resizing and the ability to save.
    /// </summary>
    public static class ImageUtilities
    {
        /// <summary>
        ///     A quick lookup for getting image _encoders
        /// </summary>
        private static Dictionary<string, ImageCodecInfo> _encoders;

        /// <summary>
        ///     A quick lookup for getting image _encoders
        /// </summary>
        public static Dictionary<string, ImageCodecInfo> Encoders
        {
            //get accessor that creates the dictionary on demand
            get
            {
                //if the quick lookup isn't initialised, initialise it
                if (_encoders == null) _encoders = new Dictionary<string, ImageCodecInfo>();

                //if there are no codecs, try loading them
                if (_encoders.Count == 0)
                    //get all the codecs
                    foreach (var codec in ImageCodecInfo.GetImageEncoders())
                        //add each codec to the quick lookup
                        _encoders.Add(codec.MimeType.ToLower(), codec);

                //return the lookup
                return _encoders;
            }
        }

        /// <summary>
        ///     Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="size">The size to resize to.</param>
        /// <param name="drawSettings">The draw settings for image to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, Size size, DrawSettings drawSettings = null)
        {
            return ResizeImage(image, size.Width, size.Height, drawSettings);
        }

        /// <summary>
        ///     Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <param name="drawSettings">The draw settings for image to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(Image image, int width, int height, DrawSettings drawSettings = null)
        {
            //a holder for the result
            var result = new Bitmap(width, height);

            //set the resolutions the same to avoid cropping due to resolution differences
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            var ds = drawSettings ?? new DrawSettings();

            //use a graphics object to draw the resized image into the bitmap
            using (var graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = ds.CompositingQuality;
                graphics.InterpolationMode = ds.InterpolationMode;
                graphics.SmoothingMode = ds.SmoothingMode;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }

        /// <summary>
        ///     Saves an image, with the given quality
        /// </summary>
        /// <param name="path">Path to which the image would be saved.</param>
        /// <param name="image">The image to save as own format.</param>
        /// <param name="quality">An integer from 0 to 100, with 100 being the highest quality</param>
        /// <param name="imageExt">Image extension to define image type to save as</param>
        /// <exception cref="ArgumentOutOfRangeException">An invalid value was entered for image quality.</exception>
        /// <exception cref="InvalidOperationException">An invalid image format taken for image quality.</exception>
        public static void SaveImage(string path, Image image, int quality, string imageExt)
        {
            SaveImage(path, image, quality, GetImageFormat(imageExt));
        }

        /// <summary>
        ///     Saves an image, with the given quality
        /// </summary>
        /// <param name="path">Path to which the image would be saved.</param>
        /// <param name="image">The image to save as own format.</param>
        /// <param name="quality">An integer from 0 to 100, with 100 being the highest quality</param>
        /// <param name="imageFormat">Image format to save as</param>
        /// <exception cref="ArgumentOutOfRangeException">An invalid value was entered for image quality.</exception>
        /// <exception cref="InvalidOperationException">An invalid image format taken for image quality.</exception>
        public static void SaveImage(string path, Image image, int quality, ImageFormat imageFormat)
        {
            //ensure the quality is within the correct range
            if (quality < 0 || quality > 100)
            {
                var error =
                    $"Image quality must be between 0 and 100, with 100 being the highest quality.  A value of {quality} was specified.";
                //throw a helpful exception
                throw new ArgumentOutOfRangeException(error);
            }

            //create an encoder parameter for the image quality
            var qualityParam = new EncoderParameter(Encoder.Quality, quality);

            //get the image codec
            var imageCodec = GetImageCodecInfo(imageFormat);

            //create a collection of all parameters that we will pass to the encoder and set the quality parameter for the codec
            var encoderParams = new EncoderParameters(1) {Param = {[0] = qualityParam}};
            //save the image using the codec and the parameters
            image.Save(path, imageCodec, encoderParams);
        }

        /// <summary>
        ///     Returns the image codec with the given mime type
        /// </summary>
        public static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            //do a case insensitive search for the mime type
            var lookupKey = mimeType.ToLower();

            //the codec to return, default to null
            ImageCodecInfo foundCodec = null;

            //if we have the encoder, get it to return
            if (Encoders.ContainsKey(lookupKey))
                //pull the codec from the lookup
                foundCodec = Encoders[lookupKey];

            return foundCodec;
        }

        /// <summary>
        ///     Get max size with proportioned or not.
        /// </summary>
        /// <param name="image">The image to take as base for new size</param>
        /// <param name="maxWidth">Max width for new size.</param>
        /// <param name="maxHeight">Max height for new size.</param>
        /// <param name="withProportion">Is size take with proportion?</param>
        /// <returns></returns>
        public static Size GetProportionedSize(Image image, int maxWidth, int maxHeight, bool withProportion = true)
        {
            if (withProportion)
            {
                double sourceWidth = image.Width;
                double sourceHeight = image.Height;

                if (sourceWidth < maxWidth && sourceHeight < maxHeight)
                {
                    maxWidth = (int) sourceWidth;
                    maxHeight = (int) sourceHeight;
                }
                else
                {
                    var aspect = sourceHeight / sourceWidth;

                    if (sourceWidth < sourceHeight)
                        maxWidth = Convert.ToInt32(Math.Round(maxHeight / aspect, 0));
                    else
                        maxHeight = Convert.ToInt32(Math.Round(maxWidth * aspect, 0));
                }
            }

            return new Size(maxWidth, maxHeight);
        }

        /// <summary>
        ///     Gets image format according to 'imageExt'
        /// </summary>
        /// <param name="imageExt">Image Extension</param>
        /// <returns></returns>
        public static ImageFormat GetImageFormat(string imageExt)
        {
            switch (imageExt.ToLower(CultureInfo.GetCultureInfo("en-us")))
            {
                case "png":
                    return ImageFormat.Png;
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "tif":
                case "tiff":
                    return ImageFormat.Tiff;
                default:
                    return null;
            }
        }

        /// <summary>
        ///     Gets image codec info to define image type to save image as
        /// </summary>
        /// <param name="imageFormat">Image Format</param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageCodecInfo(ImageFormat imageFormat)
        {
            if (Equals(imageFormat, ImageFormat.Png)) return GetEncoderInfo("image/png");
            if (Equals(imageFormat, ImageFormat.Jpeg)) return GetEncoderInfo("image/jpeg");
            if (Equals(imageFormat, ImageFormat.Bmp)) return GetEncoderInfo("image/bmp");
            if (Equals(imageFormat, ImageFormat.Tiff)) return GetEncoderInfo("image/tiff");

            throw new InvalidOperationException(
                "Invalid format. Available format are these: png, jpg, jpeg, bmp, tif, tiff");
        }
    }

    /// <summary>
    ///     Draw Settings for image resize
    /// </summary>
    public class DrawSettings
    {
        /// <summary>
        ///     Constructs the class for using on image resize
        /// </summary>
        /// <param name="compositingQuality">Compositing Quality</param>
        /// <param name="interpolationMode">Interpolation Mode</param>
        /// <param name="smoothingMode">Smoothing Mode</param>
        public DrawSettings(CompositingQuality compositingQuality = CompositingQuality.HighQuality,
            InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic,
            SmoothingMode smoothingMode = SmoothingMode.HighQuality)
        {
            CompositingQuality = compositingQuality;
            InterpolationMode = interpolationMode;
            SmoothingMode = smoothingMode;
        }

        /// <summary>
        ///     Compositing Quality
        /// </summary>
        public CompositingQuality CompositingQuality { get; set; }

        /// <summary>
        ///     Interpolation Mode
        /// </summary>
        public InterpolationMode InterpolationMode { get; set; }

        /// <summary>
        ///     Smoothing Mode
        /// </summary>
        public SmoothingMode SmoothingMode { get; set; }
    }
}
