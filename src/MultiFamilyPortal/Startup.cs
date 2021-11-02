using Microsoft.AspNetCore.Components.Authorization;
using MultiFamilyPortal.Areas.Identity;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Extensions;
using MultiFamilyPortal.Infrastructure;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddMFContext(configuration)
                .AddDatabaseDeveloperPageExceptionFilter()
                .AddAuthenticationProviders(configuration)
                .AddCorePortalServices(configuration)
                .AddScoped<IStartupTask, PortalStartup>()
                .AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<SiteUser>>();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddTelerikBlazor();
            services.RegisterThemes(env);

            // These have to be manually registered to avoid being linked out
            services.RegisterTheme<DefaultTheme.DefaultTheme>();
            services.RegisterTheme<AdminTheme.AdminTheme>();
            services.RegisterTheme<PortalTheme.PortalTheme>();
        }

        public static void ConfigureApp(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for
                // production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseIdentityServer();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
