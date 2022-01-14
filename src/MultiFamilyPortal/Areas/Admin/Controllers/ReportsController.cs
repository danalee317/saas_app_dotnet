using System.IO.Compression;
using System.Net.Mime;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.AdminTheme.Services;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Policy = PortalPolicy.UnderwritingViewer)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private IMFPContext _dbContext { get; }
        private IReportGenerator _generator { get; }
        private IUnderwritingService _underwritingService { get; }

        public ReportsController(IMFPContext dbContext, IUnderwritingService underwritingService, IReportGenerator report)
        {
            _dbContext = dbContext;
            _underwritingService = underwritingService;
            _generator = report;
        }

        [HttpGet("full-report/{propertyId:guid}")]
        public async Task<IActionResult> GetFullReport(Guid propertyId)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);
            return NotFound();
        }

        [HttpGet("deal-summary/{propertyId:guid}")]
        public async Task<IActionResult> GetDealSummary(Guid propertyId)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);
            return NotFound();
        }

        [HttpGet("assumptions/{propertyId:guid}")]
        public async Task<IActionResult> GetAssumptions(Guid propertyId)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);
            return NotFound();
        }

        [HttpGet("cash-flow/{propertyId:guid}")]
        public async Task<IActionResult> GetCashFlow(Guid propertyId)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);
            return NotFound();
        }

        [HttpGet("lease-exposure/{propertyId:guid}")]
        public async Task<IActionResult> GetLeaseExposure(Guid propertyId)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);
            return NotFound();
        }

        [HttpGet("100k-projections/{propertyId:guid}")]
        public async Task<IActionResult> Get100kProjections(Guid propertyId)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);
            return NotFound();
        }

        [HttpGet("manager-report/{propertyId:guid}")]
        public async Task<IActionResult> GetManagerReport(Guid propertyId)
        {
            var result = await _generator.ManagersReturns(propertyId);
            if(result is null)
                return NotFound();
                
            if(result.Data?.Length == 0)
                return NotFound();

            return File(result.Data, result.MimeType, result.FileName);
        }

        [HttpGet("investment-tiers/{propertyId:guid}/{group}")]
        public async Task<IActionResult> GetInvestmentTiers(Guid propertyId, string group)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);
            return NotFound();
        }

        [HttpPost("investment-tiers/{propertyId:guid}/{group}")]
        public async Task<IActionResult> UpdateInvestmentTiers(Guid propertyId, string group, [FromBody] IEnumerable<UnderwritingInvestmentTier> investmentTiers)
        {
            return RedirectToAction(nameof(GetInvestmentTiers));
        }

        [HttpGet("rementor-underwriting-template/{propertyId:guid}")]
        public async Task<IActionResult> GetREMentorUnderwritingTemplate(Guid propertyId)
        {
            (var property, var files, var rootArchive) = await SetupREMentorFiles(propertyId);
            var filePath = GenerateREMentorUnderwritingTemplate(rootArchive, property, files);
            var fileName = Path.GetFileName(filePath);
            var fileInfo = FileTypeLookup.GetFileTypeInfo(fileName);
            return File(System.IO.File.ReadAllBytes(filePath), fileInfo.MimeType, fileName);
        }

        [HttpGet("rementor-underwriting-template-v2/{propertyId:guid}")]
        public async Task<IActionResult> GetREMentorUnderwritingTemplateV2(Guid propertyId)
        {
            (var property, var files, var rootArchive) = await SetupREMentorFiles(propertyId);
            var filePath = GenerateREMentorUnderwritingTemplateV2(rootArchive, property, files);
            var fileName = Path.GetFileName(filePath);
            var fileInfo = FileTypeLookup.GetFileTypeInfo(fileName);
            return File(System.IO.File.ReadAllBytes(filePath), fileInfo.MimeType, fileName);
        }

        [HttpGet("rementor-bucketlist/{propertyId:guid}")]
        public async Task<IActionResult> GetREMentorBucketlist(Guid propertyId)
        {
            (var property, var files, var rootArchive) = await SetupREMentorFiles(propertyId);

            var filePath = GenerateREMentorBucketlist(rootArchive, property);
            var fileName = Path.GetFileName(filePath);
            var fileInfo = FileTypeLookup.GetFileTypeInfo(fileName);
            return File(System.IO.File.ReadAllBytes(filePath), fileInfo.MimeType, fileName);
        }

        [HttpGet("rementor-templates/{propertyId:guid}")]
        public async Task<IActionResult> GetAllREMentorUnderwritingTemplates(Guid propertyId)
        {
            (var property, var files, var rootArchive) = await SetupREMentorFiles(propertyId);

            var fileName = $"{property.Name}.zip";
            GenerateREMentorUnderwritingTemplate(rootArchive, property, files);
            GenerateREMentorUnderwritingTemplateV2(rootArchive, property, files);
            GenerateREMentorBucketlist(rootArchive, property);

            var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.zip");
            ZipFile.CreateFromDirectory(rootArchive, filePath, CompressionLevel.Fastest, false);

            return File(System.IO.File.ReadAllBytes(filePath), MediaTypeNames.Application.Zip, fileName);
        }

        private string GenerateREMentorUnderwritingTemplate(string rootArchive, UnderwritingAnalysis property, IEnumerable<UnderwritingAnalysisFile> files)
        {
            var data = UnderwritingService.GenerateUnderwritingSpreadsheet(property, files);
            var filePath = Path.Combine(rootArchive, $"{property.Name}.xlsx");
            System.IO.File.WriteAllBytes(filePath, data);
            return filePath;
        }

        private string GenerateREMentorUnderwritingTemplateV2(string rootArchive, UnderwritingAnalysis property, IEnumerable<UnderwritingAnalysisFile> files)
        {
            var data = UnderwritingV2Service.GenerateUnderwritingSpreadsheet(property, files);
            var filePath = Path.Combine(rootArchive, $"{property.Name}-v2.xlsx");
            System.IO.File.WriteAllBytes(filePath, data);
            return filePath;
        }

        private string GenerateREMentorBucketlist(string rootArchive, UnderwritingAnalysis property)
        {
            var data = REMentorBucketListService.GenerateBucketlist(property);
            var filePath = Path.Combine(rootArchive, $"{property.Name}-Bucketlist.xlsx");
            System.IO.File.WriteAllBytes(filePath, data);
            return filePath;
        }

        private async Task<(UnderwritingAnalysis property, IEnumerable<UnderwritingAnalysisFile> files, string rootArchive)> SetupREMentorFiles(Guid propertyId)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

            var files = await GetAnalysisFiles(propertyId);
            var rootArchive = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(rootArchive);

            return (property, files, rootArchive);
        }

        private async Task<IEnumerable<UnderwritingAnalysisFile>> GetAnalysisFiles(Guid propertyId)
        {
            var host = $"{Request.Scheme}://{Request.Host}";
            return await _dbContext.UnderwritingProspectFiles
                .Where(x => x.PropertyId == propertyId)
                .Select(x => new UnderwritingAnalysisFile
                {
                    Id = x.Id,
                    Type = x.Type.Humanize(LetterCasing.Title),
                    Description = x.Description,
                    DownloadLink = $"{host}/api/files/property/{propertyId}/file/{x.Id}",
                    Icon = x.Icon,
                    Name = x.Name,
                    Timestamp = x.Timestamp,
                })
                .ToArrayAsync();
        }
    }
}
