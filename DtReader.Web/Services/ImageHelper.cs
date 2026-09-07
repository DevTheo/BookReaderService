using System.Drawing;
using SkiaSharp;

namespace DtReader.Web.Services;

public record ThumbnailResult(byte[] Buffer, SKEncodedImageFormat Format); 

public static class ImageHelper
{
    /// <summary>
    /// Generates a thumbnail from an image. 
    /// Accepts image as JPG / PNG formats and returns memory stream of thumbnail
    /// It maintains aspect ratio of the image while generating thumbnail
    /// </summary>
    /// <param name="imageStream">Image file as stream</param>
    /// <param name="width">Width in pixels for thumbnail</param>
    /// <param name="height">Height in pixels for thumbnail</param>
    /// <returns></returns>
    public static ThumbnailResult GenerateThumbnail(byte[] jpegBuffer, int maxWidth, int maxHeight, SKEncodedImageFormat targetFormat = SKEncodedImageFormat.Webp)
    {
        // Load the source image from the input stream
        using var loadedImage = SKImage.FromEncodedData(jpegBuffer);
        if (loadedImage == null)
        {
            return new ThumbnailResult(jpegBuffer, SKEncodedImageFormat.Jpeg);
        }
        using var image = SKBitmap.FromImage(loadedImage);
        if (image == null)
        {
            return new ThumbnailResult(jpegBuffer, SKEncodedImageFormat.Jpeg);
        }

        // Fit within (maxWidth, maxHeight), preserving aspect ratio; never upscale.
        var scale = Math.Min(
            Math.Min((double)maxWidth / image.Width, (double)maxHeight / image.Height),
            1.0);
        var width = (int)Math.Floor(image.Width * scale + .5);
        var height = (int)Math.Floor(image.Height * scale + .5);

        using var target = new SKBitmap(width, height, image.ColorType, image.AlphaType);

        if (image.ScalePixels(target, new SKSamplingOptions(SKCubicResampler.CatmullRom)))
        {
            using var ms = new MemoryStream();
            target.Encode(ms, targetFormat, 100);
            return new ThumbnailResult(ms.GetBuffer(), targetFormat);
        }

        return new ThumbnailResult(jpegBuffer, SKEncodedImageFormat.Jpeg);
    }
}