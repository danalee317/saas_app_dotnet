using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiFamilyPortal.Data;
using SkiaSharp;

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
            GetImage("favicon.ico", 16, true);

        [HttpGet("/favicon-16x16.png")]
        public IActionResult GetFavIcon16() =>
            GetImage("favicon-16x16.png", 16);

        [HttpGet("/favicon-32x32.png")]
        public IActionResult GetFavIcon32() =>
            GetImage("favicon-32x32.png", 32);

        [HttpGet("/android-chrome-192x192.png")]
        public IActionResult AndroidChrome192() =>
            GetImage("android-chrome-192x192.png", 192);

        [HttpGet("/android-chrome-512x512.png")]
        public IActionResult AndroidChrome512() =>
            GetImage("android-chrome-512x512.png", 512);

        [HttpGet("/apple-touch-icon.png")]
        public IActionResult GetAppleTouch() =>
            GetImage("apple-touch-icon.png", 180);

        [HttpGet("/site.webmanifest")]
        public async Task<IActionResult> SiteWebManifest([FromServices]IMFPContext db)
        {
            var title = await db.GetSettingAsync<string>(PortalSetting.SiteTitle);
            var legalName = await db.GetSettingAsync<string>(PortalSetting.LegalBusinessName);

            if(string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(legalName))
            {
                title = legalName;
            }
            else if(!string.IsNullOrEmpty(title) && string.IsNullOrEmpty(legalName))
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

        private IActionResult GetImage(string name, int defaultSize, bool iconFile = false)
        {
            var filePath = Path.Combine(_hostEnvironment.ContentRootPath, "App_Data", name);
            var fileInfo = new FileInfo(filePath);
            if(!fileInfo.Exists)
            {
                fileInfo.Directory.Create();

                var defaultPath = Path.Combine(_hostEnvironment.WebRootPath, "default-resources", "favicon", "default.png");
                using var src = SKImage.FromEncodedData(System.IO.File.ReadAllBytes(defaultPath));
                var canvasMax = Math.Max(src.Width, src.Height);

                var info = new SKImageInfo(defaultSize, defaultSize, SKColorType.Rgba8888);
                using var output = SKImage.Create(info);
                src.ScalePixels(output.PeekPixels(), SKFilterQuality.High);

                using (var stream = System.IO.File.Create(filePath))
                {
                    var type = iconFile ? SKEncodedImageFormat.Ico : SKEncodedImageFormat.Png;
                    using var bitmap = SKBitmap.FromImage(output);
                    bitmap.Encode(stream, type, 100);
                }
            }

            var contentType = iconFile ? "image/x-icon" : "image/png";
            return File(System.IO.File.ReadAllBytes(filePath), contentType, name);
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
