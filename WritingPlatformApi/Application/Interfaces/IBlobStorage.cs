using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IBlobStorage
    {
        Task<bool> FileExistsAsync(string fileName);
        Task PutContextAsync(string filename, Stream file);
        List<int> FindByShop(Guid courseId);
        Task<List<string>> GetAllAsync();
        Task DeleteAsync(string filename);
    }
}
