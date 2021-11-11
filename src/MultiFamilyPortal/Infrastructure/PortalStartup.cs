using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.FirstRun;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Themes;
using System.Reflection;

namespace MultiFamilyPortal.Infrastructure
{
    public class PortalStartup : IStartupTask
    {
        private UserManager<SiteUser> _userManager { get; }
        private RoleManager<IdentityRole> _roleManager { get; }
        private IEnumerable<IPortalFrontendTheme> _themes { get; }
        private IMFPContext _dbContext { get; }
        private ISiteConfigurationValidator _configurationValidator { get; }

        public PortalStartup(UserManager<SiteUser> userManager, RoleManager<IdentityRole> roleManager, IEnumerable<IPortalFrontendTheme> themes, IMFPContext dbContext, ISiteConfigurationValidator configurationValidator)
        {
            _configurationValidator = configurationValidator;
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _themes = themes;
        }
        public async Task StartAsync()
        {
            await SeedSiteContent();
            await SeedThemes();
            await SeedEmailTemplates();
            await SeedRoles();
            if(!await _userManager.Users.AnyAsync())
            {
                _configurationValidator.SetFirstRunTheme(new FirstRunTheme());
            }
        }

        private async Task SeedSiteContent()
        {
            var pages = new[]
            {
                new CustomContent
                {
                    Id = PortalPage.Privacy,
                    Title = "Privacy Policy",
                    HtmlContent = GetTemplate("privacy.html")
                },
                new CustomContent
                {
                    Id = PortalPage.Terms,
                    Title = "Terms & Conditions",
                    HtmlContent = GetTemplate("terms-conditions.html")
                },
                new CustomContent
                {
                    Id = PortalPage.Strategy,
                    Title = "Strategy",
                    HtmlContent = GetTemplate("strategy.html")
                },
            };

            foreach(var page in pages)
            {
                if(!await _dbContext.CustomContent.AnyAsync(x => x.Title == page.Title))
                {
                    await _dbContext.CustomContent.AddAsync(page);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task SeedThemes()
        {
            var defaultThemeName = nameof(PortalTheme.PortalTheme);
            foreach (var theme in _themes)
            {
                if (!await _dbContext.SiteThemes.AnyAsync(x => x.Id == theme.Name))
                {
                    var siteTheme = new SiteTheme
                    {
                        Id = theme.Name,
                        IsDefault = theme.GetType().Name.Contains(defaultThemeName)
                    };
                    _dbContext.SiteThemes.Add(siteTheme);
                    await _dbContext.SaveChangesAsync();
                }
            }

            if(!await _dbContext.SiteThemes.AnyAsync(x => x.IsDefault == true))
            {
                var theme = await _dbContext.SiteThemes
                    .FirstOrDefaultAsync(x => x.Id == defaultThemeName);
                theme.IsDefault = true;
                _dbContext.SiteThemes.Update(theme);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task SeedRoles()
        {
            var type = typeof(PortalRoles);
            var allRoles = type.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => x.Name);

            foreach (var roleName in allRoles)
            {
                if (await _roleManager.RoleExistsAsync(roleName))
                    continue;

                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private async Task SeedEmailTemplates()
        {
            var defaultPartials = new[]
            {
                new EmailPartialTemplate
                {
                    Key = "category",
                    Content = GetTemplate("category.html")
                },
                new EmailPartialTemplate
                {
                    Key = "tag",
                    Content = GetTemplate("tag.html")
                }
            };
            foreach(var partial in defaultPartials)
            {
                if(!await _dbContext.EmailPartialTemplates.AnyAsync(x => x.Key == partial.Key))
                {
                    _dbContext.EmailPartialTemplates.Add(partial);
                    await _dbContext.SaveChangesAsync();
                }
            }

            var defaultTemplates = new[]
            {
                new EmailTemplate
                {
                    Key = PortalTemplate.BlogSubscriberNotification,
                    Description = "Sent to subscribers when a Blog Post or Newsletter is published",
                    Model = typeof(Dtos.SubscriberNotification).AssemblyQualifiedName,
                    Html = GetTemplate("subscribernotification.html"),
                    PlainText = GetTemplate("subscribernotification.txt"),
                    LastUpdated = DateTimeOffset.Now
                },
                new EmailTemplate
                {
                    Key = PortalTemplate.ContactMessage,
                    Description = "This may be used when sending messages to users such as password resets, or confirmation messages",
                    Model = typeof(Dtos.ContactFormEmailNotification).AssemblyQualifiedName,
                    Html = GetTemplate("contact-message.html"),
                    PlainText = GetTemplate("contact-message.txt"),
                    LastUpdated = DateTimeOffset.Now
                },
                new EmailTemplate
                {
                    Key = PortalTemplate.ContactNotification,
                    Description = "This is sent to the public Email address for the website. This will contain the contents of the contact form",
                    Model = typeof(Dtos.ContactNotificationTemplate).AssemblyQualifiedName,
                    Html = GetTemplate("contact-notification.html"),
                    PlainText = GetTemplate("contact-notification.txt"),
                    LastUpdated = DateTimeOffset.Now
                },
                new EmailTemplate
                {
                    Key = PortalTemplate.InvestorNotification,
                    Description = "This is sent to the public Email address for the website. This will contain the contents of the investor contact form",
                    Model = typeof(Dtos.InvestorInquiryNotificationTemplate).AssemblyQualifiedName,
                    Html = GetTemplate("investor-notification.html"),
                    PlainText = GetTemplate("investor-notification.txt"),
                    LastUpdated = DateTimeOffset.Now
                },
            };

            foreach (var template in defaultTemplates)
            {
                if (!await _dbContext.EmailTemplates.AnyAsync(x => x.Key == template.Key))
                {
                    _dbContext.EmailTemplates.Add(template);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private static string GetTemplate(string name)
        {
            var assembly = typeof(PortalStartup).Assembly;
            using var stream = assembly.GetManifestResourceStream($"MultiFamilyPortal.Templates.{name}");

            if (stream is null || stream == Stream.Null)
                return string.Empty;

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
