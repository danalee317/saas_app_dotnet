using Microsoft.Extensions.Hosting;

namespace MultiFamilyPortal.Services
{
    internal class LocalStorageService : IStorageService
    {
        private string _basePath { get; }

        public LocalStorageService(IHostEnvironment environment)
        {
            _basePath = Path.Combine(environment.ContentRootPath, "App_Data", "files");
            Directory.CreateDirectory(_basePath);
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(_basePath, path);
            File.Delete(fullPath);
            return Task.CompletedTask;
        }

        public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(_basePath, path);
            var data = await File.ReadAllBytesAsync(fullPath, cancellationToken);
            return new MemoryStream(data);
        }

        public async Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, CancellationToken cancellationToken = default)
        {
            var fullPath = Path.Combine(_basePath, path);
            var fileInfo = new FileInfo(fullPath);
            if (fileInfo.Exists)
                return StoragePutResult.AlreadyExists;

            fileInfo.Directory.Create();
            using var fileStream = fileInfo.OpenWrite();
            await content.CopyToAsync(fileStream);

            return StoragePutResult.Success;
        }
    }
}
