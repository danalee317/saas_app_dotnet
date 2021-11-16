using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace MultiFamilyPortal.Services
{
    public class BrandService : IBrandService
    {
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

        public BrandService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BrandService>();
        }

        public Task CreateIcons(string defaultIcon)
        {
            var fileInfo = new FileInfo(defaultIcon);

            if (!fileInfo.Exists)
            {
                throw new System.IO.FileNotFoundException("Default icon not found", defaultIcon);
            }

            try
            {
                foreach (var icon in _favicons)
                    CreateFavicon(fileInfo, icon.Value, icon.Key);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return Task.CompletedTask;
        }

        private void CreateFavicon(FileInfo file, int size, string name)
        {
            using var src = SKImage.FromEncodedData(System.IO.File.ReadAllBytes(file.FullName));
            var canvasMax = Math.Max(src.Width, src.Height);

            var info = new SKImageInfo(size, size, SKColorType.Rgba8888);
            using var output = SKImage.Create(info);
            src.ScalePixels(output.PeekPixels(), SKFilterQuality.High);
            var filePath = Path.Combine(file.DirectoryName, name);
            if (File.Exists(filePath)) File.Delete(filePath);
            
            using (var stream = System.IO.File.Create(filePath))
            {
                var type = Path.GetExtension(name).ToLower() == ".ico" ? SKEncodedImageFormat.Ico : SKEncodedImageFormat.Png;
                using var bitmap = SKBitmap.FromImage(output);
                bitmap.Encode(stream, type, 100);
            }
        }

        public async Task CreateImage(IFormFile file, string name, string path)
        {
            try
            {
                Directory.CreateDirectory(path);

                var fileExt = Path.GetExtension(file.FileName);
                var fileName = name + fileExt;
                var filePath = Path.Combine(path, fileName);

                if (File.Exists(filePath)) File.Delete(filePath);

                switch (fileExt.ToLower())
                {
                    case ".svg":
                        using (var stream = new FileStream(filePath, FileMode.Create))
                            await file.CopyToAsync(stream);
                        break;

                    default:
                        {
                            using var src = SKImage.FromEncodedData(file.OpenReadStream());
                            var canvasMax = Math.Max(src.Width, src.Height);
                            int size = canvasMax > 512 ? 512 : canvasMax;
                            var info = new SKImageInfo(size, size, SKColorType.Rgba8888);
                            using var output = SKImage.Create(info);
                            src.ScalePixels(output.PeekPixels(), SKFilterQuality.High);

                            using (var stream = System.IO.File.Create(filePath))
                            {
                                using var bitmap = SKBitmap.FromImage(output);
                                bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public (byte[] FileContents, int Height, int Width) Resize(byte[] fileContents,
        int maxWidth, int maxHeight,
        SKFilterQuality quality = SKFilterQuality.Medium)
        {
            using MemoryStream ms = new MemoryStream(fileContents);
            using SKBitmap sourceBitmap = SKBitmap.Decode(ms);

            int height = Math.Min(maxHeight, sourceBitmap.Height);
            int width = Math.Min(maxWidth, sourceBitmap.Width);

            using SKBitmap scaledBitmap = sourceBitmap.Resize(new SKImageInfo(width, height), quality);
            using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
            using SKData data = scaledImage.Encode();

            return (data.ToArray(), height, width);
        }
    }
}
