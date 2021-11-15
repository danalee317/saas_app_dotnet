using Microsoft.AspNetCore.Http;

namespace MultiFamilyPortal.Services
{
    public interface IBrandService
    {
        Task CreateIcons(string name);
        Task CreateImage(IFormFile file, string name, string path);
    }
}