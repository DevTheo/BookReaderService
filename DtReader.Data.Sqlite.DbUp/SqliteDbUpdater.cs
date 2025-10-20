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
        var upgrader =
            DeployChanges.To
                .SqliteDatabase(options.Value.ConnectionString)
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