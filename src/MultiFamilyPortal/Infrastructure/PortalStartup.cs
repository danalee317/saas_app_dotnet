using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public PortalStartup(UserManager<SiteUser> userManager, RoleManager<IdentityRole> roleManager, IEnumerable<IPortalFrontendTheme> themes, IMFPContext dbContext)
        {
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
            if(await _userManager.Users.AnyAsync(x => x.Email != "admin@website.com"))
            {
                await EnsureDefaultAccountCanBeDeleted();
            }
            else if(!await _userManager.Users.AnyAsync())
            {
                await AddDefaultAccount();
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
                }
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
            foreach(var theme in _themes)
            {
                if(!await _dbContext.SiteThemes.AnyAsync(x => x.Id == theme.Name))
                {
                    var siteTheme = new SiteTheme
                    {
                        Id = theme.Name,
                        IsDefault = theme.GetType().Name.Contains("DefaultTheme")
                    };
                    _dbContext.SiteThemes.Add(siteTheme);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task EnsureDefaultAccountCanBeDeleted()
        {
            if (await _dbContext.UnderwritingPropertyProspects.AnyAsync(x => x.Underwriter.Email == "admin@website.com"))
            {
                var adminRole = await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == PortalRoles.PortalAdministrator);
                var userRoles = await _dbContext.UserRoles.ToArrayAsync();
                var userRole = userRoles.FirstOrDefault(ur => ur.RoleId == adminRole.Id);

                if(userRole is null)
                    return;

                var prospects = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Underwriter.Email == "admin@website.com").ToArrayAsync();
                foreach (var prospect in prospects)
                    prospect.UnderwriterId = userRole.UserId;

                _dbContext.UnderwritingPropertyProspects.UpdateRange(prospects);
                await _dbContext.SaveChangesAsync();
            }

            var user = await _userManager.FindByEmailAsync("admin@website.com");

            var highlightedUser = await _dbContext.HighlightedUsers.FirstOrDefaultAsync(x => x.UserId == user.Id);
            var goals = await _dbContext.UnderwriterGoals.FirstOrDefaultAsync(x => x.UnderwriterId == user.Id);

            if(highlightedUser != null)
            {
                _dbContext.HighlightedUsers.Remove(highlightedUser);
                await _dbContext.SaveChangesAsync();
            }

            if(goals != null)
            {
                _dbContext.UnderwriterGoals.Remove(goals);
                await _dbContext.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(user);
        }

        private async Task AddDefaultAccount()
        {
            var user = new SiteUser("admin@website.com") {
                FirstName = "Admin",
                LastName = "User",
                Email = "admin@website.com" ,
                EmailConfirmed = true,
                Title = "Demo User",
                Bio = "This is a demo user. Be sure to set up an account and delete this user when you are done."
            };

            var result = await _userManager.CreateAsync(user, "mfportal123!");

            await _userManager.AddToRolesAsync(user, new[]
            {
                PortalRoles.PortalAdministrator,
                PortalRoles.Underwriter,
                PortalRoles.BlogAuthor
            });

            user = await _dbContext.Users.FirstAsync(x => x.Email == user.Email);
            _dbContext.HighlightedUsers.Add(new HighlightedUser
            {
                Order = 1,
                UserId = user.Id,
            });
            _dbContext.UnderwriterGoals.Add(new UnderwriterGoal { UnderwriterId = user.Id });
            await _dbContext.SaveChangesAsync();
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
            var defaultPartials = new[] {
                new EmailPartialTemplate {
                    Key = "category",
                    Content = GetTemplate("category.html")
                },
                new EmailPartialTemplate {
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

            var defaultTemplates = new[] {
                new EmailTemplate {
                    Key = "subscribernotification",
                    Model = typeof(Dtos.SubscriberNotification).FullName,
                    Html = GetTemplate("subscribernotification.html"),
                    PlainText = GetTemplate("subscribernotification.txt"),
                    LastUpdated = DateTimeOffset.Now
                },
                new EmailTemplate {
                    Key = "contact-form",
                    Model = typeof(Dtos.ContactFormEmailNotification).FullName,
                    Html = GetTemplate("contact-form.html"),
                    PlainText = GetTemplate("contact-form.txt"),
                    LastUpdated = DateTimeOffset.Now
                }
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
