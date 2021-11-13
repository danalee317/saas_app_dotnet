using System.Data;
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
using MultiFamilyPortal.Dtos.Underwrting;
using MultiFamilyPortal.Extensions;

namespace MultiFamilyPortal.Areas.Admin.Controllers
{
    [Authorize(Policy = PortalPolicy.UnderwritingViewer)]
    [ApiController]
    [Route("/api/admin/underwriting")]
    public class UnderwritingController : ControllerBase
    {
        private IMFPContext _dbContext { get; }
        private ILogger _logger { get; }

        public UnderwritingController(IMFPContext dbContext, ILogger<UnderwritingController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
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
                if (!await _dbContext.UnderwritingPropertyProspects.Where(x => x.Timestamp > start && x.Timestamp < end).AnyAsync())
                    return Ok(Array.Empty<ProspectPropertyResponse>());

                var query = _dbContext.UnderwritingPropertyProspects
                    .Include(x => x.Underwriter)
                    .Where(x => x.Timestamp > start && x.Timestamp < end);

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
                .Where(x => x.Market == market)
                .ToArrayAsync();
            return Ok(guidance);
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
            var prospect = new UnderwritingProspectProperty
            {
                Name = property.Name,
                Units = property.Units,
                UnderwriterId = user.Id,
                RentableSqFt = property.Units * 800,
                Management = property.Units < 80 ? 0.07 : 0.05,
                CapX = 300,
                CapXType = CostType.PerDoor,
                MarketVacancy = 0.07,
                PhysicalVacancy = 0.05,
                AskingPrice = 50000 * property.Units,
                StrikePrice = 40000 * property.Units,
                OfferPrice = 36000 * property.Units,
                PurchasePrice = 37000 * property.Units,
                GrossPotentialRent = 800 * property.Units
            };

            await _dbContext.UnderwritingPropertyProspects.AddAsync(prospect);
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
            var property = await GetUnderwritingAnalysis(propertyId);
            var data = UnderwritingService.GenerateUnderwritingSpreadsheet(property);

            var fileName = $"{property.Name}.xlsx";
            var info = FileTypeLookup.GetFileTypeInfo(fileName);
            return File(data, info.MimeType, fileName);
        }

