using MimeTypes;
using SharpSevenZip;

namespace DtReader.Web.Services;

public interface IImageFileService
{
    int GetPageCount(string fileName);
    Task<ImagePage> GetPage(string fileName, int pageNumber);
}

public interface IComicFileService : IImageFileService;

public record ImagePage(string MimeType, byte[] Data);

public class ComicFileService : IComicFileService
{
    public int GetPageCount(string fileName)
    {
        using var extractor =
            new SharpSevenZipExtractor(
                $".{Path.DirectorySeparatorChar}Ebooks{Path.DirectorySeparatorChar}{fileName}");
        
        return (int)extractor.FilesCount;
    }
    
    public async Task<ImagePage> GetPage(string fileName, int pageNumber)
    {
        using var extractor =
            new SharpSevenZipExtractor(
                $".{Path.DirectorySeparatorChar}Ebooks{Path.DirectorySeparatorChar}{fileName}");
        var imageFiles = extractor.ArchiveFileNames.ToArray();
        var index = imageFiles.Length <= pageNumber ? 0 : pageNumber;
        var imageFile = imageFiles[index];
        var mimeType = "image/jpeg";
        MimeTypeMap.TryGetMimeType(imageFile, out mimeType);
        using var ms = new MemoryStream();
        await extractor.ExtractFileAsync(index, ms);
        await ms.FlushAsync();
        var buffer = ms.ToArray();
        return new ImagePage(mimeType, buffer);
    }
}