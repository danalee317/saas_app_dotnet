using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = PortalRoles.PortalAdministrator)]
    [ApiController]
    [Route("/api/admin/settings")]
    public class SettingsController : ControllerBase
    {
        private IMFPContext _dbContext { get; }
        private IBrandService _brand { get; }

        private ILogger<SettingsController> _logger { get; }

        public SettingsController(IMFPContext dbContext, IBrandService brand, ILoggerFactory loggerFactory)
        {
            _dbContext = dbContext;
            _brand = brand;
            _logger = loggerFactory.CreateLogger<SettingsController>();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _dbContext.Settings.AsNoTracking().ToArrayAsync());
        }

        [HttpPost("save/{key}")]
        public async Task<IActionResult> Save(string key, [Bind(nameof(Setting.Key), nameof(Setting.Value))] Setting setting)
        {
            _dbContext.Settings.Update(setting);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("highlightable-users")]
        public async Task<IActionResult> GetHighlightableUsers()
        {
            var response = (await (from role in _dbContext.Roles
                         where role.Name == PortalRoles.PortalAdministrator ||
                         role.Name == PortalRoles.BlogAuthor || role.Name == PortalRoles.Underwriter
                         join userRole in _dbContext.UserRoles
                         on role.Id equals userRole.RoleId
                         join user in _dbContext.Users
                         on userRole.UserId equals user.Id
                         select new AdminTheme.Models.HighlightableUser
                         {
                             DisplayName = user.FirstName + " " + user.LastName,
                             Title = user.Title,
                             Email = user.Email,
                             UserId = user.Id
                         }).ToArrayAsync())
                         .DistinctBy(x => x.UserId);

            var existing = await _dbContext.HighlightedUsers.ToArrayAsync();
            foreach(var current in existing)
            {
                var user = response.FirstOrDefault(x => x.UserId == current.UserId);
                if(user is not null)
                    user.Order = current.Order;
            }

            return Ok(response);
        }

        [HttpPost("highlightable-users/update")]
        public async Task<IActionResult> UpdateHighlightableUsers([FromBody]IEnumerable<AdminTheme.Models.HighlightableUser> users)
        {
            var existing = await _dbContext.HighlightedUsers.ToArrayAsync();
            _dbContext.HighlightedUsers.RemoveRange(existing);
            await _dbContext.SaveChangesAsync();

            var updated = users.Select(x => new HighlightedUser
            {
                Order = x.Order,
                UserId = x.UserId,
            });
            await _dbContext.HighlightedUsers.AddRangeAsync(updated);
            await _dbContext.SaveChangesAsync();

            return await GetHighlightableUsers();
        }

        [HttpGet("themes")]
        public async Task<IActionResult> GetThemes()
        {
            var themes = await _dbContext.SiteThemes.ToArrayAsync();
            return Ok(themes);
        }

        [HttpPost("themes/default")]
        public async Task<IActionResult> SetDefaultTheme([Bind(nameof(SiteTheme.Id))]SiteTheme defaultTheme)
        {
            var themes = await _dbContext.SiteThemes.ToArrayAsync();

            foreach(var theme in themes)
                theme.IsDefault = theme.Id == defaultTheme.Id;

            _dbContext.SiteThemes.UpdateRange(themes);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("email-templates")]
        public async Task<IActionResult> GetEmailTemplates()
        {
            var templates = await _dbContext.EmailTemplates.ToArrayAsync();
            return Ok(templates);
        }

        [HttpPost("email-templates/update")]
        public async Task<IActionResult> UpdateEmailTemplate([Bind(nameof(EmailTemplate.Key), nameof(EmailTemplate.Html), nameof(EmailTemplate.PlainText))]EmailTemplate request)
        {
            var template = await _dbContext.EmailTemplates.FirstOrDefaultAsync(x => x.Key == request.Key);
            if (template is null)
                return NotFound();

            template.Html = request.Html;
            template.PlainText = request.PlainText;
            template.LastUpdated = DateTimeOffset.Now;

            _dbContext.EmailTemplates.Update(template);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("branding")]
        public async Task<IActionResult> GetBranding()
        {
            // We'll want to update colors and set the logo
            throw new NotImplementedException();
        }

        [HttpPost("branding/{imageName}")]   
        public async Task<IActionResult> UpdateBrandingLogo(string imageName, [FromForm] IFormFile file, [FromServices] IWebHostEnvironment env)
        {
            if (file.Length > 0)
            {
                try
                {
                    var physicalPath = Path.Combine(env.ContentRootPath, "App_Data", imageName == "favicon" ? "Icons" : "Brands");
                    var fileName = "favicon" + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(physicalPath, fileName);
                    Directory.CreateDirectory(physicalPath);

                    if (imageName == "favicon")
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await file.CopyToAsync(stream);
                        await _brand.CreateIcons(filePath);
                    }
                    else
                    {
                        await _brand.CreateImage(file, imageName, physicalPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading logo");
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("No file uploaded");
                return BadRequest("No file was uploaded.");
            }

            return Ok();
        }
    }
}
