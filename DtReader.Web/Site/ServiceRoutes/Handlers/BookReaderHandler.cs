using System.Net;
using System.Net.Http.Headers;
using DtReader.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace DtReader.Web.Site.ServiceRoutes.Handlers;

public static class BookReaderHandler
{
    public static async Task ViewPageImage(
        [FromQuery]string name,
        [FromQuery]string page,
        [FromServices]IComicFileService comicService,
        [FromServices]IHttpContextAccessor httpContextAccessor)
    {
        var pageNum = 0;
        int.TryParse(page, out pageNum);

        ImagePage comicPage;
        var extension = Path.GetExtension(name);
        if (extension.StartsWith(".cb", StringComparison.InvariantCultureIgnoreCase))
        {
            // Call Comic Api to get page and return it
            comicPage = await comicService.GetPage(name, pageNum);
        }
        else if (extension.Equals(".djvu", StringComparison.InvariantCultureIgnoreCase))
        {
            
        }
        else 
        {
            throw new ApplicationException("Invalid image type");
        }
 
        using var ms = new MemoryStream(comicPage.Data);
        httpContextAccessor!.HttpContext!.Response.ContentType = comicPage.MimeType;
        httpContextAccessor.HttpContext!.Response.ContentLength = comicPage.Data.Length;
        await ms.CopyToAsync(httpContextAccessor.HttpContext!.Response.Body);
    }
}