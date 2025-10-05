
using System.Reflection;
using DtReader.Web.Services;
using DtReader.Web.Site.Html;
using DtReader.Web.Site.ServiceRoutes;

void Setup7ZipLib()
{
    var fullPathToDll = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar)).TrimEnd(Path.DirectorySeparatorChar);

    SharpSevenZip.SharpSevenZipBase.SetLibraryPath($@"{fullPathToDll}{Path.DirectorySeparatorChar}7z.dll");
}

void SetupServices(IServiceCollection services)
{
    services.AddSingleton<IComicFileService, ComicFileService>();
}


Setup7ZipLib();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

SetupServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.AddServiceRoutes();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();