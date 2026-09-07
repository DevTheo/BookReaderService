using System.IO;
namespace DtReader.Core;

public enum DbDataSourceType
{
    NotConfigured,
    Sqlite
}

public class DtReaderConfig
{
    /// <summary>
    /// Location of the SQLite file. Relative paths are resolved against the
    /// app content root at startup (see Program.cs); never against the
    /// process working directory.
    /// </summary>
    public string SqliteDbPath { get; set; } = Path.Combine("App_Data", "DtReader.db");

    public DbDataSourceType DataSourceType { get; set; } = DbDataSourceType.Sqlite;

    /// <summary>Single source of truth for every SQLite consumer.</summary>
    public string ConnectionString => $"Data Source={SqliteDbPath}";
}