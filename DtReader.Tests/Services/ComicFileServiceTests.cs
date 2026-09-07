using DtReader.Web.Services;
using SkiaSharp;
using Xunit;

// ComicFileService resolves ebooks relative to the process working directory
// (./Ebooks/<file>), so these tests swap the cwd for a fixture folder.
// Test classes must not run in parallel.

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace DtReader.Tests.Services;

public class ComicFileServiceTests : IDisposable
{
    private readonly string _root;
    private readonly string _originalCwd;
    private readonly ComicFileService _sut = new();

    public ComicFileServiceTests()
    {
        SharpSevenZip.SharpSevenZipBase.SetLibraryPath(
            Path.Combine(AppContext.BaseDirectory, "7z.dll"));

        _originalCwd = Environment.CurrentDirectory;
        _root = Path.Combine(Path.GetTempPath(), $"dtreader_cbz_{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(_root, "Ebooks"));
        Environment.CurrentDirectory = _root;

        // Two distinct pages so page identity is observable.
        // A .cbz is a zip of images. Includes the ComicRack-standard metadata
        // entry and a folder entry: neither is a page.
        using var ms = new MemoryStream();
        using (var zip = new System.IO.Compression.ZipArchive(ms, System.IO.Compression.ZipArchiveMode.Create, leaveOpen: true))
        {
            void Add(string name, byte[]? data = null)
            {
                var entry = zip.CreateEntry(name);
                if (data == null) return;
                using var stream = entry.Open();
                stream.Write(data);
            }
            Add("ComicInfo.xml", "<ComicInfo />"u8.ToArray());
            Add("pages/");
            Add("pages/page-1.png", TestImages.Png(8, 6, new SKColor(255, 0, 0)));
            Add("pages/page-2.png", TestImages.Png(6, 8, new SKColor(0, 0, 255)));
        }
        File.WriteAllBytes(Path.Combine(_root, "Ebooks", "sample.cbz"), ms.ToArray());
    }

    [Fact]
    public async Task GetPageCount_counts_only_image_pages()
    {
        Assert.Equal(2, await _sut.GetPageCount("sample.cbz"));
    }

    [Fact]
    public async Task GetPage_returns_decodable_image_with_mime_type()
    {
        var page = await _sut.GetPage("sample.cbz", pageNumber: 0);

        Assert.Equal("image/png", page.MimeType);
        var bitmap = SKBitmap.Decode(page.Data);
        Assert.NotNull(bitmap);
        Assert.Equal(8, bitmap.Width);
        Assert.Equal(6, bitmap.Height);
    }

    [Fact]
    public async Task Pages_are_distinct()
    {
        var page1 = await _sut.GetPage("sample.cbz", pageNumber: 0);
        var page2 = await _sut.GetPage("sample.cbz", pageNumber: 1);

        Assert.NotEqual(page1.Data, page2.Data);
    }


    [Fact]
    public async Task Last_page_is_extractable()
    {
        var page = await _sut.GetPage("sample.cbz", pageNumber: 1);

        var bitmap = SKBitmap.Decode(page.Data);
        Assert.NotNull(bitmap);
        Assert.Equal(6, bitmap.Width);
        Assert.Equal(8, bitmap.Height);
    }

    [Fact]
    public async Task Page_beyond_range_clamps_to_last_page()
    {
        var clamped = await _sut.GetPage("sample.cbz", pageNumber: 99);
        var last = await _sut.GetPage("sample.cbz", pageNumber: 1);

        Assert.Equal(last.Data, clamped.Data);
    }

    [Fact]
    public async Task Negative_page_clamps_to_first_page()
    {
        var clamped = await _sut.GetPage("sample.cbz", pageNumber: -5);
        var first = await _sut.GetPage("sample.cbz", pageNumber: 0);

        Assert.Equal(first.Data, clamped.Data);
    }
    public void Dispose()
    {
        Environment.CurrentDirectory = _originalCwd;
        try { Directory.Delete(_root, recursive: true); } catch (IOException) { /* best effort */ }
    }

}
