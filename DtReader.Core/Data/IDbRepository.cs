using System.Collections.Generic;
using System.Threading.Tasks;
using DtReader.Core.Models;

namespace DtReader.Core.Data;

public interface IDbRepository
{
    Task<List<BookInfo>> GetBookInfo();
}