        [HttpGet("property/{propertyId:guid}")]
        public async Task<IActionResult> GetProperty(Guid propertyId)
        {
            var property = await GetUnderwritingAnalysis(propertyId);
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
            var userId = await _dbContext.Users
                .Where(x => x.Email == email)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
            //var underwritiers = await _dbContext.Users.Where(x => x.Roles.Any(r => r.Role.Name == PortalRoles.PortalAdministrator || r.Role.Name == PortalRoles.Underwriter))
            //    .ToDictionaryAsync(x => x.Email, x => x.Id);
            var property = await _dbContext.UnderwritingPropertyProspects
                .Include(x => x.Notes)
                .FirstOrDefaultAsync(x => x.Id == propertyId);

            // Update Line Items
            if(await _dbContext.UnderwritingLineItems.AnyAsync(x => x.PropertyId == propertyId))
            {
                var oldLineItems = await _dbContext.UnderwritingLineItems.Where(x => x.PropertyId == propertyId).ToArrayAsync();
                if (oldLineItems.Any())
                {
                    _dbContext.UnderwritingLineItems.RemoveRange(oldLineItems);
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (await _dbContext.UnderwritingMortgages.AnyAsync(x => x.PropertyId == propertyId))
            {
                var oldMortgages = await _dbContext.UnderwritingMortgages.Where(x => x.PropertyId == propertyId).ToArrayAsync();
                if (oldMortgages.Any())
                {
                    _dbContext.UnderwritingMortgages.RemoveRange(oldMortgages);
                    await _dbContext.SaveChangesAsync();
                }
            }

            var newNotes = analysis.Notes
                .Where(x => !property.Notes.Any(n => n.Id == x.Id))
                .Select(x => new UnderwritingNote {
                    Id = x.Id,
                    Note = x.Note,
                    PropertyId = propertyId,
                    UnderwriterId = userId
                });
            if (newNotes.Any())
            {
                await _dbContext.UnderwritingNotes.AddRangeAsync(newNotes);
                await _dbContext.SaveChangesAsync();
            }

            if(analysis.PurchasePrice > 0)
            {
                if(analysis.LoanType == UnderwritingLoanType.Automatic)
                {
                    var analysisMortgage = analysis?.Mortgages?.FirstOrDefault() ?? new UnderwritingAnalysisMortgage();
                    var mortgage = new UnderwritingMortgage {
                        BalloonMonths = analysisMortgage.BalloonMonths,
                        InterestOnly = analysisMortgage.InterestOnly,
                        InterestRate = analysisMortgage.InterestRate,
                        LoanAmount = analysis.PurchasePrice * analysis.LTV,
                        Points = analysisMortgage.Points,
                        PropertyId = propertyId,
                        TermInYears = analysisMortgage.TermInYears,
                    };
                    await _dbContext.UnderwritingMortgages.AddAsync(mortgage);
                }
                else if(analysis?.Mortgages?.Any() ?? false)
                {
                    var mortgages = analysis.Mortgages
                        .Select(x => new UnderwritingMortgage {
                            BalloonMonths = x.BalloonMonths,
                            InterestOnly = x.InterestOnly,
                            InterestRate = x.InterestRate,
                            LoanAmount = x.LoanAmount,
                            Points = x.Points,
                            PropertyId = propertyId,
                            TermInYears = x.TermInYears
                        });

                    await _dbContext.UnderwritingMortgages.AddRangeAsync(mortgages);
                }
            }

            var newLineItems = new List<UnderwritingLineItem>();
            if(analysis?.Sellers?.Any() ?? false)
            {
                var sellers = analysis.Sellers
                    .Select(x => new UnderwritingLineItem {
                        Amount = x.Amount,
                        Category = x.Category,
                        Column = UnderwritingColumn.Sellers,
                        Description = string.IsNullOrEmpty(x.Description) ? x.Category.GetDisplayName() : x.Description,
                        ExpenseType = x.ExpenseType,
                        PropertyId = propertyId,
                        Type = x.Category.GetLineItemType()
                    });

                newLineItems.AddRange(sellers);
            }

            if (analysis?.Ours?.Any() ?? false)
            {
                var ours = analysis.Ours
                    .Where(x => !(x.Category == UnderwritingCategory.Management || x.Category == UnderwritingCategory.PhysicalVacancy))
                    .Select(x => new UnderwritingLineItem {
                        Amount = x.Amount,
                        Category = x.Category,
                        Column = UnderwritingColumn.Ours,
                        Description = string.IsNullOrEmpty(x.Description) ? x.Category.GetDisplayName() : x.Description,
                        ExpenseType = x.ExpenseType,
                        PropertyId = propertyId,
                        Type = x.Category.GetLineItemType()
                    }).ToList();

                var vacancyRate = Math.Min(analysis.MarketVacancy, analysis.PhysicalVacancy);
                if (vacancyRate < 0.05)
                    vacancyRate = 0.05;

                var vacancy = new UnderwritingLineItem {
                    Amount = analysis.GrossPotentialRent * vacancyRate,
                    Category = UnderwritingCategory.PhysicalVacancy,
                    Description = UnderwritingCategory.PhysicalVacancy.GetDisplayName(),
                    ExpenseType = ExpenseSheetType.T12,
                    Column = UnderwritingColumn.Ours,
                    PropertyId = propertyId,
                    Type = UnderwritingType.Income
                };

                var management = new UnderwritingLineItem
                {
                    Amount = analysis.Management * analysis.GrossPotentialRent,
                    Category = UnderwritingCategory.Management,
                    Column = UnderwritingColumn.Ours,
                    Description = UnderwritingCategory.Management.GetDisplayName(),
                    ExpenseType = ExpenseSheetType.T12,
                    PropertyId = propertyId,
                    Type = UnderwritingType.Expense
                };
                ours.Add(vacancy);
                ours.Add(management);

                newLineItems.AddRange(ours);
            }

            if(newLineItems.Any())
            {
                await _dbContext.UnderwritingLineItems.AddRangeAsync(newLineItems);
                await _dbContext.SaveChangesAsync();
            }

            property = await _dbContext.UnderwritingPropertyProspects
                .Include(x => x.LineItems)
                .Include(x => x.Mortgages)
                .Include(x => x.Notes)
                .FirstOrDefaultAsync(x => x.Id == propertyId);

            property.Address = analysis.Address;
            property.AquisitionFeePercent = analysis.AquisitionFeePercent;
            property.AskingPrice = analysis.AskingPrice;
            property.CapRate = analysis.CapRate;
            property.CapX = analysis.CapX;
            property.CapXType = analysis.CapXType;
            property.CashOnCash = analysis.CashOnCash;
            property.City = analysis.City;
            property.ClosingCostMiscellaneous = analysis.ClosingCostMiscellaneous;
            property.ClosingCostPercent = analysis.ClosingCostPercent;
            property.DebtCoverage = analysis.DebtCoverage;
            property.DeferredMaintenance = analysis.DeferredMaintenance;
            property.Downpayment = analysis.Downpayment;
            property.GrossPotentialRent = analysis.GrossPotentialRent;
            property.LoanType = analysis.LoanType;
            property.LTV = analysis.LTV;
            property.Management = analysis.Management;
            property.Market = analysis.Market;
            property.MarketVacancy = analysis.MarketVacancy;
            property.Name = analysis.Name;
            property.NOI = analysis.NOI;
            property.OfferPrice = analysis.OfferPrice;
            property.OurEquityOfCF = analysis.OurEquityOfCF;
            property.PhysicalVacancy = analysis.PhysicalVacancy;
            property.PurchasePrice = analysis.PurchasePrice;
            property.RentableSqFt = analysis.RentableSqFt;
            property.SECAttorney = analysis.SECAttorney;
            property.State = analysis.State;
            property.Status = analysis.Status;
            property.StrikePrice = analysis.StrikePrice;
            property.Units = analysis.Units;
            property.Vintage = analysis.Vintage;
            property.Zip = analysis.Zip;

            _dbContext.Update(property);
            await _dbContext.SaveChangesAsync();

            var updatedAnalysis = await GetUnderwritingAnalysis(propertyId);
            return Ok(updatedAnalysis);
        }

        [HttpPost("upload/save/{id:guid}")]
        public async Task<IActionResult> Save(Guid id, IFormFile file) // must match SaveField which defaults to "files"
        {
            if (file != null)
            {
                try
                {
                    var fileContent = ContentDispositionHeaderValue.Parse(file.ContentDisposition);

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.
                    //var fileName = Path.GetFileName(fileContent.FileName.ToString().Trim('"'));
                    //var physicalPath = Path.Combine(HostingEnvironment.WebRootPath, fileName);

                    //// Implement security mechanisms here - prevent path traversals,
                    //// check for allowed extensions, types, size, content, viruses, etc.
                    //// This sample always saves the file to the root and is not sufficient for a real application.

                    //using (var fileStream = new FileStream(physicalPath, FileMode.Create))
                    //{
                    //    await file.CopyToAsync(fileStream);
                    //}
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

        private async Task<UnderwritingAnalysis> GetUnderwritingAnalysis(Guid propertyId)
        {
            var property = await _dbContext.UnderwritingPropertyProspects
                .Select(x => new UnderwritingAnalysis {
                    Address = x.Address,
                    AquisitionFeePercent = x.AquisitionFeePercent,
                    AskingPrice = x.AskingPrice,
                    CapX = x.CapX,
                    CapXType = x.CapXType,
                    City = x.City,
                    ClosingCostMiscellaneous = x.ClosingCostMiscellaneous,
                    ClosingCostPercent = x.ClosingCostPercent,
                    DeferredMaintenance = x.DeferredMaintenance,
                    Downpayment = x.Downpayment,
                    GrossPotentialRent = x.GrossPotentialRent,
                    Id = x.Id,
                    LoanType = x.LoanType,
                    LTV = x.LTV,
                    Management = x.Management,
                    Market = x.Market,
                    MarketVacancy = x.MarketVacancy,
                    Name = x.Name,
                    Notes = x.Notes.Select(n => new UnderwritingAnalysisNote {
                        Id = n.Id,
                        Note = n.Note,
                        Timestamp = n.Timestamp,
                        Underwriter = n.Underwriter.DisplayName,
                        UnderwriterEmail = n.Underwriter.Email,
                        UnderwriterId = n.UnderwriterId
                    }).ToList(),
                    OfferPrice = x.OfferPrice,
                    OurEquityOfCF = x.OurEquityOfCF,
                    PhysicalVacancy = x.PhysicalVacancy,
                    PurchasePrice = x.PurchasePrice,
                    RentableSqFt = x.RentableSqFt,
                    SECAttorney = x.SECAttorney,
                    State = x.State,
                    Status = x.Status,
                    StrikePrice = x.StrikePrice,
                    Timestamp = x.Timestamp,
                    Underwriter = x.Underwriter.DisplayName,
                    UnderwriterEmail = x.Underwriter.Email,
                    Units = x.Units,
                    Vintage = x.Vintage,
                    Zip = x.Zip,
                })
                .FirstOrDefaultAsync(x => x.Id == propertyId);

            var mortgages = await _dbContext.UnderwritingMortgages.Where(x => x.PropertyId == property.Id)
                .Select(m => new UnderwritingAnalysisMortgage
                {
                    Id = m.Id,
                    BalloonMonths = m.BalloonMonths,
                    InterestOnly = m.InterestOnly,
                    InterestRate = m.InterestRate,
                    LoanAmount = m.LoanAmount,
                    Points = m.Points,
                    TermInYears = m.TermInYears
                })
                .ToArrayAsync();
            if(mortgages.Any())
            {
                property.AddMortgages(mortgages);
            }

            var lineItems = await _dbContext.UnderwritingLineItems.Where(x => x.PropertyId == property.Id).ToArrayAsync();
            if(lineItems.Any())
            {
                foreach(var column in lineItems.GroupBy(x => x.Column))
                {
                    var items = column.Select(x => new UnderwritingAnalysisLineItem
                    {
                        Amount = x.Amount,
                        Category = x.Category,
                        Description = x.Description,
                        ExpenseType = x.ExpenseType,
                        Id = x.Id
                    });
                    if (column.Key == UnderwritingColumn.Sellers)
                        property.AddSellerItems(items);
                    else
                        property.AddOurItems(items);
                }
            }

            return property;
        }
    }
}
