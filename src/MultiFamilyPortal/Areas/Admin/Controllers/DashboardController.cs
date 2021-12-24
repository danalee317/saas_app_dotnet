using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.AdminTheme.Models;
using System;
using UAParser;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = PortalPolicy.AdminPortalViewer)]
    [ApiController]
    [Route("/api/[area]/[controller]")]
    public class DashboardController : ControllerBase
    {
        private IMFPContext _dbContext { get; }

        private IBlogContext _context { get; }

        private ILogger<DashboardController> _logger { get; }

        public DashboardController(IMFPContext dbContext, IBlogContext context, ILoggerFactory loggerFactory)
        {
            _dbContext = dbContext;
            _context = context;
            _logger = loggerFactory.CreateLogger<DashboardController>();
        }

        [HttpGet("underwriting")]
        public async Task<IActionResult> UnderwritingAnalytics(string userId = null)
        {
            try
            {
                var weeklyGoal = 0;
                IQueryable<UnderwritingProspectProperty> query = _dbContext.UnderwritingPropertyProspects;
                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(x => x.UnderwriterId == userId);
                    weeklyGoal = await _dbContext.UnderwriterGoals.Where(x => x.UnderwriterId == userId).Select(x => x.PropertiesUnderwritten).FirstOrDefaultAsync();
                }
                else
                {
                    weeklyGoal = await _dbContext.UnderwriterGoals.SumAsync(x => x.PropertiesUnderwritten);
                }

                var now = DateTime.Now;
                var _7DaysAgo = now.AddDays(-7);
                var _30DaysAgo = now.AddMonths(-1);

                var allUnderwritings = await query.CountAsync();
                var firstDate = await query.OrderBy(x => x.Timestamp).FirstOrDefaultAsync();
                var months = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 30;
                var weeks = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 7;
                var allUnderwritingsLastMonth = await query.Where(x => x.Timestamp >= _30DaysAgo).CountAsync();
                var allUnderwritingsLastWeek = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Timestamp >= _7DaysAgo).CountAsync();

                var referenceMonth = months <= 1 ? 1 : months - 1;
                var referenceWeek = weeks <= 1 ? 1 : weeks - 1;

                var result = new DashboardUnderwritingResponse
                {
                    MonthlyReports = (int)months switch
                    {
                        <= 0 => allUnderwritings,
                        _ => allUnderwritings / (int)months,
                    },
                    WeeklyReports = (int)weeks switch
                    {
                        <= 0 => allUnderwritings,
                        _ => allUnderwritings / (int)weeks,
                    },
WeeklyGoal = weeklyGoal,
                    Active = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Status == UnderwritingStatus.Active).CountAsync(),
                    Passed = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Status == UnderwritingStatus.Passed).CountAsync(),
                    OfferSubmitted = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Status == UnderwritingStatus.OfferSubmitted).CountAsync(),
                    OfferAccepted = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Status == UnderwritingStatus.OfferAccepted).CountAsync(),
                    OfferRejected = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Status == UnderwritingStatus.OfferRejected).CountAsync(),
                    LOISubmitted = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Status == UnderwritingStatus.LOISubmitted).CountAsync(),
                    LOIAccepted = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Status == UnderwritingStatus.LOIAccepted).CountAsync(),
                    LOIRejected = await _dbContext.UnderwritingPropertyProspects.Where(x => x.Status == UnderwritingStatus.LOIRejected).CountAsync(),
                    MonthlyPercent = (double)allUnderwritings - allUnderwritingsLastMonth / referenceMonth,
                    WeeklyPercent = (double)allUnderwritings - allUnderwritingsLastMonth / referenceWeek,
                };

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("investors")]
        public async Task<IActionResult> InvestorAnalytics()
        {
            var result = new DashboardInvestorsResponse
            {
                Total = await _dbContext.InvestorProspects.CountAsync(),
                Contacted = await _dbContext.InvestorProspects.Where(x => x.Contacted == true).CountAsync(),
                Investors = await _dbContext.InvestorProspects.Where(x => x.Contacted == false).OrderByDescending(y => y.Timestamp).AsNoTracking().Take(7).ToListAsync()
            };

            return Ok(result);
        }

        [HttpGet("activity")]
        public async Task<IActionResult> ActivityAnalytics(string userId = null)
        {
            IQueryable<ActivityLog> query = _dbContext.ActivityLogs;
            if(!string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.UserId == userId);
            }

            var activies = await query.Where(x => x.Timestamp >= DateTime.Now.AddDays(-7)).AsNoTracking().ToArrayAsync();

            var breakdown = activies.GroupBy(x => x.Type)
                .ToDictionary(x => x.Key, x => x.Sum(t => t.Total.TotalMinutes));

            var result = new DashboardActivityResponse
            {
                Breakdown = breakdown
            };

            return Ok(result);
        }

        [HttpGet("blog")]
        public async Task<IActionResult> BlogAnalytics()
        {
            var monthly = 0;
            var weekly = 0;
            var total = 0;

            if (await _context.Subscribers.AnyAsync())
            {
                var firstDate = await _context.Subscribers.OrderBy(x => x.Timestamp).FirstOrDefaultAsync();
                var months = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 30;
                var weeks = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 7;
                total = await _context.Subscribers.CountAsync();

                switch ((int)months)
                {
                    case <= 0:
                        monthly = total;
                        break;
                    default:
                        monthly = total / (int)months;
                        break;
                }

                switch ((int)weeks)
                {
                    case <= 0:
                        weekly = total;
                        break;
                    default:
                        weekly = total / (int)weeks;
                        break;
                }
            }

            var result = new DashboardBlogResponse()
            {
                Total = total,
                Monthly = monthly,
                Weekly = weekly,
            };

            return Ok(result);
        }
    }
}
