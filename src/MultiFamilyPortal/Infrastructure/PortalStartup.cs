using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Data.Services;
using MultiFamilyPortal.FirstRun;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.Infrastructure
{
    public class PortalStartup : IStartupTask
    {
        private IEnumerable<IPortalFrontendTheme> _themes { get; }
        private IStartupContextHelper _contextHelper { get; }
        private ISiteConfigurationValidator _configurationValidator { get; }
        private IServiceProvider _serviceProvider { get; }

        public PortalStartup(IEnumerable<IPortalFrontendTheme> themes,
            IStartupContextHelper contextHelper,
            ISiteConfigurationValidator configurationValidator)
        {
            _configurationValidator = configurationValidator;
            _contextHelper = contextHelper;
            _themes = themes;
        }
        public async Task StartAsync()
        {
            await SeedSiteContent();
            await SeedThemes();
            await SeedEmailTemplates();
            await SeedRoles();
            await _contextHelper.RunUserManagerAction(async userManager =>
            {
                if (!await userManager.Users.AnyAsync())
                {
                    // TODO: Refactor this... this will cause all portals to go First Run if any of theme require it.
                    //_configurationValidator.SetFirstRunTheme(new FirstRunTheme());

                    // NOTE: If debugging be sure to uncomment the above line to get the FirstRunTheme.
                    System.Diagnostics.Debugger.Break();
                }
            });
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

            await _contextHelper.RunDatabaseAction(async db =>
            {
                foreach (var page in pages)
                {
                    if (!await db.CustomContent.AnyAsync(x => x.Title == page.Title))
                    {
                        await db.CustomContent.AddAsync(page);
                        await db.SaveChangesAsync();
                    }
                }
            });
        }

        private async Task SeedThemes()
        {
            var defaultThemeName = nameof(PortalTheme.PortalTheme);
            await _contextHelper.RunDatabaseAction(async db =>
            {
                foreach (var theme in _themes)
                {
                    if (!await db.SiteThemes.AnyAsync(x => x.Id == theme.Name))
                    {
                        var siteTheme = new SiteTheme
                        {
                            Id = theme.Name,
                            IsDefault = theme.GetType().Name.Contains(defaultThemeName)
                        };
                        db.SiteThemes.Add(siteTheme);
                        await db.SaveChangesAsync();
                    }
                }

                if (!await db.SiteThemes.AnyAsync(x => x.IsDefault == true))
                {
                    var theme = await db.SiteThemes
                        .FirstOrDefaultAsync(x => x.Id == defaultThemeName);
                    theme.IsDefault = true;
                    db.SiteThemes.Update(theme);
                    await db.SaveChangesAsync();
                }
            });
        }

        private async Task SeedRoles()
        {
            var type = typeof(PortalRoles);
            var allRoles = type.GetFields(BindingFlags.Static | BindingFlags.Public).Select(x => x.Name);

            await _contextHelper.RunRoleManagerAction(async roleManager =>
            {
                foreach (var roleName in allRoles)
                {
                    if (await roleManager.RoleExistsAsync(roleName))
                        continue;

                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            });
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
                await _contextHelper.RunDatabaseAction(async db =>
                {
                    if (!await db.EmailPartialTemplates.AnyAsync(x => x.Key == partial.Key))
                    {
                        db.EmailPartialTemplates.Add(partial);
                        await db.SaveChangesAsync();
                    }
                });
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
                await _contextHelper.RunDatabaseAction(async db =>
                {
                    if (!await db.EmailTemplates.AnyAsync(x => x.Key == template.Key))
                    {
                        db.EmailTemplates.Add(template);
                        await db.SaveChangesAsync();
                    }
                });
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
