using Microsoft.AspNetCore.Authorization;
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
            var Png = $"{name}.png";
            var Svg = $"{name}.svg";
            var savedPath =  Path.Combine(_env.ContentRootPath, "App_Data","Brands");
            var defaultFile = Path.Combine(_env.WebRootPath, "default-resources", "logo");

            var pngInfo = FileTypeLookup.GetFileTypeInfo(Png);
            var svgInfo = FileTypeLookup.GetFileTypeInfo(Svg);

            if (SysFile.Exists(Path.Combine(savedPath, Png)))
                return PhysicalFile(Path.Combine(savedPath, Png), pngInfo.MimeType);
            
            else if(SysFile.Exists(Path.Combine(savedPath, Svg)))
                return PhysicalFile(Path.Combine(savedPath, Svg), svgInfo.MimeType);
            
            else if(SysFile.Exists(Path.Combine(defaultFile, Png)))
                return PhysicalFile(Path.Combine(defaultFile, Png), pngInfo.MimeType);
            
            else if(SysFile.Exists(Path.Combine(defaultFile, Svg)))
                return PhysicalFile(Path.Combine(defaultFile, Svg), svgInfo.MimeType);
            
            else
                return NotFound();    
        }
    }
}
