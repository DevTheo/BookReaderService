using MimeTypes;
using SharpSevenZip;

namespace DtReader.Web.Services;

public interface IComicFileService
{
    Task<ComicPage> GetPage(string fileName, int pageNumber);
}

public record ComicPage(string MimeType, byte[] Data);

public class ComicFileService : IComicFileService
{
    public async Task<ComicPage> GetPage(string fileName, int pageNumber)
    {
        var extractor =
            new SharpSevenZipExtractor(
                $"..{Path.DirectorySeparatorChar}Ebooks{Path.DirectorySeparatorChar}{fileName}");
        var imageFiles = extractor.ArchiveFileNames.ToArray();
        var index = imageFiles.Length <= pageNumber ? 0 : pageNumber;
        var imageFile = imageFiles[index];
        var mimeType = "image/jpeg";
        MimeTypeMap.TryGetMimeType(imageFile, out mimeType);
        using var ms = new MemoryStream();
        await extractor.ExtractFileAsync(index, ms);
        await ms.FlushAsync();
        var buffer = ms.ToArray();
        return new ComicPage(mimeType, buffer);
    }
}