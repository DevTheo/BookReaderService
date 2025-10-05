using System.Net;
using System.Net.Http.Headers;
using DtReader.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DtReader.Web.Site.ServiceRoutes.Handlers;

public static class BookReaderHandler
{
    public static async Task<HttpResponseMessage> ViewComicPage(
        [FromQuery]string name,
        [FromQuery]string page,
        [FromServices]IComicFileService comicService,
        IHttpContextAccessor httpContextAccessor)
    {
        var pageNum = 0;
        int.TryParse(page, out pageNum);
        
        // Call Comic Api to get page and return it
        var comicPage = await comicService.GetPage(name, pageNum);

        var response = new HttpResponseMessage(HttpStatusCode.Accepted)
        {
            Content = new ByteArrayContent(comicPage.Data)
        };
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(comicPage.MimeType);
        return response;
    }
}