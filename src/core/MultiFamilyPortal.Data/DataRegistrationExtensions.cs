using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Internals;

namespace MultiFamilyPortal
{
    public static class DataRegistrationExtensions
    {
        public static IServiceCollection AddMFContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MFPContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IStartupTask, DbContextStartupTask>();
            services.AddTransient<IBlogContext>(sp => sp.GetRequiredService<MFPContext>());
            services.AddTransient<IMFPContext>(sp => sp.GetRequiredService<MFPContext>());
            
            return services;
        }
    }
}
