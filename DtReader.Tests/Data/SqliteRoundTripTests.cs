using DtReader.Core;
using DtReader.Data.Sqlite;
using DtReader.Data.Sqlite.DbUp;
using DtReader.Data.Sqlite.Repositories;
using DtReader.Core.Services;
using Xunit;
namespace DtReader.Tests.Data;

/// <summary>
/// End-to-end test of the SQLite data layer: runs the real DbUp migration
/// (001-Initial-tables.sql) against a temp file database, then reads it back
/// through DbRepository/Dapper.
/// </summary>
public class SqliteRoundTripTests : IDisposable
{
    private readonly string _dbPath;
    private readonly DtReader.Core.DtReaderConfig _config;
    private readonly SqliteConnectionProvider _provider;
    private readonly DbRepository _repository;

    public SqliteRoundTripTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"dtreader_test_{Guid.NewGuid():N}.db");
        _config = new DtReader.Core.DtReaderConfig
        {
            DataSourceType = DbDataSourceType.Sqlite,
            SqliteDbPath = _dbPath,
        };
        var updater = new SqliteDbUpdater(new RecordingLogger(), Microsoft.Extensions.Options.Options.Create(_config));
        Assert.True(updater.UpdateDb(), "DbUp migration failed");

        _provider = new SqliteConnectionProvider(Microsoft.Extensions.Options.Options.Create(_config));
        _repository = new DbRepository(_provider);
        _provider.Connection.Open();
    }

    [Fact]
    public async Task Empty_database_returns_empty_list()
    {
        var books = await _repository.GetBookInfo();
        Assert.Empty(books);
    }

    [Fact]
    public async Task Inserted_rows_are_mapped_to_BookInfo()
    {
        var id = Guid.NewGuid();
        using (var cmd = _provider.Connection.CreateCommand())
        {
            cmd.CommandText =
                "INSERT INTO BookInfo (Id, Name, BookFilePath, ThumbnailFileName, Author, SeriesIdentifier, Info) " +
                "VALUES (@Id, @Name, @BookFilePath, @ThumbnailFileName, @Author, @SeriesIdentifier, @Info)";
            cmd.Parameters.AddWithValue("@Id", id.ToString());
            cmd.Parameters.AddWithValue("@Name", "Sherlock Holmes");
            cmd.Parameters.AddWithValue("@BookFilePath", "Ebooks/sherlock.epub");
            cmd.Parameters.AddWithValue("@ThumbnailFileName", "thumbs/sherlock.webp");
            cmd.Parameters.AddWithValue("@Author", "Arthur Conan Doyle");
            cmd.Parameters.AddWithValue("@SeriesIdentifier", DBNull.Value);
            cmd.Parameters.AddWithValue("@Info", DBNull.Value);
            Assert.Equal(1, cmd.ExecuteNonQuery());
        }

        var books = await _repository.GetBookInfo();

        var book = Assert.Single(books);
        Assert.Equal(id, book.Id);
        Assert.Equal("Sherlock Holmes", book.Name);
        Assert.Equal("Ebooks/sherlock.epub", book.BookFilePath);
        Assert.Equal("thumbs/sherlock.webp", book.ThumbnailFileName);
        Assert.Equal("Arthur Conan Doyle", book.Author);
        Assert.Null(book.SeriesIdentifier);
        Assert.Null(book.Info);
    }

    [Fact]
    public void Migration_is_idempotent()
    {
        // Second run on the same db must succeed (DbUp tracks applied scripts).
        var updater = new SqliteDbUpdater(new RecordingLogger(), Microsoft.Extensions.Options.Options.Create(_config));
        Assert.True(updater.UpdateDb());
    }

    public void Dispose()
    {
        _provider.Dispose();
        try { File.Delete(_dbPath); } catch (IOException) { /* temp file cleanup best effort */ }
    }
}
