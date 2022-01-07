using MultiFamilyPortal.Dtos.Underwriting;
using Xunit;

namespace MultiFamilyPortal.Tests.Fixtures.Dtos
{
    public class UnderwritingAnalysisMortgageFixture
    {
        [Fact]
        public void CalculatesPIPayment()
        {
            var mortgage = new UnderwritingAnalysisMortgage
            {
                LoanAmount = 1000000
            };

            Assert.Equal(57289.84, mortgage.AnnualDebtService);
        }

        [Fact]
        public void CalculatesIOPayment()
        {
            var mortgage = new UnderwritingAnalysisMortgage
            {
                LoanAmount = 1000000,
                InterestOnly = true,
            };

            Assert.Equal(40000, mortgage.AnnualDebtService);
        }

        [Fact]
        public void CalculatesPointCost()
        {
            var mortgage = new UnderwritingAnalysisMortgage
            {
                LoanAmount = 1000000
            };

            Assert.Equal(10000, mortgage.PointCost);
        }
    }
}
