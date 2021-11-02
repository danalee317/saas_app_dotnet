using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = PortalRoles.PortalAdministrator)]
    [ApiController]
    [Route("/api/admin/settings")]
    public class SettingsController : ControllerBase
    {
        private IMFPContext _dbContext { get; }

        public SettingsController(IMFPContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
