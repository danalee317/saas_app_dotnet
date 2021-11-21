using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Services;

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
                .Select(x => new UserAccountResponse
                {
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Id = x.Id,
                    Phone = x.PhoneNumber,
                    LocalAccount = string.IsNullOrEmpty(x.PasswordHash) == false
                })
                .ToArrayAsync();

            foreach(var user in users)
            {
                user.Roles = userRoles.Where(x => x.UserId == user.Id).Select(x => roles[x.RoleId]).ToArray();
            }

            return Ok(users.OrderBy(x => x.FirstName));
        }

        [Authorize(Policy = PortalPolicy.Underwriter)]
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
            foreach(var user in grouped)
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

        [HttpGet("subscribers")]
        public async Task<IActionResult> GetSubscribers()
        {
            var subscribers = await ((IBlogContext)_dbContext).Subscribers.ToArrayAsync();
            return Ok(subscribers);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserRequest request, [FromServices] IEmailService emailSender, [FromServices] ITemplateProvider templateProvider)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email) || !ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = new SiteUser(request.Email)
            {
                Email = request.Email,
                EmailConfirmed = true,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.Phone,
                PhoneNumberConfirmed = true,
            };

            var password = "";
            var info = "Use your Microsoft or Google Account to login";
            var tip = "";
            var actionPoint = "<b>If you are not aware of this action, ignore this message.</b>";
            IdentityResult result = null;
            if (request.UseLocalAccount)
            {
                try
                {
                    HttpClient _client = new HttpClient();
                    var response = await _client.GetAsync("https://www.passwordrandom.com/query?command=password");
                    password = await response.Content.ReadAsStringAsync();
                    result = await _userManager.CreateAsync(user, password);
                }
                catch  (Exception ex)
                {
                    return StatusCode(500,ex);
                }

                info=$"Your password is <b> {password}</b>";
                tip="You can change your password in your profile.";
            }
            else
            {
                result = await _userManager.CreateAsync(user);
            }

            if (!result.Succeeded)
                return BadRequest();

            await _userManager.AddToRolesAsync(user, request.Roles);
            if (request.Roles.Contains(PortalRoles.PortalAdministrator) || request.Roles.Contains(PortalRoles.Underwriter))
            {
                var goals = new UnderwriterGoal
                {
                    UnderwriterId = user.Id,
                };
                await _dbContext.UnderwriterGoals.AddAsync(goals);
                await _dbContext.SaveChangesAsync();
            }

            // TODO: Email User confirmation 
            if (result.Succeeded)
            {
                var siteTitle = await _dbContext.GetSettingAsync<string>(PortalSetting.SiteTitle);
                var notification = new ContactFormEmailNotification
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Message = $"<p>A new account has been created at https://{HttpContext.Request.Host} with your email address. {info} .<br/>{tip}<br/> {actionPoint}</p>",
                    SiteTitle = siteTitle,
                    SiteUrl = $"https://{HttpContext.Request.Host}",
                    Subject = $"{siteTitle} - New Account Created",
                    Year = DateTime.Now.Year,
                };
                var template = await templateProvider.GetTemplate(PortalTemplate.ContactMessage, notification);
                await emailSender.SendAsync(user.Email, template);
            }

            return Ok();
        }
    }
}
