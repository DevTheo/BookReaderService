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
        using var image = SKBitmap.FromImage(loadedImage);
        if (image == null)
        {
            return new ThumbnailResult(jpegBuffer, SKEncodedImageFormat.Jpeg);
        }
        
        var width = image.Width;
        var height = image.Height;
        if (width > maxWidth)
        {
            height = (int)Math.Floor((height * ((double)width / maxWidth)) + .5);
            width = maxWidth;
        }
        if (height > maxHeight)
        {
            width = (int)Math.Floor((width * ((double)height / maxHeight)) + .5);
            height = maxHeight;
        }

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