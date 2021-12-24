using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.AdminTheme.Models;

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
        public async Task<IActionResult> UnderwritingAnalytics()
        {
            try
            {
                var email = User.FindFirstValue(ClaimTypes.Email);
                var user = await _dbContext.Users
                              .Include(x => x.Goals)
                              .AsNoTracking()
                              .FirstOrDefaultAsync(x => x.Email == email);

                var now = DateTime.Now;
                var _7DaysAgo = now.AddDays(-7);
                var _30DaysAgo = now.AddMonths(-1);
                int monthly = 0;
                int weekly = 0;
                int active = 0;
                int passed = 0;
                int offerSubmited = 0;
                int offerAccepted = 0;
                int offerRejected = 0;
                int lSubmited = 0;
                int lAccepted = 0;
                int lRejected = 0;
                double monthlyP = 0;
                double weeklyP = 0;

                if (await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id).AnyAsync())
                {
                    var allUnderwritings = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id).CountAsync();
                    var firstDate = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id).OrderBy(x => x.Timestamp).FirstOrDefaultAsync();
                    var allUnderwritingsLastMonth = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Timestamp >= _30DaysAgo).CountAsync();
                    var allUnderwritingsLastWeek = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Timestamp >= _7DaysAgo).CountAsync();
                    var months = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 30;
                    var weeks = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 7;

                    switch ((int)months)
                    {
                        case <= 0:
                            monthly = allUnderwritings;
                            break;
                        default:
                            monthly = allUnderwritings / (int)months;
                            break;
                    }

                    switch ((int)weeks)
                    {
                        case <= 0:
                            weekly = allUnderwritings;
                            break;
                        default:
                            weekly = allUnderwritings / (int)weeks;
                            break;
                    }

                    var referenceMonth = months <= 1 ? 1 : months - 1;
                    var referenceWeek = weeks <= 1 ? 1 : weeks - 1;

                    monthlyP = (double)allUnderwritings - allUnderwritingsLastMonth / referenceMonth;
                    weeklyP = (double)allUnderwritings - allUnderwritingsLastMonth / referenceWeek;

                    active = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Status == UnderwritingStatus.Active).CountAsync();
                    passed = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Status == UnderwritingStatus.Passed).CountAsync();
                    offerSubmited = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Status == UnderwritingStatus.OfferSubmitted).CountAsync();
                    offerAccepted = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Status == UnderwritingStatus.OfferAccepted).CountAsync();
                    offerRejected = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Status == UnderwritingStatus.OfferRejected).CountAsync();
                    lSubmited = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Status == UnderwritingStatus.LOISubmitted).CountAsync();
                    lAccepted = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Status == UnderwritingStatus.LOIAccepted).CountAsync();
                    lRejected = await _dbContext.UnderwritingPropertyProspects.Where(x => x.UnderwriterId == user.Id && x.Status == UnderwritingStatus.LOIRejected).CountAsync();
                }

                var result = new DashboardUnderwritingResponse
                {
                    MonthlyReports = monthly,
                    WeeklyReports = weekly,
                    WeeklyGoal = user.Goals.PropertiesUnderwritten,
                    Active = active,
                    Passed = passed,
                    OfferSubmitted = offerSubmited,
                    OfferAccepted = offerAccepted,
                    OfferRejected = offerRejected,
                    LOISubmitted = lSubmited,
                    LOIAccepted = lAccepted,
                    LOIRejected = lRejected,
                    MonthlyPercent = monthlyP,
                    WeeklyPercent = weeklyP,
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
        public async Task<IActionResult> ActivityAnalytics()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _dbContext.Users
                          .Include(x => x.Goals)
                          .AsNoTracking()
                          .FirstOrDefaultAsync(x => x.Email == email);

            var activies = await _dbContext.ActivityLogs.Where(x => x.Timestamp >= DateTime.Now.AddDays(-7)).AsNoTracking().ToArrayAsync();

            var totalActivity = TimeSpan.FromMinutes(activies.Sum(x => x.Total.TotalMinutes));
            Console.WriteLine($"Total Activity: {totalActivity}");

            var breakdown = activies.Where(x => x.UserId == user.Id)
                .GroupBy(x => x.Type)
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
