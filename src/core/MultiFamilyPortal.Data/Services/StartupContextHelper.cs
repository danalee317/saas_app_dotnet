using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultiFamilyPortal.SaaS.Data;
using MultiFamilyPortal.SaaS.Models;
using MultiFamilyPortal.SaaS.TenantProviders;

namespace MultiFamilyPortal.Data.Services
{
    internal class StartupContextHelper : IStartupContextHelper
    {
        private TenantContext _tenantContext { get; }
        private IConfiguration _configuration { get; }
        private IServiceProvider _serviceProvider { get; }
        private ILogger _logger { get; }

        public StartupContextHelper(TenantContext tenantContext,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory)
        {
            _tenantContext = tenantContext;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<StartupContextHelper>();
        }

        public async Task RunDatabaseAction(Func<MFPContext, Tenant, Task> action)
        {
            var tenants = await _tenantContext.Tenants.AsNoTracking().ToArrayAsync();
            var options = new DbContextOptionsBuilder<MFPContext>().Options;
            var settings = new DatabaseSettings();
            _configuration.Bind(settings);
            var operationalStoreOptions = _serviceProvider.GetRequiredService<IOptions<OperationalStoreOptions>>();
            foreach (var tenant in tenants)
            {
                try
                {
                    using var db = new MFPContext(options, operationalStoreOptions, new StartupTenantProvider(tenant), settings);
                    await action(db, tenant);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing database action for Tenant {tenant.Host}");
                }
            }
        }

        public async Task RunRoleManagerAction(Func<RoleManager<IdentityRole>, Tenant, Task> action)
        {
            await RunDatabaseAction(async (db, tenant) =>
            {
                //var roleManager = new RoleManager<IdentityRole>()
            });
        }

        public async Task RunUserManagerAction(Func<UserManager<MFPContext>, Tenant, Task> action)
        {
            var optionsAccessor = _serviceProvider.GetRequiredService<IOptions<IdentityOptions>>();
            await RunDatabaseAction(async (db, tenant) =>
            {

                //var userManager = new UserManager<MFPContext>(db, optionsAccessor,  )
            });
        }
    }
}
