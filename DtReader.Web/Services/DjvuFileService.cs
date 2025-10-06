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
        var index = doc.Pages.Length < pageNumber ? doc.Pages.Length - 1 : pageNumber;
        var page = doc.Pages[index];
        //page.        
        var skBitmap = page.BuildPageImage().ToSKBitmap();

        using var ms = new MemoryStream();
        skBitmap.Encode(ms, SKEncodedImageFormat.Jpeg, 80);
        var mimeType = "image/jpeg";
        await ms.FlushAsync();
        var buffer = ms.ToArray();
        return new ImagePage(mimeType, buffer);
    }
}