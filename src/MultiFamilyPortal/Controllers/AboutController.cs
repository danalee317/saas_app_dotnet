using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("/api/[controller]")]
    public class AboutController : ControllerBase
    {
        private IMFPContext _dbContext { get; }

        public AboutController(IMFPContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> GetHighlightedUsers()
        {
            var users = await _dbContext.HighlightedUsers
                .Include(x => x.User)
                    .ThenInclude(x => x.SocialLinks)
                        .ThenInclude(x => x.SocialProvider)
                .ToListAsync();

            var response = users.OrderBy(x => x.Order)
                .Select(x => new HighlightedUserResponse
                {
                    Bio = x.User.Bio,
                    DisplayName = x.User.DisplayName,
                    Email = x.User.Email,
                    Phone = x.User.PhoneNumber,
                    Links = x.User.SocialLinks.Select(s => new SocialLinkResponse
                    {
                        Icon = s.SocialProvider.Icon,
                        Name = s.SocialProvider.Name,
                        Link = s.Uri.ToString()
                    })
                });

            return Ok(response);
        }
    }
}
