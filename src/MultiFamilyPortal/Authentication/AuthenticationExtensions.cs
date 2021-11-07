using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthenticationProviders(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new AuthenticationOptions();
            configuration.GetSection("Authentication").Bind(options);

            services.AddTransient<IClaimsTransformation, PortalClaimsTransformation>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PortalPolicy.AdminPortalViewer, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.Underwriter, PortalRoles.Mentor, PortalRoles.BlogAuthor));
                options.AddPolicy(PortalPolicy.Blogger, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.BlogAuthor));
                options.AddPolicy(PortalPolicy.InvestorRelations, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.Underwriter));
                options.AddPolicy(PortalPolicy.Underwriter, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.Underwriter));
                options.AddPolicy(PortalPolicy.UnderwritingViewer, builder =>
                    builder.RequireRole(PortalRoles.PortalAdministrator, PortalRoles.Underwriter, PortalRoles.Mentor, PortalRoles.BlogAuthor));
                options.AddPolicy(PortalPolicy.InvestorPortalViewer, builder =>
                    builder.RequireRole(PortalRoles.Investor, PortalRoles.Sponsor));
            });

            services.AddIdentity<SiteUser, IdentityRole>(options =>
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
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    }
                    else
                    {
                        context.Response.Redirect(context.RedirectUri);
                    }
                    return Task.FromResult(0);
                };
            });
            //services.AddDefaultIdentity<SiteUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddUserManager<UserManager<SiteUser>>()
            //    //.AddRoleManager<RoleManager<IdentityRole>>()
            //    //.AddRoles<IdentityRole>()
            //    .AddEntityFrameworkStores<MFPContext>();
            var authBuilder = services.AddAuthentication();
            if (!string.IsNullOrEmpty(options.Google?.ClientId) && !string.IsNullOrEmpty(options.Google?.ClientSecret))
            {
                authBuilder.AddGoogle(google =>
                {
                    google.ClientId = options.Google.ClientId;
                    google.ClientSecret = options.Google.ClientSecret;
                });
            }

            if (!string.IsNullOrEmpty(options.Microsoft?.ClientId) && !string.IsNullOrEmpty(options.Microsoft?.ClientSecret))
            {
                authBuilder.AddMicrosoftAccount(microsoft =>
                {
                    microsoft.ClientId = options.Microsoft.ClientId;
                    microsoft.ClientSecret = options.Microsoft.ClientSecret;
                });
            }

            services.AddIdentityServer()
                .AddApiAuthorization<SiteUser, MFPContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer();
               //.AddJwtBearer(options =>
               //{
               //    options.TokenValidationParameters = new TokenValidationParameters {
               //        ValidateIssuerSigningKey = true,
               //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
               //              .GetBytes(configuration.GetSection("AppSettings:Key").Value)),
               //        ValidateIssuer = false,
               //        ValidateAudience = false
               //    };
               //});

            return services;
        }
    }
}
