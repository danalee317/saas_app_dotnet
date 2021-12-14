using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = PortalPolicy.AdminPortalViewer)]
    [ApiController]
    [Route("/api/[area]/[controller]")]
    public class DashboardController : ControllerBase
    {
        private IMFPContext _dbContext { get; }

        public DashboardController(IMFPContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("analytics")]
        public async Task<IActionResult> Analytics()
        {
            var now = DateTime.Now;
            var _7DaysAgo = now.AddDays(-7);
            var _30DaysAgo = now.AddMonths(-1);
            var goals = new UnderwriterGoal();
            var underwrittenLast7 = await _dbContext.UnderwritingPropertyProspects.CountAsync(x => x.Timestamp > _7DaysAgo);
            var underwrittenLast30 = await _dbContext.UnderwritingPropertyProspects.CountAsync(x => x.Timestamp > _30DaysAgo);
            var prospectsLast7 = await _dbContext.InvestorProspects.CountAsync(x => x.Timestamp > _7DaysAgo);
            var prospectsLast30 = await _dbContext.InvestorProspects.CountAsync(x => x.Timestamp > _30DaysAgo);
            var contactedLast7 = await _dbContext.InvestorProspects.Where(x => x.Contacted == true).CountAsync(x => x.Timestamp > _7DaysAgo);
            var contactedLast30 = await _dbContext.InvestorProspects.CountAsync(x => x.Timestamp > _30DaysAgo && x.Contacted == true);
            var activies = await _dbContext.ActivityLogs.Where(x => x.Timestamp > _30DaysAgo).AsNoTracking().ToArrayAsync();

            var totalActivity = TimeSpan.FromMinutes(activies.Sum(x => x.Total.TotalMinutes));

            var breakdown = activies
                .GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x => TimeSpan.FromMinutes(x.Sum(t => t.Total.Minutes)));

            var result = new DashboardAnalyticsResponse {
                ExpectedUnderwrittenLast7Days = goals.PropertiesUnderwritten,
                ExpectedUnderwrittenLast30Days = goals.PropertiesUnderwritten * 4,
                UnderwrittenLast7Days = underwrittenLast7,
                UnderwrittenLast30Days = underwrittenLast30,
                Prospects7Days = prospectsLast7,
                Prospects30Days = prospectsLast30,
                ProspectsContacted7Days = contactedLast7,
                ProspectsContacted30Days = contactedLast30,
                Activity = new DashboardActivityResponse {
                    Total = totalActivity,
                    Breakdown = breakdown
                }
            };

            return Ok(result);
        }
    }
}
