
using Azure.Storage.Blobs;

namespace SoliGest.Server.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["BlobStorage:ConnectionString"];
            _containerName = configuration["BlobStorage:ContainerName"];
        }

        public async Task<string> UploadFileAsync(IFormFile file, string fileName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString(); // retorna URL público
        }
    }
}
