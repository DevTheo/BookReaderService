namespace DtReader.Web.Services;

public interface IDjvuFileService : IImageFileService;

public class DjvuFileService : IDjvuFileService
{
    public int GetPageCount(string fileName)
    {
        throw new NotImplementedException();
    }

    public Task<ImagePage> GetPage(string fileName, int pageNumber)
    {
        throw new NotImplementedException();
    }
}