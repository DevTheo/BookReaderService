using DtReader.Web.Services;
using SkiaSharp;
using Xunit;

namespace DtReader.Tests.Services;

/// <summary>
/// Exercises DjvuFileService against a real ~1MB djvu sample.
/// Like ComicFileService it resolves files relative to ./Ebooks, so the
/// cwd is swapped for a fixture folder (test parallelization disabled
/// assembly-wide in ComicFileServiceTests.cs).
/// </summary>
public class DjvuFileServiceTests : IDisposable
{
    private readonly string _root;
    private readonly string _originalCwd;
    private readonly DjvuFileService _sut = new();
    private readonly string _fileName = "Acromaid.djvu";

    public DjvuFileServiceTests()
    {
        _originalCwd = Environment.CurrentDirectory;
        _root = Path.Combine(Path.GetTempPath(), $"dtreader_djvu_{Guid.NewGuid():N}");
        Directory.CreateDirectory(Path.Combine(_root, "Ebooks"));
        Environment.CurrentDirectory = _root;
        File.Copy(
            Path.Combine(AppContext.BaseDirectory, _fileName),
            Path.Combine(_root, "Ebooks", _fileName));
    }

    [Fact]
    public async Task GetPageCount_is_positive()
    {
        Assert.True(await _sut.GetPageCount(_fileName) > 0);
    }

    [Fact]
    public async Task Last_page_renders_to_a_decodable_image()
    {
        var count = await _sut.GetPageCount(_fileName);

        var page = await _sut.GetPage(_fileName, pageNumber: count - 1);

        Assert.Equal("image/jpeg", page.MimeType);
        var bitmap = SKBitmap.Decode(page.Data);
        Assert.NotNull(bitmap);
        Assert.True(bitmap.Width > 0 && bitmap.Height > 0);
    }

    [Fact]
    public async Task Page_beyond_range_clamps_to_last_page()
    {
        var count = await _sut.GetPageCount(_fileName);

        var clamped = await _sut.GetPage(_fileName, pageNumber: count + 10);
        var last = await _sut.GetPage(_fileName, pageNumber: count - 1);

        Assert.Equal(last.Data, clamped.Data);
    }

    [Fact]
    public async Task Negative_page_clamps_to_first_page()
    {
        var clamped = await _sut.GetPage(_fileName, pageNumber: -1);
        var first = await _sut.GetPage(_fileName, pageNumber: 0);

        Assert.Equal(first.Data, clamped.Data);
    }

    public void Dispose()
    {
        Environment.CurrentDirectory = _originalCwd;
        try { Directory.Delete(_root, recursive: true); } catch (IOException) { /* best effort */ }
    }
}
