using System.Reactive.Linq;
using System.Reactive;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using Xunit;

namespace MultiFamilyPortal.Tests.Fixtures
{
    public class UnderwritingAnalysisFixture
    {
        [Fact]
        public async Task DefaultCapRateEqualsZero()
        {
            var analysis = new UnderwritingAnalysis();

            await Task.Delay(200);
            Assert.Equal(0, analysis.CapRate);
        }

        [Fact]
        public async Task CapRateEqualsZeroWithOnlyIncome()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItem(new UnderwritingAnalysisLineItem
            {
                Amount = 100000,
                Category = UnderwritingCategory.GrossScheduledRent,
                ExpenseType = ExpenseSheetType.T12
            });

            await Task.Delay(200);
            Assert.Equal(0, analysis.CapRate);
        }

        [Fact]
        public async Task CapRateEquals10WithPurchasePriceAndIncomeOf100k()
        {
            var analysis = new UnderwritingAnalysis
            {
                PurchasePrice = 1000000
            };
            analysis.AddOurItem(new UnderwritingAnalysisLineItem
            {
                Amount = 100000,
                Category = UnderwritingCategory.GrossScheduledRent,
                ExpenseType = ExpenseSheetType.T12
            });

            await Task.Delay(200);
            Assert.Equal(0.1, analysis.CapRate);
        }

        [Fact]
        public async Task CapRateEquals0WithOnlyExpenses()
        {
            var analysis = new UnderwritingAnalysis
            {
                PurchasePrice = 1000000
            };
            analysis.AddOurItem(new UnderwritingAnalysisLineItem
            {
                Amount = 20000,
                Category = UnderwritingCategory.Taxes,
                ExpenseType = ExpenseSheetType.T12
            });

            await Task.Delay(200);
            Assert.Equal(0, analysis.CapRate);
        }

        [Fact]
        public async Task NOIEqualsIncomeWithNoExpenses()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItem(new UnderwritingAnalysisLineItem
            {
                Amount = 100000,
                Category = UnderwritingCategory.GrossScheduledRent,
                ExpenseType = ExpenseSheetType.T12
            });

            await Task.Delay(500);
            Assert.Single(analysis.Ours);
            Assert.Equal(100000, analysis.NOI);
        }

        [Fact]
        public async Task NOIIsNegativeWithNoIncome()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItem(new UnderwritingAnalysisLineItem
            {
                Amount = 20000,
                Category = UnderwritingCategory.Taxes,
                ExpenseType = ExpenseSheetType.T12
            });

            await Task.Delay(200);
            Assert.Equal(-20000, analysis.NOI);
        }

        [Fact]
        public async Task ForecastHasHoldPlus1()
        {
            var analysis = new UnderwritingAnalysis
            {
                PurchasePrice = 2000000,
                StartDate = DateTimeOffset.Now.AddMonths(3),
                HoldYears = 5
            };

            await Task.Delay(500);
            Assert.Equal(6, analysis.IncomeForecast.Count());
        }

        [Fact]
        public async Task ProjectionsHasHoldPlus1()
        {
            var analysis = new UnderwritingAnalysis
            {
                PurchasePrice = 2000000,
                StartDate = DateTimeOffset.Now.AddMonths(3),
                HoldYears = 5,
                Units = 40
            };

            await Task.Delay(500);
            Assert.Equal(6, analysis.Projections.Count());
        }
    }
}
