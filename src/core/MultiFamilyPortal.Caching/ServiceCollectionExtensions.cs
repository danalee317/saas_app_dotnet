using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MultiFamilyPortal.Caching
{
    /// <summary>
    /// Extension methods to register the output caching service.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the output caching service with the dependency injection system.
        /// </summary>
        public static IServiceCollection AddOutputCaching(this IServiceCollection services)
        {
            return services.AddOutputCaching(o => { });
        }

        /// <summary>
        /// Registers the output caching service with the dependency injection system.
        /// </summary>
        public static IServiceCollection AddOutputCaching(this IServiceCollection services, Action<OutputCacheOptions> outputCacheOptions)
        {
            var options = new OutputCacheOptions();
            outputCacheOptions(options);

            services.AddSingleton(options);
            services.TryAddSingleton<IOutputCachingService, OutputCachingService>();
            return services;
        }
    }
}
