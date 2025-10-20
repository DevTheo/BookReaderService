using System;
using System.Threading.Tasks;
using DtReader.Core;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace DtReader.Data.Sqlite;

public interface ISqliteConnectionProvider
{
    SqliteConnection Connection { get; }
}

public class SqliteConnectionProvider
    : ISqliteConnectionProvider, IDisposable, IAsyncDisposable
{
    public SqliteConnectionProvider(IOptions<DtReaderConfig> options)
    {
        SQLitePCL.Batteries.Init();
        Connection = new(options.Value.ConnectionString);
    }

    public SqliteConnection Connection { get; }

    public void Dispose()
    {
        Connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Connection.DisposeAsync();
    }
}