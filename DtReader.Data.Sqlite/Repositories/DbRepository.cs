using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DtReader.Core.Data;
using DtReader.Core.Models;

namespace DtReader.Data.Sqlite.Repositories;

public class DbRepository (
    ISqliteConnectionProvider connnectionProvider
): IDbRepository
{
    public async Task<List<BookInfo>> GetBookInfo()
    {
        // TODO: add try catch and logs
        return (await 
            connnectionProvider.Connection
                .QueryAsync<BookInfo>(SqlScripts.GetBookInfos.GetSql())).ToList();
    }
}