using System.Drawing.Imaging;
using DjvuNet.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace DtReader.Web.Services;

public interface IDjvuFileService : IImageFileService;

public class DjvuFileService : IDjvuFileService
{
    public Task<int> GetPageCount(string fileName)
    {
        var fullPath = $".{Path.DirectorySeparatorChar}Ebooks{Path.DirectorySeparatorChar}{fileName}";
        var doc = new DjvuNet.DjvuDocument(fullPath);
        return Task.FromResult(doc.Pages.Length);
    }

    public async Task<ImagePage> GetPage(string fileName, int pageNumber)
    {
        var fullPath = $".{Path.DirectorySeparatorChar}Ebooks{Path.DirectorySeparatorChar}{fileName}";
        var doc = new DjvuNet.DjvuDocument(fullPath);
        if (doc.Pages.Length == 0)
        {
            throw new InvalidOperationException($"Document '{fileName}' contains no pages.");
        }
        var index = Math.Clamp(pageNumber, 0, doc.Pages.Length - 1);
        var page = doc.Pages[index];
        var skBitmap = page.BuildPageImage().ToSKBitmap();

        using var ms = new MemoryStream();
        skBitmap.Encode(ms, SKEncodedImageFormat.Jpeg, 80);
        var mimeType = "image/jpeg";
        await ms.FlushAsync();
        var buffer = ms.ToArray();
        return new ImagePage(mimeType, buffer);
    }
}