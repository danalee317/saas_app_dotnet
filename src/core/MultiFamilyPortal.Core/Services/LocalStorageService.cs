using Microsoft.Extensions.Hosting;
using MultiFamilyPortal.SaaS;

namespace MultiFamilyPortal.Services
{
    internal class LocalStorageService : IStorageService
    {
        private string _contentRoot { get; }
        private string _basePath  => Path.Combine(_contentRoot, "App_Data", _tenantProvider.GetTenant().Host, "files");
        private ITenantProvider _tenantProvider { get; }

        public LocalStorageService(IHostEnvironment environment, ITenantProvider tenantProvider)
        {
            _contentRoot = environment.ContentRootPath;
            _tenantProvider = tenantProvider;
        }

        public Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var fullPath = GetFullPath(path);
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
            var root = _basePath;
            Directory.CreateDirectory(root);
            return Path.Combine(root, path);
        }
    }
}
