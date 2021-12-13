using System.Data;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.AdminTheme.Services;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Extensions;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Policy = PortalPolicy.UnderwritingViewer)]
    [ApiController]
    [Route("/api/admin/underwriting")]
    public class UnderwritingController : ControllerBase
    {
        private IMFPContext _dbContext { get; }
        private IUnderwritingService _underwritingService { get; }
        private ILogger _logger { get; }

        public UnderwritingController(IMFPContext dbContext, IUnderwritingService underwritingService, ILogger<UnderwritingController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _underwritingService = underwritingService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]DateTimeOffset start, [FromQuery]DateTimeOffset end, [FromQuery]string underwriterId = null)
        {
            if (start == default)
                start = DateTimeOffset.Now.AddMonths(-1);

            if (end == default)
                end = DateTimeOffset.Now;

            try
            {
                if (!await _dbContext.UnderwritingPropertyProspects.Where(x => x.Timestamp.Date >= start && x.Timestamp.Date <= end.Date).AnyAsync())
                    return Ok(Array.Empty<ProspectPropertyResponse>());

                var query = _dbContext.UnderwritingPropertyProspects
                    .Include(x => x.Underwriter)
                    .Where(x => x.Timestamp.Date >= start.Date && x.Timestamp.Date <= end.Date);

                if (!string.IsNullOrEmpty(underwriterId))
                {
                    query = query.Where(x => x.UnderwriterId == underwriterId);
                }

                var response = await query.Select(x => new ProspectPropertyResponse
                {
                    CapRate = x.CapRate,
                    City = x.City,
                    CoC = x.CashOnCash,
                    Created = x.Timestamp,
                    DebtCoverage = x.DebtCoverage,
                    Id = x.Id,
                    Name = x.Name,
                    State = x.State,
                    Status = x.Status,
                    Underwriter = x.Underwriter.DisplayName,
                    UnderwriterEmail = x.Underwriter.Email,
                    Units = x.Units,
                }).ToArrayAsync();
                return Ok(response.OrderByDescending(x => x.Created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.GetType().Name} encountered while fetching the Prospect Properties");
                return BadRequest(ex);
            }
        }

        [HttpGet("underwriters")]
        public async Task<IActionResult> GetUnderwriters()
        {
            var roles = await _dbContext.Roles
                .Where(x => x.Name == PortalRoles.Underwriter || x.Name == PortalRoles.PortalAdministrator)
                .ToArrayAsync();
            var underwriterRole = roles.FirstOrDefault(x => x.Name == PortalRoles.Underwriter);
            var adminRole = roles.FirstOrDefault(x => x.Name == PortalRoles.PortalAdministrator);
            var userIds = await _dbContext.UserRoles
                .Where(x => x.RoleId == underwriterRole.Id || x.RoleId == adminRole.Id)
                .Select(x => x.UserId)
                .Distinct()
                .ToArrayAsync();

            var underwriters = new List<UnderwriterResponse>();
            foreach(var userId in userIds)
            {
                var user = await _dbContext.Users
                .Select(x => new UnderwriterResponse
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DisplayName = x.DisplayName,
                    Email = x.Email,
                    Id = x.Id,
                })
                .FirstOrDefaultAsync(x => x.Id == userId);
                underwriters.Add(user);
            }

            return Ok(underwriters.OrderBy(x => x.FirstName));
        }

        [HttpGet("guidance")]
        public async Task<IActionResult> GetGuidance([FromQuery]string market)
        {
            var guidance = await _dbContext.UnderwritingGuidance
                .ToArrayAsync();

            if(string.IsNullOrEmpty(market))
                return Ok(guidance.OrderBy(x => x.Market));
            
            var marketGuidance = guidance.Where(x => IsMarket(x.Market, market));

            if (marketGuidance is null || !marketGuidance.Any())
                marketGuidance = guidance.Where(x => string.IsNullOrEmpty(x.Market));
               
            return Ok(marketGuidance);
        }

        [HttpPost("guidance")]
        public async Task<IActionResult> CreateGuidance(UnderwritingGuidance guidance)
        {
            if(string.IsNullOrEmpty(guidance.Market))
                    return BadRequest();

            var existing = await _dbContext.UnderwritingGuidance
                                           .Where(x => x.Market.ToLower() == guidance.Market.Trim().ToLower() && 
                                           x.Category == guidance.Category && 
                                           x.Type == guidance.Type)
                                           .FirstOrDefaultAsync();

            if (existing is not null)
                return BadRequest();

            guidance.Id = Guid.NewGuid();
            guidance.Market = guidance.Market.Trim();
            await _dbContext.UnderwritingGuidance.AddAsync(guidance);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("guidance/{id}")]
        public async Task<IActionResult> EditGuidance(Guid id, [FromBody] UnderwritingGuidance guidance)
        {
            if(string.IsNullOrEmpty(guidance.Market) || id == Guid.Empty || id != guidance.Id)
                    return BadRequest();

            var existing = await _dbContext.UnderwritingGuidance.FirstOrDefaultAsync(x => x.Id == id);

            if (existing is null)
                return NotFound();

            existing.Market = guidance.Market.Trim();
            existing.Category = guidance.Category;
            existing.Type = guidance.Type;
            existing.Min = guidance.Min;
            existing.Max = guidance.Max;
            existing.IgnoreOutOfRange = guidance.IgnoreOutOfRange;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("guidance/{id}")]
        public async Task<IActionResult> DeleteGuidance(Guid id)
        {
            var existing = await _dbContext.UnderwritingGuidance
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existing is null)
                return NotFound();

            _dbContext.UnderwritingGuidance.Remove(existing);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("markets")]
        public async Task<IActionResult> GetMarkets()
        {
            var markets = await _dbContext.UnderwritingPropertyProspects
                .Select(x => x.Market)
                .Distinct()
                .ToListAsync();

            markets.AddRange(await _dbContext.UnderwritingGuidance.Select(x => x.Market).ToArrayAsync());

            return Ok(markets.Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
        }

        [Authorize(Policy = PortalPolicy.Underwriter)]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]CreateUnderwritingPropertyRequest property)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            var startDate = DateTimeOffset.Now.AddMonths(3);
            while(startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                startDate = startDate.AddDays(1);
            }

            var prospect = new UnderwritingProspectProperty
            {
                Name = property.Name,
                Units = property.Units,
                Address = property.Address,
                City = property.City,
                State = property.State,
                Zip = property.Zip,
                Vintage = property.Vintage,
                Market = property.Market,
                UnderwriterId = user.Id,
                RentableSqFt = property.Units * 800,
                Management = property.Units < 80 ? 0.07 : 0.05,
                CapX = 300,
                CapXType = CostType.PerDoor,
                MarketVacancy = 0.07,
                PhysicalVacancy = 0.05,
                AskingPrice = 50000 * property.Units,
                StrikePrice = 40000 * property.Units,
                OfferPrice = 40000 * property.Units,
                PurchasePrice = 40000 * property.Units,
                GrossPotentialRent = property.Units * 800 * 12,
                PropertyClass = PropertyClass.ClassB,
                NeighborhoodClass = PropertyClass.ClassB,
                StartDate = startDate,
                DesiredYield = 0.1,
                HoldYears = 5,
            };

            await _dbContext.UnderwritingPropertyProspects.AddAsync(prospect);
            await _dbContext.SaveChangesAsync();

            var bucketlist = new UnderwritingProspectPropertyBucketList
            {
                PropertyId = prospect.Id
            };
            await _dbContext.UnderwritingProspectPropertyBucketLists.AddAsync(bucketlist);
            await _dbContext.SaveChangesAsync();

            prospect.BucketListId = bucketlist.Id;
            _dbContext.UnderwritingPropertyProspects.Update(prospect);
            await _dbContext.SaveChangesAsync();

            await _dbContext.UnderwritingMortgages.AddAsync(new UnderwritingMortgage {
                LoanAmount = prospect.PurchasePrice * prospect.LTV,
                PropertyId = prospect.Id,
                InterestRate = 0.04,
                Points = 0.01,
                TermInYears = 30
            });
            await _dbContext.SaveChangesAsync();
            var response = await _dbContext.UnderwritingPropertyProspects
                .Select(x => new ProspectPropertyResponse {
                    Id = x.Id,
                    Name = x.Name,
                    CapRate = x.CapRate,
                    Created = x.Timestamp,
                    City = x.City,
                    DebtCoverage = x.DebtCoverage,
                    CoC = x.CashOnCash,
                    State = x.State,
                    Underwriter = x.Underwriter.DisplayName,
                    UnderwriterEmail = x.Underwriter.Email,
                    Units = x.Units
                })
                .FirstOrDefaultAsync(x => x.Id == prospect.Id);

            return Ok(response);
        }

        [HttpGet("property/exportUnderwriting/{propertyId:guid}")]
        public async Task<IActionResult> ExportUnderwriting(Guid propertyId)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

            var fileName = $"{property.Name}.zip";
            byte[] zipData = Array.Empty<byte>();
            using (var fileStream = new MemoryStream())
            using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create))
            {
                var v1Data = UnderwritingService.GenerateUnderwritingSpreadsheet(property);
                archive.AddFile($"{property.Name}.xlsx", v1Data);

                var v2Data = UnderwritingV2Service.GenerateUnderwritingSpreadsheet(property);
                archive.AddFile($"{property.Name}-v2.xlsx", v2Data);
                zipData = fileStream.ToArray();
            }

            var info = FileTypeLookup.GetFileTypeInfo(fileName);
            return File(zipData, info.MimeType, fileName);
        }

        [HttpGet("property/{propertyId:guid}")]
        public async Task<IActionResult> GetProperty(Guid propertyId)
        {
            var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);
            return Ok(property);
        }

        [Authorize(Policy = PortalPolicy.Underwriter)]
        [HttpPost("update/note/{id:guid}")]
        public async Task<IActionResult> UpdateNote(Guid id, [FromBody]UnderwritingAnalysisNote model)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var note = await _dbContext.UnderwritingNotes.FirstOrDefaultAsync(x => x.Id == id && x.Underwriter.Email == email);

            if (note is null)
                return BadRequest();

            note.Note = model.Note;
            _dbContext.UnderwritingNotes.Update(note);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = PortalPolicy.Underwriter)]
        [HttpDelete("delete/note/{id:guid}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var note = await _dbContext.UnderwritingNotes.FirstOrDefaultAsync(x => x.Id == id && x.Underwriter.Email == email);

            if (note is null)
                return Ok();

            _dbContext.UnderwritingNotes.Remove(note);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = PortalPolicy.Underwriter)]
        [HttpPost("update/{propertyId:guid}")]
        public async Task<IActionResult> UpdateProperty(Guid propertyId, [FromBody] UnderwritingAnalysis analysis)
        {
            if (propertyId == default || propertyId != analysis.Id)
                return BadRequest();

            var email = User.FindFirstValue(ClaimTypes.Email);
            var updatedAnalysis = await _underwritingService.UpdateProperty(propertyId, analysis, email);

            return Ok(updatedAnalysis);
        }

        [HttpGet("files/{propertyId:guid}")]
        public async Task<IActionResult> GetFiles(Guid propertyId)
        {
            var host = $"{Request.Scheme}://{Request.Host}";
            var files = await _dbContext.UnderwritingProspectFiles
                .Where(x => x.PropertyId == propertyId)
                .Select(x => new UnderwritingAnalysisFile
                {
                    Id = x.Id,
                    Description = x.Description,
                    DownloadLink = $"/api/files/property/{propertyId}/file/{x.Id}",
                    Icon = x.Icon,
                    Name = x.Name,
                    Timestamp = x.Timestamp,
                })
                .ToArrayAsync();

            return Ok(files);
        }

        [HttpDelete("files/{propertyId:guid}/file/{fileId:guid}")]
        public async Task<IActionResult> Delete(Guid propertyId, Guid fileId, [FromServices]IStorageService storage)
        {
            var file = await _dbContext.UnderwritingProspectFiles.FirstOrDefaultAsync(x => x.PropertyId == propertyId && x.Id == fileId);
            if(file != null)
            {
                _dbContext.UnderwritingProspectFiles.Remove(file);
                await _dbContext.SaveChangesAsync();
                var path = Path.Combine("underwriting", $"{propertyId}", $"{fileId}{Path.GetExtension(file.Name)}");
                await storage.DeleteAsync(path);
                return Ok();
            }

            return NotFound();
        }

        [HttpPost("upload/save/{id:guid}")]
        public async Task<IActionResult> SaveFile(
            [FromQuery]UnderwritingProspectFileType fileType,
            [FromQuery]string description,
            Guid id,
            IFormFile file,
            [FromServices]IWebHostEnvironment hostEnvironment,
            [FromServices]IStorageService storage)
            // must match SaveField which defaults to "files"
        {
            if (file != null)
            {
                try
                {
                    var property = await _dbContext.UnderwritingPropertyProspects.FirstOrDefaultAsync(x => x.Id == id);
                    if (property is null)
                        return NotFound();

                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                    var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    var physicalPath = Path.Combine(hostEnvironment.WebRootPath, fileName);

                    var prospectFile = new UnderwritingProspectFile
                    {
                        Description = description,
                        Icon = FileTypeLookup.GetFileTypeInfo(fileName).Icon,
                        Name = fileName,
                        PropertyId = property.Id,
                        Type = fileType,
                        UnderwriterEmail = User.FindFirstValue(ClaimTypes.Email)
                    };

                    await _dbContext.UnderwritingProspectFiles.AddAsync(prospectFile);
                    await _dbContext.SaveChangesAsync();

                    var path = Path.Combine("underwriting", $"{property.Id}", $"{prospectFile.Id}{Path.GetExtension(fileName)}");
                    var result = await storage.PutAsync(path, file.OpenReadStream(), FileTypeLookup.GetFileTypeInfo(fileName).MimeType);

                    if(result != StoragePutResult.Success)
                    {
                        _dbContext.UnderwritingProspectFiles.Remove(prospectFile);
                        await _dbContext.SaveChangesAsync();
                        return Conflict();
                    }

                    return Ok();
                }
                catch(Exception ex)
                {
                    // Implement error handling here, this example merely indicates an upload failure.
                    Response.StatusCode = 500;
                    await Response.WriteAsync(ex.Message); // custom error message
                }
            }

            // Return an empty string message in this case
            return new EmptyResult();
        }


        [HttpPost("upload/remove/{id:guid}")]
        public ActionResult Remove(Guid id, string fileToRemove) // must match RemoveField which defaults to "files"
        {
            if (fileToRemove != null)
            {
                try
                {
                    //var fileName = Path.GetFileName(fileToRemove);
                    //var physicalPath = Path.Combine(HostingEnvironment.WebRootPath, fileName);

                    //if (System.IO.File.Exists(physicalPath))
                    //{
                    //    // Implement security mechanisms here - prevent path traversals,
                    //    // check for allowed extensions, types, permissions, etc.
                    //    // this sample always deletes the file from the root and is not sufficient for a real application.

                    //    System.IO.File.Delete(physicalPath);
                    //}
                }
                catch(Exception ex)
                {
                    // Implement error handling here, this example merely indicates an upload failure.
                    Response.StatusCode = 500;
                    Response.WriteAsync(ex.Message); // custom error message
                }
            }

            // Return an empty string message in this case
            return new EmptyResult();
        }

        private static bool IsMarket(string guidanceMarket, string subjectMarket)
        {
            if (string.IsNullOrEmpty(guidanceMarket) && string.IsNullOrEmpty(subjectMarket))
                return true;

            if (!string.IsNullOrEmpty(subjectMarket) && !string.IsNullOrEmpty(guidanceMarket))
                return guidanceMarket.Equals(subjectMarket, StringComparison.InvariantCultureIgnoreCase);

            return false;
        }
    }
}
