using DtReader.Web.Site.Html.Widgets.Widgets.HelloVisitor;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DtReader.Web.Site.ServiceRoutes.Handlers;

public static class DemoHandler
{
    public static RazorComponentResult<HelloVisitorChildContent> SetNameAsync(string visitorName)
    {
        return new RazorComponentResult<HelloVisitorChildContent>(
        new {
            VisitorName = visitorName
        });
    }

}