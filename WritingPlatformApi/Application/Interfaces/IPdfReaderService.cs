namespace Application.Interfaces
{
    public interface IPdfReaderService
    {
        int GetPageCount(Stream filePath);
    }

}
