using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Extensions;

namespace MultiFamilyPortal.Services
{
    internal class UnderwritingService : IUnderwritingService
    {
        private IMFPContext _dbContext { get; }

        public UnderwritingService(IMFPContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UnderwritingAnalysis> GetUnderwritingAnalysis(Guid propertyId)
        {
            var property = await _dbContext.UnderwritingPropertyProspects
                .Select(x => new UnderwritingAnalysis
                {
                    Address = x.Address,
                    AquisitionFeePercent = x.AquisitionFeePercent,
                    AskingPrice = x.AskingPrice,
                    BucketList = new UnderwritingAnalysisBucketList
                    {
                        CompetitionNotes = x.BucketList.CompetitionNotes,
                        ConstructionType = x.BucketList.ConstructionType,
                        HowUnderwritingWasDetermined = x.BucketList.HowUnderwritingWasDetermined,
                        MarketCapRate = x.BucketList.MarketCapRate,
                        MarketPricePerUnit = x.BucketList.MarketPricePerUnit,
                        Summary = x.BucketList.Summary,
                        UtilityNotes = x.BucketList.UtilityNotes,
                        ValuePlays = x.BucketList.ValuePlays,
                    },
                    CapitalImprovements = x.CapitalImprovements.Select(c => new UnderwritingAnalysisCapitalImprovement
                    {
                        Cost = c.Cost,
                        Description = c.Description,
                    }).ToList(),
                    CapX = x.CapX,
                    CapXType = x.CapXType,
                    City = x.City,
                    ClosingCostMiscellaneous = x.ClosingCostMiscellaneous,
                    ClosingCostPercent = x.ClosingCostPercent,
                    DeferredMaintenance = x.DeferredMaintenance,
                    DesiredYield = x.DesiredYield,
                    Downpayment = x.Downpayment,
                    GrossPotentialRent = x.GrossPotentialRent,
                    HoldYears = x.HoldYears,
                    Id = x.Id,
                    LoanType = x.LoanType,
                    LTV = x.LTV,
                    Management = x.Management,
                    Market = x.Market,
                    MarketVacancy = x.MarketVacancy,

                    Name = x.Name,
                    NeighborhoodClass = x.NeighborhoodClass,
                    Notes = x.Notes.Select(n => new UnderwritingAnalysisNote
                    {
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
                    PropertyClass = x.PropertyClass,
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
            if (mortgages.Any())
            {
                property.AddMortgages(mortgages);
            }

            var unitModels = await _dbContext.UnderwritingPropertyUnitModels
                .Where(x => x.PropertyId != propertyId)
                .Select(m => new UnderwritingAnalysisModel
                {
                    Baths = m.Baths,
                    Beds = m.Beds,
                    MarketRent = m.MarketRent,
                    Name = m.Name,
                    Units = m.Units.Select(u => new UnderwritingAnalysisUnit
                    {
                        AtWill = u.AtWill,
                        Balance = u.Balance,
                        DepositOnHand = u.DepositOnHand,
                        LeaseEnd = u.LeaseEnd,
                        LeaseStart = u.LeaseStart,
                        Ledger = u.Ledger.Select(l => new UnderwritingAnalysisUnitLedgerItem
                        {
                            ChargesCredits = l.ChargesCredits,
                            Rent = l.Rent,
                            Type = l.Type
                        }).ToList()
                    }).ToList(),
                }).ToListAsync();
            property.Models = unitModels;

            var lineItems = await _dbContext.UnderwritingLineItems.Where(x => x.PropertyId == property.Id).ToArrayAsync();
            if (lineItems.Any())
            {
                foreach (var column in lineItems.GroupBy(x => x.Column))
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

        public async Task<UnderwritingAnalysis> UpdateProperty(Guid propertyId, UnderwritingAnalysis analysis, string email)
        {
            var property = await _dbContext.UnderwritingPropertyProspects
                .Include(x => x.BucketList)
                .Include(x => x.Notes)
                .FirstOrDefaultAsync(x => x.Id == propertyId);


            await UpdateBucketlist(property, analysis);
            await UpdateCapitalImprovements(propertyId, analysis);
            await UpdateLineItems(propertyId, analysis);
            await UpdateMortgages(propertyId, analysis);
            await UpdateNotes(propertyId, analysis, property, email);
            await UpdateUnitModelsAndRentRoll(propertyId, analysis);
            await UpdateProspectProperty(propertyId, analysis);

            return await GetUnderwritingAnalysis(propertyId);
        }

        private async Task UpdateLineItems(Guid propertyId, UnderwritingAnalysis analysis)
        {
            if (await _dbContext.UnderwritingLineItems.AnyAsync(x => x.PropertyId == propertyId))
            {
                var oldLineItems = await _dbContext.UnderwritingLineItems.Where(x => x.PropertyId == propertyId).ToArrayAsync();
                if (oldLineItems.Any())
                {
                    _dbContext.UnderwritingLineItems.RemoveRange(oldLineItems);
                    await _dbContext.SaveChangesAsync();
                }
            }

            var newLineItems = new List<UnderwritingLineItem>();
            if (analysis?.Sellers?.Any() ?? false)
            {
                var sellers = analysis.Sellers
                    .Select(x => new UnderwritingLineItem
                    {
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
                    .Select(x => new UnderwritingLineItem
                    {
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

                var vacancy = new UnderwritingLineItem
                {
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

            if (newLineItems.Any())
            {
                await _dbContext.UnderwritingLineItems.AddRangeAsync(newLineItems);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task UpdateMortgages(Guid propertyId, UnderwritingAnalysis analysis)
        {
            if (await _dbContext.UnderwritingMortgages.AnyAsync(x => x.PropertyId == propertyId))
            {
                var oldMortgages = await _dbContext.UnderwritingMortgages.Where(x => x.PropertyId == propertyId).ToArrayAsync();
                if (oldMortgages.Any())
                {
                    _dbContext.UnderwritingMortgages.RemoveRange(oldMortgages);
                    await _dbContext.SaveChangesAsync();
                }
            }

            if (analysis.PurchasePrice > 0)
            {
                if (analysis.LoanType == UnderwritingLoanType.Automatic)
                {
                    var analysisMortgage = analysis?.Mortgages?.FirstOrDefault() ?? new UnderwritingAnalysisMortgage();
                    var mortgage = new UnderwritingMortgage
                    {
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
                else if (analysis?.Mortgages?.Any() ?? false)
                {
                    var mortgages = analysis.Mortgages
                        .Select(x => new UnderwritingMortgage
                        {
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
        }

        private async Task UpdateNotes(Guid propertyId, UnderwritingAnalysis analysis, UnderwritingProspectProperty property, string email)
        {
            var userId = await _dbContext.Users
                .Where(x => x.Email == email)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var newNotes = analysis.Notes
                .Where(x => !property.Notes.Any(n => n.Id == x.Id))
                .Select(x => new UnderwritingNote
                {
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
        }

        private async Task UpdateCapitalImprovements(Guid propertyId, UnderwritingAnalysis analysis)
        {
            if (await _dbContext.UnderwritingProspectPropertyCapitalImprovements.AnyAsync(x => x.PropertyId == propertyId))
            {
                var existingImprovements = await _dbContext.UnderwritingProspectPropertyCapitalImprovements
                    .Where(x => x.PropertyId != propertyId)
                    .ToArrayAsync();
                _dbContext.UnderwritingProspectPropertyCapitalImprovements.RemoveRange(existingImprovements);
                await _dbContext.SaveChangesAsync();
            }

            if (analysis.CapitalImprovements?.Any() ?? false)
            {
                await _dbContext.UnderwritingProspectPropertyCapitalImprovements.AddRangeAsync(analysis.CapitalImprovements.Select(x => new UnderwritingProspectPropertyCapitalImprovements
                {
                    Cost = x.Cost,
                    Description = x.Description,
                    PropertyId = propertyId
                }));
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task UpdateUnitModelsAndRentRoll(Guid propertyId, UnderwritingAnalysis analysis)
        {
            var existingUnits = await _dbContext.UnderwritingPropertyUnitModels
                .Include(x => x.Units)
                    .ThenInclude(x => x.Ledger)
                .ToArrayAsync();

            if (existingUnits?.Any() ?? false)
            {
                _dbContext.UnderwritingPropertyUnitModels.RemoveRange(existingUnits);
                await _dbContext.SaveChangesAsync();
            }

            if (analysis.Models?.Any() ?? false)
            {
                foreach (var model in analysis.Models)
                {
                    var unitModel = new UnderwritingPropertyUnitModel
                    {
                        Baths = model.Baths,
                        Beds = model.Beds,
                        MarketRent = model.MarketRent,
                        Name = model.Name,
                        PropertyId = propertyId,
                    };
                    await _dbContext.AddAsync(unitModel);
                    await _dbContext.SaveChangesAsync();
                    await AddUnits(model.Units, unitModel);
                }
            }
        }

        private async Task AddUnits(IEnumerable<UnderwritingAnalysisUnit> units, UnderwritingPropertyUnitModel model)
        {
            if (units is null || !units.Any())
                return;

            foreach(var unit in units)
            {
                var propertyUnit = new UnderwritingPropertyUnit
                {
                    AtWill = unit.AtWill,
                    Balance = unit.Balance,
                    DepositOnHand = unit.DepositOnHand,
                    LeaseEnd = unit.LeaseEnd,
                    LeaseStart = unit.LeaseStart,
                    ModelId = model.Id,
                    Rent = unit.Rent,
                    Renter = unit.Renter,
                    Unit = unit.Unit
                };
                await _dbContext.UnderwritingPropertyUnits.AddAsync(propertyUnit);
                await _dbContext.SaveChangesAsync();
                if(unit.Ledger?.Any() ?? false)
                {
                    var ledgerItems = unit.Ledger.Select(x => new UnderwritingPropertyUnitLedger
                    {
                        ChargesCredits = x.ChargesCredits,
                        Rent = x.Rent,
                        Type = x.Type,
                        UnitId = propertyUnit.Id
                    });
                    await _dbContext.UnderwritingPropertyUnitsLedger.AddRangeAsync(ledgerItems);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

        private async Task UpdateBucketlist(UnderwritingProspectProperty property, UnderwritingAnalysis analysis)
        {
            if (property.BucketList is null)
            {
                var bucketList = new UnderwritingProspectPropertyBucketList
                {
                    CompetitionNotes = analysis.BucketList.CompetitionNotes,
                    ConstructionType = analysis.BucketList.ConstructionType,
                    HowUnderwritingWasDetermined = analysis.BucketList.HowUnderwritingWasDetermined,
                    MarketCapRate = analysis.BucketList.MarketCapRate,
                    MarketPricePerUnit = analysis.BucketList.MarketPricePerUnit,
                    PropertyId = property.Id,
                    Summary = analysis.BucketList.Summary,
                    UtilityNotes = analysis.BucketList.UtilityNotes,
                    ValuePlays = analysis.BucketList.ValuePlays
                };

                await _dbContext.UnderwritingProspectPropertyBucketLists.AddAsync(bucketList);
                await _dbContext.SaveChangesAsync();
                property.BucketListId = bucketList.Id;
            }
            else
            {
                var bucketList = property.BucketList;
                bucketList.CompetitionNotes = analysis.BucketList.CompetitionNotes;
                bucketList.ConstructionType = analysis.BucketList.ConstructionType;
                bucketList.HowUnderwritingWasDetermined = analysis.BucketList.HowUnderwritingWasDetermined;
                bucketList.MarketCapRate = analysis.BucketList.MarketCapRate;
                bucketList.MarketPricePerUnit = analysis.BucketList.MarketPricePerUnit;
                bucketList.Summary = analysis.BucketList.Summary;
                bucketList.UtilityNotes = analysis.BucketList.UtilityNotes;
                bucketList.ValuePlays = analysis.BucketList.ValuePlays;
                _dbContext.UnderwritingProspectPropertyBucketLists.Update(bucketList);
                await _dbContext.SaveChangesAsync();
            }
        }

        private async Task UpdateProspectProperty(Guid propertyId, UnderwritingAnalysis analysis)
        {
            var property = await _dbContext.UnderwritingPropertyProspects
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
            property.DesiredYield = analysis.DesiredYield;
            property.Downpayment = analysis.Downpayment;
            property.GrossPotentialRent = analysis.GrossPotentialRent;
            property.HoldYears = analysis.HoldYears;
            property.LoanType = analysis.LoanType;
            property.LTV = analysis.LTV;
            property.Management = analysis.Management;
            property.Market = analysis.Market;
            property.MarketVacancy = analysis.MarketVacancy;
            property.Name = analysis.Name;
            property.NeighborhoodClass = analysis.NeighborhoodClass;
            property.NOI = analysis.NOI;
            property.OfferPrice = analysis.OfferPrice;
            property.OurEquityOfCF = analysis.OurEquityOfCF;
            property.PhysicalVacancy = analysis.PhysicalVacancy;
            property.PropertyClass = analysis.PropertyClass;
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
        }
    }
}
