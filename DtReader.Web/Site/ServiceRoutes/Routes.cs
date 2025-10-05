using DtReader.Web.Site.ServiceRoutes.Handlers;

namespace DtReader.Web.Site.ServiceRoutes;

public static class Routes
{
    public static WebApplication AddServiceRoutes(this WebApplication app)
    {
        app.MapGet("/api/set-name", DemoHandler.SetNameAsync);
        app.MapGet("/api/image-viewer", BookReaderHandler.ViewPageImage);
        return app;
    }
}