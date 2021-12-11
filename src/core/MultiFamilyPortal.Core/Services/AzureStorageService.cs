using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using MultiFamilyPortal.Extensions;
using MultiFamilyPortal.SaaS;

namespace MultiFamilyPortal.Services
{
    internal class AzureStorageService : IStorageService
    {
        private readonly BlobContainerClient _container;
        private readonly ITenantProvider _tenantProvider;

        public AzureStorageService(BlobContainerClient container, ITenantProvider tenantProvider)
        {
            _container = container;
            _tenantProvider = tenantProvider;
        }

        public async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            await _container
                .GetBlockBlobClient(GetTenantPath(path))
                .DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }

        public async Task<Stream> GetAsync(string path, CancellationToken cancellationToken = default)
        {
            var client = _container
                .GetBlockBlobClient(GetTenantPath(path));

            if (await client.ExistsAsync(cancellationToken))
                return await client.OpenReadAsync(new BlobOpenReadOptions(false), cancellationToken);

            return Stream.Null;
        }

        public async Task<StoragePutResult> PutAsync(string path, Stream content, string contentType, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            var blob = _container.GetBlockBlobClient(GetTenantPath(path));

            try
            {
                var options = new BlobUploadOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "ContentType", contentType }
                    },
                    AccessTier = AccessTier.Cool,
                };
                await blob.UploadAsync(content, options, cancellationToken);

                return StoragePutResult.Success;
            }
            catch (RequestFailedException e) when (e.IsAlreadyExistsException())
            {
                if(overwrite)
                {
                    await blob.DeleteAsync();
                    return await PutAsync(path, content, contentType, false, cancellationToken);
                }

                using var targetStream = await blob.OpenReadAsync(new BlobOpenReadOptions(true), cancellationToken);
                content.Position = 0;
                return content.Matches(targetStream)
                    ? StoragePutResult.AlreadyExists
                    : StoragePutResult.Conflict;
            }
        }

        private string GetTenantPath(string path)
        {
            var tenant = _tenantProvider.GetTenant();
            return Path.Combine(tenant.Host, path);
        }
    }
}
