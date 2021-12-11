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
using static System.Formats.Asn1.AsnWriter;

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
            using var scope = _services.CreateScope();
            await RunDatabaseAction(action, scope);
        }

        private async Task RunDatabaseAction(Func<MFPContext, Tenant, Task> action, IServiceScope scope)
        {
            var tenants = await _tenantContext.Tenants.AsNoTracking().ToArrayAsync();
            var settings = new DatabaseSettings();
            _configuration.Bind(settings);
            var tenantAccessor = scope.ServiceProvider.GetRequiredService<ITenantAccessor>();
            foreach (var tenant in tenants)
            {
                try
                {
                    var options = new DbContextOptionsBuilder<MFPContext>().Options;
                    var operationalStoreOptions = scope.ServiceProvider.GetRequiredService<IOptions<OperationalStoreOptions>>();
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
            using var scope = _services.CreateScope();
            await RunDatabaseAction(async (db, tenant) =>
            {
                var store = scope.ServiceProvider.GetRequiredService<IRoleStore<IdentityRole>>();
                var roleValidators = scope.ServiceProvider.GetServices<IRoleValidator<IdentityRole>>();
                var lookupNormalizer = scope.ServiceProvider.GetRequiredService<ILookupNormalizer>();
                var errorDescriber = scope.ServiceProvider.GetRequiredService<IdentityErrorDescriber>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<RoleManager<IdentityRole>>>();
                var roleManager = new RoleManager<IdentityRole>(store, roleValidators, lookupNormalizer, errorDescriber, logger);
                await action(roleManager, tenant);
            }, scope);
        }

        public async Task RunUserManagerAction(Func<UserManager<SiteUser>, Tenant, Task> action)
        {
            using var scope = _services.CreateScope();
            var errors = scope.ServiceProvider.GetService<IdentityErrorDescriber>();
            var optionsAccessor = scope.ServiceProvider.GetRequiredService<IOptions<IdentityOptions>>();
            var passwordHasher = scope.ServiceProvider.GetService<IPasswordHasher<SiteUser>>();
            var userValidators = scope.ServiceProvider.GetServices<IUserValidator<SiteUser>>();
            var passwordValidators = scope.ServiceProvider.GetServices<IPasswordValidator<SiteUser>>();
            var keyNormalizer = scope.ServiceProvider.GetService<ILookupNormalizer>();
            var logger = scope.ServiceProvider.GetService<ILogger<UserManager<SiteUser>>>();

            await RunDatabaseAction((db, tenant) =>
            {
                var store = new UserStore<SiteUser, IdentityRole, MFPContext, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>(db, errors);
                var userManager = new UserManager<SiteUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, scope.ServiceProvider, logger);
                return action(userManager, tenant);
            });
        }
    }
}
