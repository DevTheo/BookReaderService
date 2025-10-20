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

public class SqliteConnectionProvider(IOptions<DtReaderConfig> options)
    : ISqliteConnectionProvider, IDisposable, IAsyncDisposable
{
    public SqliteConnection Connection { get; } = new(options.Value.ConnectionString);

    public void Dispose()
    {
        Connection.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await Connection.DisposeAsync();
    }
}