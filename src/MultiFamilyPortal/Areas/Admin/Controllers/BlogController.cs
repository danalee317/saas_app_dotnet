using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Roles = PortalRoles.BlogAuthor + "," + PortalRoles.PortalAdministrator)]
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
    }
}
