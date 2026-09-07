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
        [FromServices]IDjvuFileService djvuService,
        [FromServices]IHttpContextAccessor httpContextAccessor)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            name.Contains('/') || name.Contains('\\') || name.Contains(".."))
        {
            throw new ArgumentException("Invalid file name", nameof(name));
        }

        var pageNum = 0;
        int.TryParse(page, out pageNum);

        var extension = Path.GetExtension(name).ToLowerInvariant();
        IImageFileService? svc = extension switch
        {
            _ when extension.StartsWith(".cb") => comicService,
            _ when extension is ".djvu" or ".djv" => djvuService,
            _ => null
        };

        if (svc != null)
        {
            var docPageAsImage = await svc.GetPage(name, pageNum);
            using var ms = new MemoryStream(docPageAsImage.Data);
            httpContextAccessor!.HttpContext!.Response.ContentType = docPageAsImage.MimeType;
            httpContextAccessor.HttpContext!.Response.ContentLength = docPageAsImage.Data.Length;
            await ms.CopyToAsync(httpContextAccessor.HttpContext!.Response.Body);
            return;
        }
        
        throw new ApplicationException("Invalid image type");
    }
}