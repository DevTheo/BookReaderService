using Microsoft.AspNetCore.Mvc;

namespace DtReader.Web.Site.ServiceRoutes.Handlers;

public static class BookReaderHandler
{
    public static IResult ViewComicPage(
        [FromQuery]string name,
        [FromQuery]string page)
    {
        var pageNum = 0;
        int.TryParse(page, out pageNum);
        
        // Call Comic Api to get page and return it
    }
}