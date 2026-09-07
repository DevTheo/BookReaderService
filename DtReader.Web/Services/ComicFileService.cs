using MimeTypes;
using SharpSevenZip;

namespace DtReader.Web.Services;

public interface IImageFileService
{
/// <summary>
/// Page numbers are 0-based. Out-of-range requests clamp to the nearest page
/// (viewers may pass stale counts); an empty document throws.
/// </summary>
    Task<int> GetPageCount(string fileName);
    Task<ImagePage> GetPage(string fileName, int pageNumber);
}

public interface IComicFileService : IImageFileService;

public record ImagePage(string MimeType, byte[] Data);

public class ComicFileService : IComicFileService
{
    public Task<int> GetPageCount(string fileName)
    {
        using var extractor = new SharpSevenZipExtractor(ComicPath(fileName));
        return Task.FromResult(GetImageEntries(extractor).Count);
    }

    public async Task<ImagePage> GetPage(string fileName, int pageNumber)
    {
        using var extractor = new SharpSevenZipExtractor(ComicPath(fileName));
        var imageFiles = GetImageEntries(extractor);
        if (imageFiles.Count == 0)
        {
            throw new InvalidOperationException($"Archive '{fileName}' contains no image pages.");
        }

        var (index, imageFile) = imageFiles[Math.Clamp(pageNumber, 0, imageFiles.Count - 1)];
        if (!MimeTypeMap.TryGetMimeType(imageFile, out var mimeType))
        {
            mimeType = "image/jpeg";
        }
        using var ms = new MemoryStream();
        await extractor.ExtractFileAsync(index, ms);
        await ms.FlushAsync();
        return new ImagePage(mimeType, ms.ToArray());
    }

    private static string ComicPath(string fileName) =>
        $".{Path.DirectorySeparatorChar}Ebooks{Path.DirectorySeparatorChar}{fileName}";

    /// <summary>
    /// Image entries only (skips ComicInfo.xml, folder entries, etc.), keeping
    /// the archive index needed by ExtractFileAsync.
    /// </summary>
    private static List<(int Index, string FileName)> GetImageEntries(SharpSevenZipExtractor extractor) =>
        extractor.ArchiveFileData
            .Where(f => !f.IsDirectory &&
                        MimeTypeMap.TryGetMimeType(f.FileName, out var mime) &&
                        mime.StartsWith("image/", StringComparison.Ordinal))
            .Select(f => (f.Index, f.FileName))
            .ToList();
}