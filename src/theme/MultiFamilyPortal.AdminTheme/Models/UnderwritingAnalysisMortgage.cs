using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class UnderwritingAnalysisMortgage
    {
        public double LoanAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double InterestRate { get; set; } = 0.04;

        public int TermInYears { get; set; } = 30;

        public bool InterestOnly { get; set; }

        public int BalloonMonths { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double Points { get; set; } = 0.01;

        [JsonIgnore]
        public double PointCost => Points * LoanAmount;

        [JsonIgnore]
        public double AnnualDebtService => CalculatePayment();

        private double CalculatePayment()
        {
            if (TermInYears <= 0)
                TermInYears = 30;

            if (InterestRate <= 0)
                InterestRate = 0.04;

            var termOfLoan = TermInYears * 12;

            if (InterestOnly)
            {
                return LoanAmount * InterestRate;
            }
            else
            {
                var payment = LoanAmount * (Math.Pow((1 + InterestRate / 12), termOfLoan) * InterestRate) / (12 * (Math.Pow((1 + InterestRate / 12), termOfLoan) - 1));

                return payment * 12;

                //var amortizationFormula = ((1 + i) / n);
                //// ((Math.Pow((1 + InterestRate), n) - 1)/Math.Pow(i(1 + i), n)) * 12;
                //var monthlyPayment = (LoanAmount * (((i * amortizationFormula) / amortizationFormula) - 1));
                //return monthlyPayment * 12;
            }
        }
    }
}
