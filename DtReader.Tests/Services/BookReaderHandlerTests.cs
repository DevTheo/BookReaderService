using DtReader.Web.Site.ServiceRoutes.Handlers;
using DtReader.Web.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace DtReader.Tests.Services;

public class BookReaderHandlerTests
{
    [Fact]
    public async Task Unsupported_extension_throws_and_never_touches_file_services()
    {
        var comic = new FakeImageService();
        var djvu = new FakeImageService();
        var accessor = MakeAccessor();

        await Assert.ThrowsAnyAsync<ApplicationException>(
            () => BookReaderHandler.ViewPageImage(
                name: "book.pdf", page: "1",
                comicService: comic, djvuService: djvu, httpContextAccessor: accessor));
    }

    [Fact]
    public async Task Cbz_request_serves_page_from_comic_service()
    {
        var comic = new FakeImageService();
        var djvu = new FakeImageService();
        var accessor = MakeAccessor();

        await BookReaderHandler.ViewPageImage(
            name: "WHIZ Comics 001.cbz", page: "3",
            comicService: comic, djvuService: djvu, httpContextAccessor: accessor);

        Assert.True(comic.Called);
        Assert.False(djvu.Called);
        var response = accessor.HttpContext!.Response;
        Assert.Equal("image/png", response.ContentType);
        Assert.Equal(3, response.Body.Length);
    }

    [Fact]
    public async Task Djvu_request_routes_to_djvu_service()
    {
        var comic = new FakeImageService();
        var djvu = new FakeImageService();
        var accessor = MakeAccessor();

        await BookReaderHandler.ViewPageImage(
            name: "dr_dobbs_journal_vol_01.djvu", page: "0",
            comicService: comic, djvuService: djvu, httpContextAccessor: accessor);

        Assert.False(comic.Called);
        Assert.True(djvu.Called);
    }

    private static IHttpContextAccessor MakeAccessor()
    {
        var httpContext = new DefaultHttpContext { Response = { Body = new MemoryStream() } };
        return new HttpContextAccessor { HttpContext = httpContext };
    }

    private sealed class FakeImageService : IComicFileService, IDjvuFileService
    {
        public bool Called { get; private set; }

        public Task<int> GetPageCount(string fileName)
        {
            Called = true;
            return Task.FromResult(1);
        }

        public Task<ImagePage> GetPage(string fileName, int pageNumber)
        {
            Called = true;
            return Task.FromResult(new ImagePage("image/png", [1, 2, 3]));
        }
    }
}
