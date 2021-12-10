namespace MultiFamilyPortal.Services
{
    public static class StorageServiceExtensions
    {
        public static async Task<bool> ExistsAsync(this IStorageService storage, string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var stream = await storage.GetAsync(path, cancellationToken);
                return stream?.Length > 0 && stream != Stream.Null;
            }
            catch
            {
            }

            return false;
        }
    }
}
