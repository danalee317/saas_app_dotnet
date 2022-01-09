using System.Reactive.Linq;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using ReactiveUI;
using Xunit;
using Xunit.Abstractions;

namespace MultiFamilyPortal.Tests.Fixtures.Dtos
{
    public class UnderwritingAnalysisFixture
    {
        private class TestObserver : IObserver<Exception>
        {
            private ITestOutputHelper _output { get; }

            public TestObserver(ITestOutputHelper output) => _output = output;

            public void OnCompleted() { }
            public void OnError(Exception error) { }
            public void OnNext(Exception value) => _output.WriteLine(value.ToString());
        }

        public UnderwritingAnalysisFixture(ITestOutputHelper output)
        {
            RxApp.DefaultExceptionHandler = new TestObserver(output);
        }

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
            Assert.Equal(0.095, analysis.CapRate);
        }

        [Fact]
        public void CapRateEquals0WithOnlyExpenses()
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

            Assert.Equal(0, analysis.CapRate);
        }

        [Fact]
        public void NOIEqualsIncomeWithNoExpenses()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItem(new UnderwritingAnalysisLineItem
            {
                Amount = 100000,
                Category = UnderwritingCategory.GrossScheduledRent,
                ExpenseType = ExpenseSheetType.T12
            });

            Assert.Contains(analysis.Ours, x => x.Category == UnderwritingCategory.GrossScheduledRent);
            Assert.Contains(analysis.Ours, x => x.Category == UnderwritingCategory.PhysicalVacancy);
            Assert.Equal(3, analysis.Ours.Count());
            Assert.Equal(95000, analysis.NOI);
        }

        [Fact]
        public void NOIIsNegativeWithNoIncome()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItem(new UnderwritingAnalysisLineItem
            {
                Amount = 20000,
                Category = UnderwritingCategory.Taxes,
                ExpenseType = ExpenseSheetType.T12
            });

            Assert.Equal(-20000, analysis.NOI);
        }

        [Fact]
        public void ForecastHasHoldPlus1()
        {
            var analysis = new UnderwritingAnalysis
            {
                PurchasePrice = 2000000,
                StartDate = DateTimeOffset.Now.AddMonths(3),
                HoldYears = 5
            };

            Assert.Equal(6, analysis.IncomeForecast.Count());
        }

        [Fact]
        public void ProjectionsHasHoldPlus1()
        {
            var analysis = new UnderwritingAnalysis
            {
                PurchasePrice = 2000000,
                StartDate = DateTimeOffset.Now.AddMonths(3),
                HoldYears = 5,
                Units = 40
            };

            Assert.Equal(40, analysis.Units);
            Assert.Equal(6, analysis.IncomeForecast.Count());
            Assert.Equal(6, analysis.Projections.Count());
        }

        [Fact]
        public void UpdatesCostPerUnit()
        {
            var analysis = new UnderwritingAnalysis();

            Assert.Equal(0, analysis.CostPerUnit);

            analysis.PurchasePrice = 2000000;

            Assert.Equal(0, analysis.CostPerUnit);

            analysis.Units = 50;
            Assert.Equal(40000, analysis.CostPerUnit);
        }

        [Fact]
        public void CalculatesTotalAnnualDebtService()
        {
            var analysis = new UnderwritingAnalysis();
            Assert.Equal(0, analysis.AnnualDebtService);

            analysis.PurchasePrice = 2000000;
            Assert.Equal(UnderwritingLoanType.Automatic, analysis.LoanType);
            Assert.Equal(0.8, analysis.LTV);
            Assert.Single(analysis.Mortgages);
            Assert.Equal(91663.74, analysis.Mortgages.First().AnnualDebtService);
            Assert.Equal(91663.74, analysis.AnnualDebtService);
        }

        [Fact]
        public void CalculatesAquisitionFee()
        {
            var analysis = new UnderwritingAnalysis();
            Assert.Equal(0, analysis.AquisitionFee);

            Assert.Equal(0.05, analysis.AquisitionFeePercent);

            analysis.PurchasePrice = 2000000;

            Assert.Equal(100000, analysis.AquisitionFee);
        }

        [Fact]
        public void CalculatesGrossScheduledRent()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.NotEmpty(analysis.Ours);

            Assert.Equal(400000, analysis.GrossScheduledRent);
        }

        [Fact]
        public void CalculatesVacancy()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);
            analysis.PhysicalVacancy = 0.05;
            Assert.Equal(20000, analysis.VacanyTotal);
        }

        [Fact]
        public void CalculatesConcessionsNonPayment()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(12000, analysis.ConcessionsNonPayment);
        }

        [Fact]
        public void CalculatesUtilityReimbursement()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(8500, analysis.UtilityReimbursement);
        }

        [Fact]
        public void CalculatesOtherIncome()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(4300, analysis.OtherIncome);
        }

        [Fact]
        public void CalculatesPropertyTaxes()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(25000, analysis.PropertyTaxes);
        }

        [Fact]
        public void CalculatesPropertyInsurance()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(8000, analysis.PropertyInsurance);
        }

        [Fact]
        public void CalculatesManagement()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);
            analysis.Management = 0.05;

            Assert.Equal(18400, analysis.ManagementTotal);
        }

        [Fact]
        public void CalculatesGeneralAdmin()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(16500, analysis.GeneralAdmin);
        }

        [Fact]
        public void CalculatesMarketing()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(2500, analysis.Marketing);
        }

        [Fact]
        public void CalculatesRepairsAndMaintenance()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(65000, analysis.RepairsMaintenance);
        }

        [Fact]
        public void CalculatesUtilities()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(8500, analysis.Utilities);
        }

        [Fact]
        public void CalculatesPayroll()
        {
            var analysis = new UnderwritingAnalysis();
            analysis.AddOurItems(_ledger);

            Assert.Equal(55000, analysis.Payroll);
        }

        private static readonly IEnumerable<UnderwritingAnalysisLineItem> _ledger = new[]
        {
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 400000, Category = UnderwritingCategory.GrossScheduledRent },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 12000, Category = UnderwritingCategory.ConsessionsNonPayment },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 8500, Category = UnderwritingCategory.UtilityReimbursement },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 3860, Category = UnderwritingCategory.OtherIncome },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 440, Category = UnderwritingCategory.OtherIncomeBad },

            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 25000, Category = UnderwritingCategory.Taxes },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 8000, Category = UnderwritingCategory.Insurance },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 16500, Category = UnderwritingCategory.GeneralAdmin },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 2500, Category = UnderwritingCategory.Marketing },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 65000, Category = UnderwritingCategory.RepairsMaintenance },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 8500, Category = UnderwritingCategory.Utility },
            new UnderwritingAnalysisLineItem { Id = Guid.NewGuid(), Amount = 55000, Category = UnderwritingCategory.Payroll },
        };
    }
}
