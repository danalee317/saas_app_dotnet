using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiFamilyPortal.Data;

namespace MultiFamilyPortal.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class FavIconController : ControllerBase
    {
        private IWebHostEnvironment _hostEnvironment { get; }

        public FavIconController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("/favicon.ico")]
        public IActionResult GetFavIcon() =>
            GetImage("favicon.ico");

        [HttpGet("/favicon-16x16.png")]
        public IActionResult GetFavIcon16() =>
            GetImage("favicon-16x16.png");

        [HttpGet("/favicon-32x32.png")]
        public IActionResult GetFavIcon32() =>
            GetImage("favicon-32x32.png");

        [HttpGet("/android-chrome-192x192.png")]
        public IActionResult AndroidChrome192() =>
            GetImage("android-chrome-192x192.png");

        [HttpGet("/android-chrome-512x512.png")]
        public IActionResult AndroidChrome512() =>
            GetImage("android-chrome-512x512.png");

        [HttpGet("/apple-touch-icon.png")]
        public IActionResult GetAppleTouch() =>
            GetImage("apple-touch-icon.png");

        [HttpGet("/site.webmanifest")]
        public async Task<IActionResult> SiteWebManifest([FromServices] IMFPContext db)
        {
            var title = await db.GetSettingAsync<string>(PortalSetting.SiteTitle);
            var legalName = await db.GetSettingAsync<string>(PortalSetting.LegalBusinessName);

            if (string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(legalName))
            {
                title = legalName;
            }
            else if (!string.IsNullOrEmpty(title) && string.IsNullOrEmpty(legalName))
            {
                legalName = title;
            }
            else
            {
                title = legalName = "Multi-Family Portal";
            }

            var manifest = new WebManifest
            {
                BackgroundColor = await db.GetSettingAsync(PortalSetting.BackgroundColor, "#ffffff"),
                ThemeColor = await db.GetSettingAsync(PortalSetting.ThemeColor, "#ffffff"),
                Name = legalName,
                ShortName = title,
                Icons = new[]
                {
                    new Icon { Source = "/android-chrome-192x192.png", Sizes = "192x192" },
                    new Icon { Source = "/android-chrome-512x512.png", Sizes = "512x512" }
                }
            };
            var json = JsonSerializer.Serialize(manifest);
            var bytes = Encoding.Default.GetBytes(json);
            return File(bytes, "application/json", "site.webmanifest");
        }

        private IActionResult GetImage(string name)
        {
            var typeInfo = FileTypeLookup.GetFileTypeInfo(name);
            try
            {
                var savedPath = Path.Combine(_hostEnvironment.ContentRootPath, "App_Data", "Icons");
                return File(System.IO.File.ReadAllBytes(Path.Combine(savedPath, name)), typeInfo.MimeType, name);
            }
            catch
            {
                var defaultFile = Path.Combine(_hostEnvironment.WebRootPath, "default-resources", "favicon");
                return File(System.IO.File.ReadAllBytes(Path.Combine(defaultFile, "default.png")), typeInfo.MimeType, name);
            }
        }

        private record WebManifest
        {
            [JsonPropertyName("name")]
            public string Name { get; init; } = default!;

            [JsonPropertyName("short_name")]
            public string ShortName { get; init; } = default!;

            [JsonPropertyName("theme_color")]
            public string ThemeColor { get; init; } = default!;

            [JsonPropertyName("background_color")]
            public string BackgroundColor { get; init; } = default!;

            [JsonPropertyName("display")]
            public string Display => "standalone";

            [JsonPropertyName("icons")]
            public IEnumerable<Icon> Icons { get; init; } = default!;
        }

        private record Icon
        {
            [JsonPropertyName("src")]
            public string Source { get; init; } = default!;

            [JsonPropertyName("sizes")]
            public string Sizes { get; init; } = default!;

            [JsonPropertyName("type")]
            public string Type => "image/png";
        }
    }
}
