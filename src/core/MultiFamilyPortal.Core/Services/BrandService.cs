using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace MultiFamilyPortal.Services
{
    public class BrandService : IBrandService
    {
        private const string Icons = nameof(Icons);
        private const string Brand = nameof(Brand);

        private readonly Dictionary<string, int> _favicons = new ()
        {
             { "favicon.ico", 16 },
             { "favicon-16x16.png", 16 },
             { "favicon-32x32.png", 32 },
             { "favicon-96x96.png", 96 },
             { "android-chrome-192x192.png", 128 },
             { "android-chrome-288x288.png", 288 },
             { "android-chrome-512x512.png", 512 },
             { "apple-touch-icon.png", 180 },
        };

        private ILogger<BrandService> _logger { get; }
        private IStorageService _storage { get; }
        private IWebHostEnvironment _hostEnvironment { get; }

        public BrandService(ILogger<BrandService> logger, IStorageService storageService, IWebHostEnvironment hostEnvironment)
        {
            _logger = logger;
            _storage = storageService;
            _hostEnvironment = hostEnvironment;
        }

        public async Task CreateDefaultIcons()
        {
            if (await _storage.ExistsAsync(Path.Combine(Icons, "favicon.ico")))
                return;

            var defaultFile = Path.Combine(_hostEnvironment.WebRootPath, "default-resources", "favicon", "default.png");
            using var defaultFileStream = File.OpenRead(defaultFile);
            await CreateIcons(defaultFileStream);
        }

        public async Task CreateIcons(Stream stream)
        {
            if (stream is null || stream == Stream.Null)
                throw new ArgumentNullException(nameof(stream));

            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var data = memoryStream.ToArray();

            try
            {
                foreach (var icon in _favicons)
                    await CreateFavicon(data, icon.Value, icon.Key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        private async Task CreateFavicon(byte[] data, int size, string name)
        {
            using var src = SKImage.FromEncodedData(data);
            var canvasMax = Math.Max(src.Width, src.Height);

            var info = new SKImageInfo(size, size, SKColorType.Rgba8888);
            using var output = SKImage.Create(info);
            src.ScalePixels(output.PeekPixels(), SKFilterQuality.High);
            var filePath = Path.Combine(Icons, name);

            using var stream = new MemoryStream();
            var type = Path.GetExtension(name).ToLower() == ".ico" ? SKEncodedImageFormat.Ico : SKEncodedImageFormat.Png;
            using var bitmap = SKBitmap.FromImage(output);
            bitmap.Encode(stream, type, 100);

            var fileTypeInfo = FileTypeLookup.GetFileTypeInfo(name);

            if(stream.Length > 0)
                await _storage.PutAsync(filePath, stream, fileTypeInfo.MimeType, overwrite: true);
        }

        public async Task CreateBrandImage(IFormFile file, string name)
        {
            try
            {
                var fileExt = Path.GetExtension(file.FileName);
                if (fileExt == ".jpeg")
                    fileExt = ".jpg";
                else if (!(fileExt == ".svg" || fileExt == ".png"))
                    fileExt = ".png";

                var fileName = name + fileExt;
                var filePath = Path.Combine(Brand, fileName);

                switch (fileExt.ToLower())
                {
                    case ".svg":
                        using (var stream = file.OpenReadStream())
                            await _storage.PutAsync(filePath, stream, FileTypeLookup.GetFileTypeInfo("image.svg").MimeType, overwrite: true);
                        break;

                    default:
                        var format = fileExt == ".jpg" ? SKEncodedImageFormat.Jpeg : SKEncodedImageFormat.Png;
                        using (var src = SKImage.FromEncodedData(file.OpenReadStream()))
                        {
                            var canvasMax = Math.Max(src.Width, src.Height);
                            int size = canvasMax > 512 ? 512 : canvasMax;
                            var info = new SKImageInfo(size, size, SKColorType.Rgba8888);
                            using var output = SKImage.Create(info);
                            src.ScalePixels(output.PeekPixels(), SKFilterQuality.High);

                            using var stream = new MemoryStream();
                            using var bitmap = SKBitmap.FromImage(output);
                            bitmap.Encode(stream, format, 100);
                            var fileTypeInfo = FileTypeLookup.GetFileTypeInfo(fileName);
                            await _storage.PutAsync(filePath, stream, fileTypeInfo.MimeType, overwrite: true);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public async Task<Stream> GetIcon(string fileName)
        {
            try
            {
                var stream = await _storage.GetAsync(Path.Combine(Icons, fileName));
                if (!(stream is null || stream == Stream.Null))
                {
                    return stream;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Brand Icon: '{fileName}'.");
            }

            return Stream.Null;
        }

        public async Task<(Stream Stream, string MimeType, string FileName)> GetBrandImage(string name)
        {
            try
            {
                var png = $"{name}.png";
                var svg = $"{name}.svg";
                var jpg = $"{name}.jpg";

                var savedPng = Path.Combine(Brand, png);
                var savedSvg = Path.Combine(Brand, svg);
                var savedJpg = Path.Combine(Brand, jpg);

                var jpgInfo = FileTypeLookup.GetFileTypeInfo(jpg);
                var pngInfo = FileTypeLookup.GetFileTypeInfo(png);
                var svgInfo = FileTypeLookup.GetFileTypeInfo(svg);

                if (await _storage.ExistsAsync(savedPng))
                    return (await _storage.GetAsync(savedPng), pngInfo.MimeType, png);

                else if (await _storage.ExistsAsync(savedSvg))
                    return (await _storage.GetAsync(savedSvg), svgInfo.MimeType, svg);

                else if (await _storage.ExistsAsync(savedJpg))
                    return (await _storage.GetAsync(savedJpg), jpgInfo.MimeType, jpg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting Brand Image: '{name}'.");
            }

            return (Stream.Null, string.Empty, string.Empty);
        }

        //private (byte[] FileContents, int Height, int Width) Resize(byte[] fileContents, int maxWidth, int maxHeight, SKFilterQuality quality = SKFilterQuality.Medium)
        //{
        //    using MemoryStream ms = new MemoryStream(fileContents);
        //    using SKBitmap sourceBitmap = SKBitmap.Decode(ms);

        //    int height = Math.Min(maxHeight, sourceBitmap.Height);
        //    int width = Math.Min(maxWidth, sourceBitmap.Width);

        //    using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), quality);
        //    using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
        //    using SKData data = scaledImage.Encode();

        //    return (data.ToArray(), height, width);
        //}
    }
}
