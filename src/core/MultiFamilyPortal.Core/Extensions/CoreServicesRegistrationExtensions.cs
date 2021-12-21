using System.Globalization;
using System.Net.Http.Headers;
using Azure.Storage.Blobs;
using BlazorAnimation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiFamilyPortal.Configuration;
using MultiFamilyPortal.Http;
using MultiFamilyPortal.SaaS.Extensions;
using MultiFamilyPortal.Services;
using PostmarkDotNet;

namespace MultiFamilyPortal.Extensions
{
    public static class CoreServicesRegistrationExtensions
    {
        public static IServiceCollection AddCorePortalServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor()
                .AddSaaSApplication(configuration)
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IBlogSubscriberService, BlogSubscriberService>()
                .AddScoped<IBlogAdminService, BlogAdminService>()
                .AddScoped<ITemplateProvider, DatabaseTemplateProvider>()
                .AddScoped<IIpLookupService, IpLookupService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<IEmailValidationService, EmailValidationService>()
                .AddScoped<IFormService, FormService>()
                .AddScoped<ITimeZoneService, TimeZoneService>()
                .AddScoped<ISiteInfo, SiteInfo>()
                .AddScoped<IStartupTask, BrandStartupTask>()
                .AddScoped<IBrandService, BrandService>()
                .AddScoped<IUnderwritingService, UnderwritingService>();

            var config = new SiteConfiguration();
            configuration.Bind(config);

            services.AddSingleton(config.Captcha);

            services.AddTransient<PostmarkClient>(sp =>
            {
                return new PostmarkClient(config.PostmarkApiKey);
            });

            if (string.IsNullOrEmpty(config?.Storage?.ConnectionString))
            {
                services.AddScoped<IStorageService, LocalStorageService>();
            }
            else
            {
                services.AddScoped(p =>
                {
                    var container = config.Storage.Container;
                    if (string.IsNullOrEmpty(container))
                        container = "tenants";

                    return new BlobContainerClient(config.Storage.ConnectionString, container);
                })
                    .AddScoped<IStorageService, AzureStorageService>();
            }

            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>()
                {
                    new CultureInfo("en-US")
                };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.Configure<AnimationOptions>("Page", options =>
            {
                options.Effect = Effect.FadeIn;
                options.Speed = Speed.Fast;
                options.Delay = TimeSpan.FromMilliseconds(100);
                options.IterationCount = 1;
            });

            services.AddRouting(options => options.LowercaseUrls = true);

            // Setup HttpClient for server side in a client side compatible fashion
            services.AddScoped<BlazorAuthenticationHandler>();
            services.AddScoped<HttpClient>(s =>
            {
                // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                var uriHelper = s.GetRequiredService<NavigationManager>();
                var handler = s.GetRequiredService<BlazorAuthenticationHandler>();
                var client = new HttpClient(handler)
                {
                    BaseAddress = new Uri(uriHelper.BaseUri)
                };

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                return client;
            });

            return services;
        }
    }
}
