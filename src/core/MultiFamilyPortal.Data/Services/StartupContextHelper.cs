using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.SaaS.Data;
using MultiFamilyPortal.SaaS.Models;
using MultiFamilyPortal.SaaS.TenantProviders;

namespace MultiFamilyPortal.Data.Services
{
    internal class StartupContextHelper : IStartupContextHelper
    {
        private TenantContext _tenantContext { get; }
        private IConfiguration _configuration { get; }
        private IServiceProvider _services { get; }
        private ILogger _logger { get; }

        public StartupContextHelper(TenantContext tenantContext,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory)
        {
            _tenantContext = tenantContext;
            _configuration = configuration;
            _services = serviceProvider;
            _logger = loggerFactory.CreateLogger<StartupContextHelper>();
        }

        public async Task RunDatabaseAction(Func<MFPContext, Tenant, Task> action)
        {
            var tenants = await _tenantContext.Tenants.AsNoTracking().ToArrayAsync();
            var options = new DbContextOptionsBuilder<MFPContext>().Options;
            var settings = new DatabaseSettings();
            _configuration.Bind(settings);
            var operationalStoreOptions = _services.GetRequiredService<IOptions<OperationalStoreOptions>>();
            var tenantAccessor = _services.GetRequiredService<ITenantAccessor>();
            foreach (var tenant in tenants)
            {
                try
                {
                    tenantAccessor.Current = tenant;
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

        public async Task RunUserManagerAction(Func<UserManager<SiteUser>, Tenant, Task> action)
        {
            var errors = _services.GetService<IdentityErrorDescriber>();
            var optionsAccessor = _services.GetRequiredService<IOptions<IdentityOptions>>();
            var passwordHasher = _services.GetService<IPasswordHasher<SiteUser>>();
            var userValidators = _services.GetServices<IUserValidator<SiteUser>>();
            var passwordValidators = _services.GetServices<IPasswordValidator<SiteUser>>();
            var keyNormalizer = _services.GetService<ILookupNormalizer>();
            var logger = _services.GetService<ILogger<UserManager<SiteUser>>>();

            await RunDatabaseAction((db, tenant) =>
            {
                var store = new UserStore<SiteUser, IdentityRole, MFPContext, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>(db, errors);
                var userManager = new UserManager<SiteUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, _services, logger);
                return action(userManager, tenant);
            });
        }
    }
}
