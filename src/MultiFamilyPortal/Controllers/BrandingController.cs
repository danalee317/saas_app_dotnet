using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SysFile = System.IO.File;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [Route("theme/[controller]")]
    [ApiController]
    public class BrandingController : ControllerBase
    {
        private IWebHostEnvironment _env { get; }

        public BrandingController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("logo")]
        public IActionResult Logo() => Get("logo");

        [HttpGet("logo-side")]
        public IActionResult LogoSide() => Get("logo-side");

        [HttpGet("logo-dark")]
        public IActionResult LogoDark() => Get("logo-dark");

        [HttpGet("logo-dark-side")]
        public IActionResult LogoDarkSide() => Get("logo-dark-side");

        private IActionResult Get(string name)
        {
            var expectedPng = Path.Combine(_env.WebRootPath, "branding", $"{name}.png");
            var expectedSvg = Path.Combine(_env.WebRootPath, "branding", $"{name}.svg");

            var defaultSvg = Path.Combine(_env.WebRootPath, "default-resources", "logo", $"{name}.svg");

            if(SysFile.Exists(expectedPng))
            {
                return PhysicalFile(expectedPng, "image/png");
            }
            else if(SysFile.Exists(expectedSvg))
            {
                return PhysicalFile(expectedSvg, "image/svg+xml");
            }
            else if(SysFile.Exists(defaultSvg))
            {
                return PhysicalFile(defaultSvg, "image/svg+xml");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
