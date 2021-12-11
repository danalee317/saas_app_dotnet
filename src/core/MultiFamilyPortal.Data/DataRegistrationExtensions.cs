using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Internals;
using MultiFamilyPortal.Data.Services;
using MultiFamilyPortal.SaaS.Extensions;

namespace MultiFamilyPortal
{
    public static class DataRegistrationExtensions
    {
        public static IServiceCollection AddMFContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MFPContext>(options => { });

            services.AddScoped<IStartupTask, DbContextStartupTask>();
            services.AddTransient<IBlogContext>(sp => sp.GetRequiredService<MFPContext>());
            services.AddTransient<IMFPContext>(sp => sp.GetRequiredService<MFPContext>());
            services.AddTransient<IStartupContextHelper, StartupContextHelper>();

            services.AddSaaSApplication(configuration);
            return services;
        }
    }
}
