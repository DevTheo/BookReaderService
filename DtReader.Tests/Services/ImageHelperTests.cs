using DtReader.Web.Services;
using SkiaSharp;
using Xunit;

namespace DtReader.Tests.Services;

public class ImageHelperTests
{
    [Fact]
    public void Image_already_within_bounds_is_reencoded_to_target_format_unchanged()
    {
        var source = TestImages.Png(40, 30, new SKColor(255, 0, 0));

        var result = ImageHelper.GenerateThumbnail(source, maxWidth: 100, maxHeight: 100);

        Assert.Equal(SKEncodedImageFormat.Webp, result.Format);
        var thumb = AssertDecode(result.Buffer);
        Assert.Equal(40, thumb.Width);
        Assert.Equal(30, thumb.Height);
    }

    [Fact]
    public void Non_image_buffer_falls_back_to_original_bytes()
    {
        byte[] notAnImage = [1, 2, 3, 4, 5];

        var result = ImageHelper.GenerateThumbnail(notAnImage, maxWidth: 10, maxHeight: 10);

        Assert.Equal(notAnImage, result.Buffer);
        Assert.Equal(SKEncodedImageFormat.Jpeg, result.Format);
    }

    [Fact]
    public void Result_can_be_decoded_by_Skia()
    {
        var source = TestImages.Jpeg(64, 48);

        var result = ImageHelper.GenerateThumbnail(source, maxWidth: 100, maxHeight: 100);

        AssertDecode(result.Buffer);
    }

    [Fact]
    public void Large_image_is_scaled_down_within_bounds_keeping_aspect_ratio()
    {
        var source = TestImages.Png(100, 60, new SKColor(0, 128, 0));

        var result = ImageHelper.GenerateThumbnail(source, maxWidth: 50, maxHeight: 50);

        var thumb = AssertDecode(result.Buffer);
        Assert.Equal(50, thumb.Width);
        Assert.Equal(30, thumb.Height);
    }

    private static SKBitmap AssertDecode(byte[] buffer)
    {
        var bitmap = SKBitmap.Decode(buffer);
        Assert.NotNull(bitmap);
        return bitmap;
    }
}
