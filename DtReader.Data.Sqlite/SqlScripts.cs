using System.IO;
using System.Reflection;

namespace DtReader.Data.Sqlite;

public record SqlScript(string EmbeddedKey);

public static class SqlScripts
{
    
    public static readonly SqlScript GetBookInfos = new($"{ScriptLoc}.GetBookInfos.sql");

    private static readonly Assembly ThisAssembly = typeof(SqlScripts).Assembly; 
    private const string ScriptLoc = "DtReader.Data.Sqlite.Sql";
    internal static string GetSql(this SqlScript sqlScript)
    {
        using var stream = new StreamReader(ThisAssembly.GetManifestResourceStream(sqlScript.EmbeddedKey)!);
        return stream.ReadToEnd();
    }

}