using DtReader.Data.Sqlite;
using Xunit;

namespace DtReader.Tests.Data;

public class SqlScriptsTests
{
    [Fact]
    public void GetBookInfos_script_is_embedded_and_targets_BookInfo()
    {
        var sql = SqlScripts.GetBookInfos.GetSql();

        Assert.Contains("BookInfo", sql);
        Assert.Contains("SELECT", sql);
    }
}
