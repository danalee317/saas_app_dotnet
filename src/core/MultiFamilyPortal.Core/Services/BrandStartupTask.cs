namespace MultiFamilyPortal.Services
{
    internal class BrandStartupTask : IStartupTask
    {
        private IBrandService _brand { get; }

        public BrandStartupTask(IBrandService brand)
        {
            _brand = brand;
        }

        public async Task StartAsync()
        {
            await _brand.CreateDefaultIcons();
        }
    }
}
