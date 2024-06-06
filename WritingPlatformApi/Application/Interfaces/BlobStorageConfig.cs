using Microsoft.Extensions.Configuration;

namespace Application.Interfaces
{
    public class BlobStorageConfig
    {
        private readonly IConfiguration _configuration;
        public BlobStorageConfig(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ConnectionString => _configuration.GetConnectionString("BlobConnectionString");
    }
}
