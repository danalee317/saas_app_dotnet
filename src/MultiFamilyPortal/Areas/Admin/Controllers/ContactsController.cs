using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = PortalPolicy.Underwriter)]
    [ApiController]
    [Route("/api/[area]/[controller]")]
    public class ContactsController : ControllerBase
    {
        private ICRMContext _dbContext { get; }

        public ContactsController(ICRMContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("investors")]
        public async Task<IActionResult> Investors()
        {
            var investorRole = await _dbContext.Roles.Where(x => x.Name == PortalRoles.Investor).Select(x => x.Id).FirstAsync();
            var sponsorRole = await _dbContext.Roles.Where(x => x.Name == PortalRoles.Sponsor).Select(x => x.Id).FirstAsync();
            var mapping = new Dictionary<string, string>
            {
                { investorRole, PortalRoles.Investor },
                { sponsorRole, PortalRoles.Sponsor },
            };

            var userIds = await _dbContext.UserRoles.Where(x => x.RoleId == investorRole || x.RoleId == sponsorRole)
                .ToArrayAsync();
            var grouped = userIds.GroupBy(x => x.UserId);
            var users = new List<UserAccountResponse>();
            foreach (var user in grouped)
            {
                var userAccount = await _dbContext.Users
                    .Select(x => new UserAccountResponse
                    {
                        Email = x.Email,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Id = x.Id,
                        Phone = x.PhoneNumber,
                        LocalAccount = string.IsNullOrEmpty(x.PasswordHash) == false,
                    })
                    .FirstAsync(x => x.Id == user.Key);

                userAccount.Roles = user.Select(x => mapping[x.RoleId]).ToArray();
                users.Add(userAccount);
            }

            return Ok(users);
        }

        [HttpGet("investors/prospects")]
        public async Task<IActionResult> GetInvestorProspects()
        {
            var prospects = await _dbContext.InvestorProspects.ToArrayAsync();
            return Ok(prospects);
        }
    }
}
