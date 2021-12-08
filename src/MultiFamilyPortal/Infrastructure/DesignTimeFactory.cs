#if DEBUG
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.SaaS.Data;
using MultiFamilyPortal.SaaS.Models;
using MultiFamilyPortal.SaaS.TenantProviders;

namespace MultiFamilyPortal.Infrastructure
{
    public class DesignTimeFactory : IDesignTimeDbContextFactory<MFPContext>
    {
        public MFPContext CreateDbContext(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddDbContext<MFPContext>(options => { })
                .AddIdentity<SiteUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                })
                .AddEntityFrameworkStores<MFPContext>()
                .AddDefaultTokenProviders();
            var app = builder.Build();
            var storeOptions = app.Services.GetRequiredService<IOptions<OperationalStoreOptions>>();

            var options = new DbContextOptionsBuilder<MFPContext>()
                .Options;
            var settings = new DatabaseSettings();
            builder.Configuration.Bind(settings);
            var tenant = new Tenant
            {
                Created = DateTimeOffset.Now,
                DatabaseName = "MultiFamilyPortal",
                Host = "localhost:7009"
            };
            return new MFPContext(options, storeOptions, new StartupTenantProvider(tenant), settings);
        }
    }
}
#endif
