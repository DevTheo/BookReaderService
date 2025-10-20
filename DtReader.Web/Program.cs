
using System.Reflection;
using DtReader.Core;
using DtReader.Core.Data;
using DtReader.Core.Services;
using DtReader.Data.Sqlite;
using DtReader.Data.Sqlite.DbUp;
using DtReader.Web.Services;
using DtReader.Web.Site.Html;
using DtReader.Web.Site.ServiceRoutes;

void Setup7ZipLib()
{
    var fullPathToDll = Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar)).TrimEnd(Path.DirectorySeparatorChar);

    SharpSevenZip.SharpSevenZipBase.SetLibraryPath($@"{fullPathToDll}{Path.DirectorySeparatorChar}7z.dll");
}

void SetupServices(IServiceCollection services, DtReaderConfig options)
{
    services
        .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
        .AddSingleton<IComicFileService, ComicFileService>()
        .AddSingleton<IDjvuFileService, DjvuFileService>()
        // Temporary logging solution
        .AddSingleton<IMessageLogger, DebugLogger>()
        ;
    if (options.DataSourceType == DbDataSourceType.Sqlite)
    {
        services
            .AddScoped<ISqliteConnectionProvider, SqliteConnectionProvider>()
            .AddScoped<ISqliteConnectionProvider, SqliteConnectionProvider>()
            .AddSingleton<IDbUpdater, SqliteDbUpdater>()
            ;
    }
}


Setup7ZipLib();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var options = new DtReaderConfig();
builder.Configuration.Bind(nameof(DtReaderConfig), options);
builder.Services.Configure<DtReaderConfig>(
    builder.Configuration.GetSection(
        key: nameof(DtReaderConfig)));

SetupServices(builder.Services, options);

var app = builder.Build();

var dbUp = app.Services.GetService<IDbUpdater>();
dbUp?.UpdateDb();

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