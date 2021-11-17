using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
    [Authorize(Policy = PortalPolicy.UnderwritingViewer)]
    [ApiController]
    [Route("/api/[area]/[controller]")]
    public class ActivityController : ControllerBase
    {
        private IMFPContext _dbContext { get; }

        public ActivityController(IMFPContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetActivities(DateTimeOffset start, DateTimeOffset end, string profileId = null)
        {
            var query = (IQueryable<ActivityLog>)_dbContext.ActivityLogs;
            if(!string.IsNullOrEmpty(profileId))
            {
                query = query.Where(x => x.UserId == profileId);
            }

            var activities = await query
                .Select(x => new ActivityResponse
                {
                    Date = x.Date,
                    Description = x.Description,
                    Id = x.Id,
                    Timestamp = x.Timestamp,
                    Total = x.Total,
                    Type = x.Type,
                    UserEmail = x.User.Email,
                    UserName = x.User.DisplayName
                })
                .Where(x => x.Date > start && x.Date < end)
                .ToArrayAsync();

            return Ok(activities);
        }

        [Authorize(Policy = PortalPolicy.Underwriter)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateActivity([FromBody]CreateActivityRequest request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user is null)
                return Unauthorized();

            var activity = new ActivityLog {
                Date = request.Date,
                Description = request.Description,
                Total = request.Total,
                Type = request.Type,
                UserId = user.Id
            };

            await _dbContext.ActivityLogs.AddAsync(activity);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = PortalPolicy.Underwriter)]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateActivity([FromBody]UpdateActivityRequest request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user is null)
                return Unauthorized();

            var activity = await _dbContext.ActivityLogs.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (activity is null)
                return BadRequest();

            activity.Date = request.Date;
            activity.Description = request.Description;
            activity.Total = request.Total;
            activity.Type = request.Type;
            _dbContext.ActivityLogs.Update(activity);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Policy = PortalPolicy.Underwriter)]
        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> DeleteActivity(Guid id)
        {
            var activity = await _dbContext.ActivityLogs.FirstOrDefaultAsync(x => x.Id == id);
            if (activity is null)
                return NotFound();

            var user = _dbContext.Users.FirstOrDefault(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            if (activity.UserId != user.Id)
                return Unauthorized();

            _dbContext.ActivityLogs.Remove(activity);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
