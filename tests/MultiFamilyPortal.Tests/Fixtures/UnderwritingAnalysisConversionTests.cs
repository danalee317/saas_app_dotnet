using System.Text.Json;
using MultiFamilyPortal.Dtos.Underwriting;
using Xunit;

namespace MultiFamilyPortal.Tests.Fixtures
{
    public class UnderwritingAnalysisConversionTests
    {
        private const string ExpectedJson = @"{""id"":""c3dd3627-9ca7-48db-bbe4-0ed04f8b42e2"",""timestamp"":""12/29/2021 4:21:16 PM -08:00"",""startDate"":""3/29/2022 4:21:16 PM -08:00"",""name"":""Hello World"",""address"":""123 Some St"",""city"":""San Diego"",""state"":""CA"",""desiredYield"":0,""holdYears"":0,""units"":45,""vintage"":1980,""loanType"":""Automatic"",""lTV"":0.75,""askingPrice"":2600000,""strikePrice"":2500000,""offerPrice"":2500000,""purchasePrice"":2500000,""downpayment"":0,""rentableSqFt"":29250,""grossPotentialRent"":0,""physicalVacancy"":0,""marketVacancy"":0,""management"":0,""capX"":0,""capXType"":""PerDoor"",""ourEquityOfCF"":0.25,""aquisitionFeePercent"":0.05,""closingCostPercent"":0.03,""deferredMaintenance"":0,""sECAttorney"":0,""closingCostMiscellaneous"":0,""status"":""Active"",""propertyClass"":""ClassA"",""neighborhoodClass"":""ClassA"",""sellers"":[{""id"":""a02c392b-1d49-46bf-a265-1f79c7b60be2"",""description"":""Gross Scheduled Rent"",""category"":""GrossScheduledRent"",""amount"":150000,""expenseType"":""T12""}],""ours"":[{""id"":""dc9dd330-68ea-4f80-aaa7-9655690161d3"",""description"":""Gross Scheduled Rent"",""category"":""GrossScheduledRent"",""amount"":150000,""expenseType"":""T12""}],""mortgages"":[{""id"":""14772ceb-4f07-447e-afdf-3e0a3fc21228"",""loanAmount"":1875000,""interestRate"":0.03875,""termInYears"":30,""interestOnly"":false,""balloonMonths"":0,""points"":0.01}],""notes"":[],""incomeForecast"":[{""year"":0,""increaseType"":""Percent"",""perUnitIncrease"":0,""unitsAppliedTo"":0,""fixedIncreaseOnRemainingUnits"":0,""vacancy"":0,""otherLossesPercent"":0,""utilityIncreases"":0,""otherIncomePercent"":0}]}";

        [Fact]
        public void SerializesWithoutException()
        {
            var model = new UnderwritingAnalysis()
            {
                Id = Guid.NewGuid(),
                Name = "Hello World",
                Address = "123 Some St",
                City = "San Diego",
                State = "CA",
                AskingPrice = 2600000,
                OfferPrice = 2500000,
                StrikePrice = 2500000,
                PurchasePrice = 2500000,
                LTV = 0.75,
                LoanType = Data.Models.UnderwritingLoanType.Automatic,
                Units = 45,
                RentableSqFt = 45 * 650,
                StartDate = DateTimeOffset.Now.AddMonths(3),
                Timestamp = DateTimeOffset.Now,
                Vintage = 1980
            };
            model.AddMortgage(new UnderwritingAnalysisMortgage
            {
                Id = Guid.NewGuid(),
                InterestRate = 0.03875,
                LoanAmount = 2500000 * 0.75,
                TermInYears = 30,
                Points = 0.01,
            });
            model.AddOurItem(new UnderwritingAnalysisLineItem
            {
                Id = Guid.NewGuid(),
                Amount = 150000,
                Category = Data.Models.UnderwritingCategory.GrossScheduledRent,
                ExpenseType = Data.Models.ExpenseSheetType.T12,
                Description = "Gross Scheduled Rent",
            });
            model.AddSellerItem(new UnderwritingAnalysisLineItem
            {
                Id = Guid.NewGuid(),
                Amount = 150000,
                Category = Data.Models.UnderwritingCategory.GrossScheduledRent,
                ExpenseType = Data.Models.ExpenseSheetType.T12,
                Description = "Gross Scheduled Rent",
            });

            string json = null;
            var ex = Record.Exception(() => json = JsonSerializer.Serialize(model));

            Assert.Null(ex);
            Assert.NotEmpty(json);
        }

        [Fact]
        public void AddsMortgages()
        {
            var model = JsonSerializer.Deserialize<UnderwritingAnalysis>(ExpectedJson);

            Assert.NotNull(model);
            Assert.Single(model.Mortgages);

            var mortgage = model.Mortgages.First();
            Assert.Equal(1875000, mortgage.LoanAmount);
        }

        [Fact]
        public void AddsProForma()
        {
            var model = JsonSerializer.Deserialize<UnderwritingAnalysis>(ExpectedJson);

            Assert.NotNull(model);
            Assert.Single(model.Ours);

            var lineItem = model.Ours.First();
            Assert.Equal(150000, lineItem.AnnualizedTotal);
        }

        [Fact]
        public void AddsStatedInPlace()
        {
            var model = JsonSerializer.Deserialize<UnderwritingAnalysis>(ExpectedJson);

            Assert.NotNull(model);
            Assert.Single(model.Sellers);

            var lineItem = model.Sellers.First();
            Assert.Equal(150000, lineItem.AnnualizedTotal);
        }
    }
}
