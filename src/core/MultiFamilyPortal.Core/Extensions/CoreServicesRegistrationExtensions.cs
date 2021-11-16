using System.Globalization;
using System.Net.Http.Headers;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiFamilyPortal.Configuration;
using MultiFamilyPortal.Http;
using MultiFamilyPortal.Services;
using SendGrid;

namespace MultiFamilyPortal.Extensions
{
    public static class CoreServicesRegistrationExtensions
    {
        public static IServiceCollection AddCorePortalServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor()
                .AddScoped<IAuthenticationService, AuthenticationService>()
                .AddScoped<IBlogSubscriberService, BlogSubscriberService>()
                .AddScoped<IBlogAdminService, BlogAdminService>()
                .AddScoped<ITemplateProvider, DatabaseTemplateProvider>()
                .AddScoped<IIpLookupService, IpLookupService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<IEmailValidationService, EmailValidationService>()
                .AddScoped<IFormService, FormService>()
                .AddScoped<ITimeZoneService, TimeZoneService>()
                .AddScoped<ISiteInfo, SiteInfo>();

            var config = new SiteConfiguration();
            configuration.Bind(config);

            services.AddSingleton(config.Captcha);

            services.AddTransient<ISendGridClient>(sp =>
            {
                return new SendGridClient(config.SendGridKey);
            });

            if (string.IsNullOrEmpty(config?.Storage?.ConnectionString))
            {
                services.AddScoped<IStorageService, LocalStorageService>();
            }
            else
            {
                services.AddScoped(p =>
                {
                    var context = p.GetRequiredService<IHttpContextAccessor>();
                    var container = context.HttpContext.Request.Host.Host;

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

            services.AddRouting(options => options.LowercaseUrls = true);

            // Setup HttpClient for server side in a client side compatible fashion
            services.AddScoped<BlazorAuthenticationHandler>();
            services.AddScoped<HttpClient>(s =>
            {
                // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                var uriHelper = s.GetRequiredService<NavigationManager>();
                var handler = s.GetRequiredService<BlazorAuthenticationHandler>();
                var client =  new HttpClient(handler)
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
