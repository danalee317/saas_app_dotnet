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
        private IConfiguration _configuration { get; }
        private IServiceProvider _services { get; }
        private ILogger _logger { get; }

        public StartupContextHelper(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _services = serviceProvider;
            _logger = loggerFactory.CreateLogger<StartupContextHelper>();
        }

        public async Task RunStartupTask(Func<Tenant, IServiceProvider, Task> action)
        {
            using var root = _services.CreateScope();
            using var tenantContext = root.ServiceProvider.GetRequiredService<TenantContext>();
            var tenants = await tenantContext.Tenants.AsNoTracking().ToArrayAsync();

            foreach (var tenant in tenants)
            {
                var scope = _services.CreateScope();
                var tenantAccessor = scope.ServiceProvider.GetRequiredService<ITenantAccessor>();
                tenantAccessor.Current = tenant;
                await action(tenant, scope.ServiceProvider);
            }
        }

        public async Task RunDatabaseAction(Func<MFPContext, Tenant, Task> action)
        {
            await RunDatabaseActionInternal((services, db, tenant) => action(db, tenant));
        }

        public async Task RunRoleManagerAction(Func<RoleManager<IdentityRole>, Tenant, Task> action)
        {
            await RunDatabaseActionInternal(async (services, db, tenant) =>
            {
                var store = services.GetRequiredService<IRoleStore<IdentityRole>>();
                var roleValidators = services.GetServices<IRoleValidator<IdentityRole>>();
                var lookupNormalizer = services.GetRequiredService<ILookupNormalizer>();
                var errorDescriber = services.GetRequiredService<IdentityErrorDescriber>();
                var logger = services.GetRequiredService<ILogger<RoleManager<IdentityRole>>>();
                var roleManager = new RoleManager<IdentityRole>(store, roleValidators, lookupNormalizer, errorDescriber, logger);
                await action(roleManager, tenant);
            });
        }

        public async Task RunUserManagerAction(Func<UserManager<SiteUser>, Tenant, Task> action)
        {
            await RunDatabaseActionInternal((services, db, tenant) =>
            {
                var errors = services.GetService<IdentityErrorDescriber>();
                var optionsAccessor = services.GetRequiredService<IOptions<IdentityOptions>>();
                var passwordHasher = services.GetService<IPasswordHasher<SiteUser>>();
                var userValidators = services.GetServices<IUserValidator<SiteUser>>();
                var passwordValidators = services.GetServices<IPasswordValidator<SiteUser>>();
                var keyNormalizer = services.GetService<ILookupNormalizer>();
                var logger = services.GetService<ILogger<UserManager<SiteUser>>>();
                var store = new UserStore<SiteUser, IdentityRole, MFPContext, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>(db, errors);
                var userManager = new UserManager<SiteUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger);
                return action(userManager, tenant);
            });
        }

        private async Task RunDatabaseActionInternal(Func<IServiceProvider, MFPContext, Tenant, Task> action)
        {
            var settings = new DatabaseSettings();
            _configuration.Bind(settings);
            await RunStartupTask(async (tenant, services) =>
            {
                try
                {
                    var options = new DbContextOptionsBuilder<MFPContext>().Options;
                    var operationalStoreOptions = services.GetRequiredService<IOptions<OperationalStoreOptions>>();

                    using var db = new MFPContext(options, operationalStoreOptions, new StartupTenantProvider(tenant), settings);
                    await action(services, db, tenant);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing database action for Tenant {tenant.Host}");
                }
            });
        }
    }
}
