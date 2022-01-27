using System.Net.Mail;
using AvantiPoint.EmailService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.FirstRun.Models;
using MultiFamilyPortal.Services;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.FirstRun.Pages
{
    public partial class Index
    {
        private FirstRunSetup Model = new();
        private bool isBusy;

        [Inject]
        private IMFPContext _dbContext { get; set; }

        [Inject]
        private UserManager<SiteUser> _userManager { get; set; }

        [Inject]
        private ITemplateProvider _templateProvider { get; set; }

        [Inject]
        private IEmailService _emailService { get; set; }

        [Inject]
        private ISiteConfigurationValidator _siteValidator { get; set; }

        [Inject]
        private NavigationManager _navigationManager { get; set; }

        private PortalNotification notification { get; set; }

        private bool SiteInfoIsValid()
        {
            return !string.IsNullOrEmpty(Model.SiteTitle) && IsEmail(Model.SenderEmail) && !string.IsNullOrEmpty(Model.SenderEmailName);
        }

        private bool BusinessEntityIsValid()
        {
            return SiteInfoIsValid() && new[]
            {
                Model.LegalName,
                Model.PublicEmail,
                Model.City,
                Model.State,
                Model.PostalCode,
            }.All(x => !string.IsNullOrEmpty(x)) && IsEmail(Model.PublicEmail);
        }

        private bool AdminAccountIsValid()
        {
            var password = Model.UsePassword ? !string.IsNullOrEmpty(Model.Password) && Model.Password.Length > 5 && Model.Password == Model.ConfirmPassword : true;
            return BusinessEntityIsValid() && IsEmail(Model.AdminUser) && password && !string.IsNullOrEmpty(Model.FirstName) && !string.IsNullOrEmpty(Model.LastName) && !string.IsNullOrEmpty(Model.AdminPhone);
        }

        private bool IsEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                _ = new MailAddress(email);
                var parts = email.Split('@');
                if (parts.Length == 2 && parts[1].Split('.').Length > 1)
                    return true;
            }
            catch
            {
            }

            return false;
        }

        private async Task OnFinish()
        {
            try
            {
                isBusy = true;

                if (!AdminAccountIsValid())
                    return;

                var settings = new[]
                {
                    new Setting
                    {
                        Key = PortalSetting.SiteTitle,
                        Value = Model.SiteTitle
                    },
                    new Setting
                    {
                        Key = PortalSetting.NotificationEmailFrom,
                        Value = Model.SenderEmailName
                    },
                    new Setting
                    {
                        Key = PortalSetting.NotificationEmail,
                        Value = Model.SenderEmail
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactStreetAddress,
                        Value = Model.Address
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactCity,
                        Value = Model.City
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactState,
                        Value = Model.State
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactZip,
                        Value = Model.PostalCode
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactStreetAddress,
                        Value = Model.Address
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactEmail,
                        Value = Model.PublicEmail
                    },
                    new Setting
                    {
                        Key = PortalSetting.ContactPhone,
                        Value = Model.Phone
                    }
                }.Where(x => !string.IsNullOrEmpty(x.Value));

                if (settings.Any())
                {
                    foreach (var setting in settings)
                    {
                        var x = await _dbContext.Settings.FirstOrDefaultAsync(x => x.Key == setting.Key);
                        x.Value = setting.Value;
                        _dbContext.Settings.Update(x);
                    }
                    await _dbContext.SaveChangesAsync();
                }

                var user = new SiteUser(Model.AdminUser)
                {
                    FirstName = Model.FirstName,
                    LastName = Model.LastName,
                    Email = Model.AdminUser,
                    PhoneNumber = Model.AdminPhone,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    Title = "Site Owner",
                    Bio = "<p>Your account is all set up. Be sure to update your Bio in the Admin Portal.</p>"
                };

                IdentityResult result = null;
                if (Model.UsePassword)
                {
                    result = await _userManager.CreateAsync(user, Model.Password);
                }
                else
                {
                    result = await _userManager.CreateAsync(user);
                }

                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(user, new[] { PortalRoles.Underwriter, PortalRoles.BlogAuthor, PortalRoles.PortalAdministrator });
                }
                else
                {
                    notification.ShowError($"Unable to create the admin account: {result.Errors.FirstOrDefault()?.Description}");
                    return;
                }

                var goals = new UnderwriterGoal
                {
                    UnderwriterId = user.Id
                };
                _dbContext.UnderwriterGoals.Add(goals);

                var highlighted = new HighlightedUser
                {
                    Order = 1,
                    UserId = user.Id
                };
                _dbContext.HighlightedUsers.Add(highlighted);
                await _dbContext.SaveChangesAsync();

                var model = new Dtos.ContactFormEmailNotification
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Message = "<p>Congratulations your website has been configured and is now ready to use.</p>",
                    Subject = "Portal Configured",
                    Year = DateTime.Now.Year
                };
                var template = await _templateProvider.GetTemplate(PortalTemplate.ContactMessage, model);
                var emailAddress = new MailAddress(user.Email, user.DisplayName);
                await _emailService.SendAsync(emailAddress, template);

                _siteValidator.SetFirstRunTheme(null);
                _navigationManager.NavigateTo("/", true, true);
            }
            catch (Exception ex)
            {
                notification.ShowError(ex.Message);
            }
            finally
            {
                isBusy = false;
            }
        }
    }
}
