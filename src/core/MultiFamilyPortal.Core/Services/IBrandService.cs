using Microsoft.AspNetCore.Http;

namespace MultiFamilyPortal.Services
{
    public interface IBrandService
    {
        Task CreateIcons(string SourcePath, string DestinationPath);
        Task CreateImage(IFormFile file, string name, string path);
    }
}