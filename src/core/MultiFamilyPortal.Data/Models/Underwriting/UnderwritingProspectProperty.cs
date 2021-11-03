using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MultiFamilyPortal.Data.Models
{
    public class UnderwritingProspectProperty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        private DateTimeOffset _timestamp = DateTimeOffset.Now;
        public DateTimeOffset Timestamp => _timestamp;

        public string Name { get; set; }

        public string UnderwriterId { get; set; }

        public string Address { get; set; }

        public string Market { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public int Units { get; set; }

        public int Vintage { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double AskingPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double StrikePrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OfferPrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PurchasePrice { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Downpayment { get; set; }

        public int RentableSqFt { get; set; }

        [JsonIgnore]
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PricePerSqFt => PurchasePrice / Math.Max(RentableSqFt, 1);

        [JsonIgnore]
        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PricePerUnit => PurchasePrice > 0 && Units > 0 ? PurchasePrice / Units : 0;

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double GrossPotentialRent { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double PhysicalVacancy { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double MarketVacancy { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Management { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double CapX { get; set; }

        public CostType CapXType { get; set; }

        [NotMapped]
        public double CapXTotal => CapXType == CostType.PerDoor ? CapX * Units : CapX;

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double NOI { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double LossToLease { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CapRate { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CashOnCash => NOI / Raise;

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double OurEquityOfCF { get; set; } = 0.25;

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double AquisitionFeePercent { get; set; } = 0.05;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [NotMapped]
        public double AquisitionFee => PurchasePrice * AquisitionFeePercent;

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double ClosingCostPercent { get; set; } = 0.03;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [NotMapped]
        public double ClosingCosts => PurchasePrice * ClosingCostPercent;

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double DeferredMaintenance { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double SECAttorney { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosingCostMiscellaneous { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosingCostOther => (AnnualOperatingExpenses() / 6) + InsuranceTotal() + DeferredMaintenance + CapXTotal + SECAttorney + ClosingCostMiscellaneous;

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double DebtCoverage { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Raise => ClosingCostOther + ClosingCosts + AquisitionFee + ((1 - LTV) * PurchasePrice);

        public UnderwritingStatus Status { get; set; }

        public UnderwritingLoanType LoanType { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double LTV { get; set; } = 0.8;

        public SiteUser Underwriter { get; set; }

        public virtual ICollection<UnderwritingLineItem> LineItems { get; set; }

        public virtual ICollection<UnderwritingNote> Notes { get; set; }

        public virtual ICollection<UnderwritingMortgage> Mortgages { get; set; }

        public virtual ICollection<UnderwritingPropertyUnitModel> Models { get; set; }

        public virtual ICollection<UnderwritingProspectFile> Files { get; set; }

        public void Update()
        {
            if (LineItems is null || PurchasePrice < 1)
                return;

            if (PurchasePrice > 0 && LoanType == UnderwritingLoanType.Custom && Mortgages != null && Mortgages.Any())
            {
                var mortgageTotal = Mortgages.Sum(x => x.LoanAmount);
                LTV = mortgageTotal / PurchasePrice;
                Downpayment = PurchasePrice - mortgageTotal;
            }
            else if (PurchasePrice > 0 && LoanType == UnderwritingLoanType.Automatic)
            {
                Downpayment = PurchasePrice - (PurchasePrice * LTV);
            }

            var income = CalculatedIncome();
            var expenses = CalculatedExpenses();
            NOI = income - expenses;
            CapRate = NOI / PurchasePrice;

            var grossScheduledRent = LineItems.FirstOrDefault(x => x.Column == UnderwritingColumn.Ours && x.Category == UnderwritingCategory.GrossScheduledRent);
            if (grossScheduledRent != null)
            {
                LossToLease = GrossPotentialRent - grossScheduledRent.AnnualizedTotal;
            }

            if (Mortgages is null || income == 0 || expenses == 0)
            {
                DebtCoverage = 1;
                return;
            }

            var totalPayments = Mortgages.Sum(x => x.AnnualDebtService);

            if (totalPayments > 0)
                DebtCoverage = NOI / totalPayments;
        }

        private double CalculatedIncome()
        {
            if (LineItems is null)
                throw new ArgumentNullException("The Line Items were null. Could not calculate the updated Income");

            var items = LineItems.Where(x => x.Column == UnderwritingColumn.Ours && x.Type == UnderwritingType.Income);
            return items.Sum(x => x.AnnualizedTotal);
        }

        private double CalculatedExpenses()
        {
            if (LineItems is null)
                throw new ArgumentNullException("The Line Items were null. Could not calculate the updated Expenses");

            var items = LineItems.Where(x => x.Column == UnderwritingColumn.Ours && x.Type == UnderwritingType.Expense);
            return items.Sum(x => x.AnnualizedTotal);
        }

        private double AnnualOperatingExpenses()
        {
            if (LineItems is null || !LineItems.Any(x => x.Column == UnderwritingColumn.Ours && x.Type == UnderwritingType.Expense))
                return 0;

            return LineItems.Where(x => x.Column == UnderwritingColumn.Ours && x.Type == UnderwritingType.Expense && x.Category != UnderwritingCategory.Insurance)
                .Sum(x => x.AnnualizedTotal);
        }

        private double InsuranceTotal()
        {
            if (LineItems is null || !LineItems.Any(x => x.Column == UnderwritingColumn.Ours && x.Category == UnderwritingCategory.Insurance))
                return 0;

            return LineItems.Where(x => x.Column == UnderwritingColumn.Ours && x.Category == UnderwritingCategory.Insurance)
                .Sum(x => x.AnnualizedTotal);
        }
    }
}
