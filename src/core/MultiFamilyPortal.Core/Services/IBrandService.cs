using Microsoft.AspNetCore.Http;

namespace MultiFamilyPortal.Services
{
    public interface IBrandService
    {
        Task CreateIcons(Stream stream, string destinationPath);
        Task CreateImage(IFormFile file, string name, string path);
    }
}
