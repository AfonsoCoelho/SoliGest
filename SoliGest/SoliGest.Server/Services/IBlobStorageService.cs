namespace SoliGest.Server.Services
{
    public interface IBlobStorageService
    {
        public Task<string> UploadFileAsync(IFormFile file, string fileName);
    }
}
