﻿using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> UnderwritingAnalytics(string userId = null)
        {
            try
            {
                int weeklyGoal = 0;
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
                DateTime _30DaysAgo = now.AddMonths(-1);

                int allUnderwritings = await query.CountAsync();
                var firstDate = await query.OrderBy(x => x.Timestamp).FirstOrDefaultAsync();
                double months = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 30;
                double weeks = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 7;
                int allUnderwritingsLastMonth = await query.Where(x => x.Timestamp >= _30DaysAgo).CountAsync();

                double referenceMonth = months <= 1 ? 1 : months - 1;
                double referenceWeek = weeks <= 1 ? 1 : weeks - 1;

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
                    MonthlyPercent = allUnderwritings - allUnderwritingsLastMonth / referenceMonth,
                    WeeklyPercent = allUnderwritings - allUnderwritingsLastMonth / referenceWeek,
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("investors")]
        public async Task<IActionResult> InvestorAnalytics()
        {
            List<DashboardInvestor> dashboardInvestors = await _dbContext.InvestorProspects
                                             .Where(x => x.Contacted == false)
                                             .AsNoTracking()
                                             .Take(7)
                                             .Select(investor => new DashboardInvestor
                                             {
                                                 Id = investor.Id,
                                                 FirstName = investor.FirstName,
                                                 LastName = investor.LastName,
                                                 Email = investor.Email,
                                                 Phone = investor.Phone,
                                                 Contacted = investor.Contacted,
                                                 Timestamp = investor.Timestamp,
                                                 Timezone = investor.Timezone,
                                                 LookingToInvest = investor.LookingToInvest,
                                              })
                                            .ToListAsync();

            var result = new DashboardInvestorsResponse
            {
                Total = await _dbContext.InvestorProspects.CountAsync(),

                Contacted = await _dbContext.InvestorProspects
                                            .Where(x => x.Contacted == true)
                                            .CountAsync(),
                Investors = dashboardInvestors,
            };

            return Ok(result);
        }

        [HttpPut("investors/{id:guid}")]
        public async Task<IActionResult> InvestorUpdateAsync(Guid id, [FromBody] DashboardInvestor investor)
        {
            if(id == default || id != investor.Id)
               return BadRequest();

            var investorModel = await _dbContext.InvestorProspects.FirstOrDefaultAsync(x => x.Id == id);

            if(investorModel is null)
               return NotFound();

            investorModel.Contacted = investor.Contacted;
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("activity")]
        public async Task<IActionResult> ActivityAnalytics(string userId = null)
        {
            IQueryable<ActivityLog> query = _dbContext.ActivityLogs;
            if(!string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.UserId == userId);
            }

            var activities = await query.Where(x => x.Timestamp >= DateTime.Now.AddDays(-7)).AsNoTracking().ToArrayAsync();

            var breakdown = activities.GroupBy(x => x.Type)
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
            int monthly = 0;
            int weekly = 0;
            int total = 0;

            if (await _context.Subscribers.AnyAsync())
            {
                var firstDate = await _context.Subscribers.OrderBy(x => x.Timestamp).FirstOrDefaultAsync();
                double months = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 30;
                double weeks = (DateTimeOffset.Now - firstDate.Timestamp).TotalDays / 7;
                total = await _context.Subscribers.CountAsync();

                monthly = (int)months switch
                {
                    <= 0 => total,
                    _ => total / (int)months
                };

                weekly = (int)weeks switch
                {
                    <= 0 => total,
                    _ => total / (int)weeks
                };
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
