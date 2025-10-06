namespace DtReader.Web.Site.Html.Data;

public record NavigationPage(
    string PageName,
    string RelUrl,
    bool IsDefault)
{
    public bool IsCurrent { get; set; }

    public string Css => IsCurrent ? "nav-item active" : "nav-item";
}

public static class NavigationData
{
    public static NavigationPage[] Pages { get; } =
    [
        new("Home", "/", true),
        new("Simple Htmx", "/simple", false),
        new("Cbz Viewer", "/ComicViewer", false),
        new("Djvu Viewer", "/DjvuViewer", false)
    ];
}