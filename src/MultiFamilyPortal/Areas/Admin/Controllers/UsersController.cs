using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = PortalRoles.PortalAdministrator)]
    [ApiController]
    [Route("/api/admin/users")]
    public class UsersController : ControllerBase
    {
        private IMFPContext _dbContext { get; }
        private UserManager<SiteUser> _userManager { get; }

        public UsersController(IMFPContext dbContext, UserManager<SiteUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var roles = await _dbContext.Roles.ToDictionaryAsync(x => x.Id, x => x.Name);
            var userRoles = await _dbContext.UserRoles.AsNoTracking().ToArrayAsync();

            var users = await _dbContext.Users
                .Select(x => new UserAccountResponse {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Id = x.Id,
                    Phone = x.PhoneNumber
                })
                .ToArrayAsync();

            foreach(var user in users)
            {
                user.Roles = userRoles.Where(x => x.UserId == user.Id).Select(x => roles[x.RoleId]).ToArray();
            }

            return Ok(users.OrderBy(x => x.FirstName));
        }

        [Authorize(Policy = PortalPolicy.Underwriter)]
        [HttpGet("/investors")]
        public async Task<IActionResult> Investors()
        {
            var users = await _dbContext.Users
                .Where(x => x.Roles.Any(r => r.Role.Name == PortalRoles.Investor))
                .Select(x => new UserAccountResponse {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Id = x.Id,
                    Phone = x.PhoneNumber,
                    Roles = x.Roles.Select(r => r.Role.Name).ToArray()
                })
                .ToArrayAsync();

            return Ok(users);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email) || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = new SiteUser(request.Email) {
                Email = request.Email,
                EmailConfirmed = true,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                PhoneNumberConfirmed = true,
            };

            var result = await _userManager.CreateAsync(user, "helloW0rld!");
            if (!result.Succeeded)
                return NoContent();

            await _userManager.AddToRolesAsync(user, request.Roles);
            if(request.Roles.Contains(PortalRoles.PortalAdministrator) || request.Roles.Contains(PortalRoles.Underwriter))
            {
                var goals = new UnderwriterGoal
                {
                    UnderwriterId = user.Id,
                };
                await _dbContext.UnderwriterGoals.AddAsync(goals);
                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
