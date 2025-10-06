using MimeTypes;
using SharpSevenZip;

namespace DtReader.Web.Services;

public interface IImageFileService
{
    Task<int> GetPageCount(string fileName);
    Task<ImagePage> GetPage(string fileName, int pageNumber);
}

public interface IComicFileService : IImageFileService;

public record ImagePage(string MimeType, byte[] Data);

public class ComicFileService : IComicFileService
{
    public Task<int> GetPageCount(string fileName)
    {
        using var extractor =
            new SharpSevenZipExtractor(
                $".{Path.DirectorySeparatorChar}Ebooks{Path.DirectorySeparatorChar}{fileName}");
        
        return Task.FromResult((int)extractor.FilesCount);
    }
    
    public async Task<ImagePage> GetPage(string fileName, int pageNumber)
    {
        using var extractor =
            new SharpSevenZipExtractor(
                $".{Path.DirectorySeparatorChar}Ebooks{Path.DirectorySeparatorChar}{fileName}");
        var imageFiles = extractor.ArchiveFileNames.ToArray();
        var index = imageFiles.Length < pageNumber ? imageFiles.Length-1 : pageNumber;
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