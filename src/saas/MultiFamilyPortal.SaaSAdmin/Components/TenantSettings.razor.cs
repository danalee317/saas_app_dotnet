using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Internals;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.SaaS.Data;
using MultiFamilyPortal.SaaS.Models;
using MultiFamilyPortal.SaaS.TenantProviders;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.SaaSAdmin.Components
{
    public partial class TenantSettings
    {
        [Parameter]
        public Tenant? Tenant { get; set; }

        [Parameter]
        public EventCallback<Tenant?> TenantChanged { get; set; }

        [Inject]
        private IConfiguration _configuration { get; set; }

        private IEnumerable<Setting> _settings = Array.Empty<Setting>();
        private bool didLoad;
        private string status = "Loading...";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                using var dbContext = CreateDbContext();
                status = "Checking Database Migrations";
                StateHasChanged();
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    status = "Applying Database Migrations";
                    StateHasChanged();
                    await dbContext.Database.MigrateAsync();
                }

                status = "Ensuring Database is Seeded";
                StateHasChanged();
                var seeder = new DatabaseContextSeeder();
                await seeder.SeedAsync(dbContext);
            }
            catch (Exception ex)
            {
                status = ex.Message;
                return;
            }
            status = "Updating Settings";
            StateHasChanged();
            await UpdateSettings();
            didLoad = true;
        }

        private async Task Close()
        {
            await TenantChanged.InvokeAsync(null);
        }

        private async Task OnSettingUpdated(GridCommandEventArgs args)
        {
            var setting = args.Item as Setting;
            if (setting == null)
                return;

            using var dbContext = CreateDbContext();
            var existing = await dbContext.Settings.FirstOrDefaultAsync(x => x.Key == setting.Key);

            if (existing == null)
                return;

            existing.Value = setting.Value;
            dbContext.Settings.Update(existing);
            await dbContext.SaveChangesAsync();

            await UpdateSettings();
        }

        private async Task UpdateSettings()
        {
            using var dbContext = CreateDbContext();
            _settings = await dbContext.Settings.ToArrayAsync();
        }

        private MFPContext CreateDbContext()
        {
            var connStrTemplate = _configuration.GetConnectionString("DefaultConnection");
            var options = new DbContextOptionsBuilder<MFPContext>()
                .Options;
            var identityOptions = new TempOptions<OperationalStoreOptions>
            {
                Value = new OperationalStoreOptions
                {

                }
            };
            return new MFPContext(options, identityOptions, new StartupTenantProvider(Tenant), _configuration.Get<DatabaseSettings>());
        }

        private class TempOptions<T> : IOptions<T>
            where T : class
        {
            public T Value { get; set; }
        }
    }
}
