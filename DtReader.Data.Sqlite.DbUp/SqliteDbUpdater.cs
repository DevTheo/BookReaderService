using System.Reflection;
using DbUp;
using DtReader.Core;
using DtReader.Core.Data;
using DtReader.Core.Services;
using Microsoft.Extensions.Options;

namespace DtReader.Data.Sqlite.DbUp;

public class SqliteDbUpdater(
    IMessageLogger logger,
    IOptions<DtReaderConfig> options
) : IDbUpdater
{
    public bool UpdateDb()
    {
        //2025-10-19 20:21:15 -04:00 [ERR] Upgrade failed due to an
        //unexpected exception: System.Exception:
        //You need to call SQLitePCL.raw.SetProvider().
        //If you are using a bundle package, this is done by calling
        //SQLitePCL.Batteries.Init().

        var connectionString = options.Value.ConnectionString;
        //EnsureDatabase.For.SqliteDatabase(connectionString);
        SQLitePCL.Batteries.Init();
        var upgrader =
            DeployChanges.To
                .SqliteDatabase(connectionString)
                .LogToConsole()
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        { 
            logger.LogError("Unable to upgrade db", result.Error);
            return false;
        }

        logger.LogInfo("Success!");
        return true;
    }

}