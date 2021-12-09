using Microsoft.Extensions.Hosting;
using MultiFamilyPortal.SaaS;

namespace MultiFamilyPortal.Services
{
    internal class LocalStorageService : IStorageService
    {
        private string _basePath { get; }
        private ITenantProvider _tenantProvider { get; }

        public LocalStorageService(IHostEnvironment environment, ITenantProvider tenantProvider)
        {
            _basePath = Path.Combine(environment.ContentRootPath, "App_Data", "files");
            _tenantProvider = tenantProvider;
            Directory.CreateDirectory(_basePath);
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullPath = Path.Combine(_basePath, path);
                File.Delete(fullPath);
            }
            finally
            {}
            return Task.CompletedTask;
        }

        public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            var data = await File.ReadAllBytesAsync(fullPath, cancellationToken);
            return new MemoryStream(data);
        }

        public async Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            var fullPath = GetFullPath(path);
            var fileInfo = new FileInfo(fullPath);
            if (fileInfo.Exists)
            {
                if (overwrite)
                    fileInfo.Delete();
                else
                    return StoragePutResult.AlreadyExists;
            }

            fileInfo.Directory.Create();
            using var fileStream = fileInfo.OpenWrite();
            await content.CopyToAsync(fileStream);

            return StoragePutResult.Success;
        }

        private string GetFullPath(string path)
        {
            var tenant = _tenantProvider.GetTenant();
            var dirPath = Path.Combine(_basePath, tenant.Host);
            Directory.CreateDirectory(dirPath);
            return Path.Combine(dirPath, path);
        }
    }
}
