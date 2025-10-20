namespace DtReader.Core;

public enum DbDataSourceType
{
    NotConfigured,
    Sqlite
}

public class DtReaderConfig
{
    public string ConnectionString { get; set; } = string.Empty;
    public DbDataSourceType DataSourceType { get; set; } = DbDataSourceType.Sqlite;
}