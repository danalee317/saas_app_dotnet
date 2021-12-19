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

        [HttpGet("crm-roles")]
        public async Task<IActionResult> GetCrmRoles()
        {
            var roles = await _dbContext.CrmContactRoles.ToArrayAsync();
            return Ok(roles);
        }

        [HttpPost("crm-role/create")]
        public async Task<IActionResult> CreateCrmRole([Bind("Name", "CoreTeam")]CRMContactRole role)
        {
            if (string.IsNullOrEmpty(role.Name))
                return BadRequest();
            else if (await _dbContext.CrmContactRoles.AnyAsync(x => x.Name == role.Name))
                return Conflict();

            await _dbContext.CrmContactRoles.AddAsync(role);
            await _dbContext.SaveChangesAsync();
            return StatusCode(201);
        }

        [HttpPut("crm-role/update/{id:guid}")]
        public async Task<IActionResult> UpdateCrmRole(Guid id, [Bind("Id", "Name", "CoreTeam")]CRMContactRole updated)
        {
            if (id != updated.Id || string.IsNullOrEmpty(updated.Name))
                return BadRequest();

            var role = await _dbContext.CrmContactRoles.FirstOrDefaultAsync(x => x.Id == id && x.SystemDefined == false);
            if (role == null)
                return NotFound();

            role.Name = updated.Name;
            role.CoreTeam = updated.CoreTeam;
            _dbContext.CrmContactRoles.Update(role);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("crm-role/delete/{id:guid}")]
        public async Task<IActionResult> DeleteCrmRole(Guid id)
        {
            if (id == default)
                return BadRequest();

            var role = await _dbContext.CrmContactRoles.FirstOrDefaultAsync(x => x.Id == id && x.SystemDefined == false);
            if (role is null)
                return Ok();

            _dbContext.CrmContactRoles.Remove(role);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("crm-contacts")]
        public async Task<IActionResult> GetCrmContacts()
        {
            var contacts = await _dbContext.CrmContacts
                .Include(x => x.Emails)
                .Include(x => x.Phones)
                .Include(x => x.Roles)
                .ToArrayAsync();

            return Ok(contacts);
        }
    }
}
