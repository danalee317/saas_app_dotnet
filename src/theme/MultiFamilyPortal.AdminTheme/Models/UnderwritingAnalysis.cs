using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class UnderwritingAnalysis
    {
        public Guid Id { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string Name { get; set; }

        public string Underwriter { get; set; }

        public string UnderwriterEmail { get; set; }

        public string Address { get; set; }

        public string Market { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public int Units { get; set; }

        public int Vintage { get; set; }

        public UnderwritingLoanType LoanType { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double LTV { get; set; } = 0.8;

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
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PricePerSqFt => PurchasePrice / Math.Max(RentableSqFt, 1);

        [JsonIgnore]
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

        [JsonIgnore]
        public double CapXTotal => CapXType == CostType.PerDoor ? CapX * Units : CapX;

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double NOI { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double LossToLease { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CapRate { get; set; }

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CashOnCash => NOI / Raise;

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double OurEquityOfCF { get; set; } = 0.25;

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double AquisitionFeePercent { get; set; } = 0.05;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double AquisitionFee => PurchasePrice * AquisitionFeePercent;

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double ClosingCostPercent { get; set; } = 0.03;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosingCosts => PurchasePrice * ClosingCostPercent;

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double DeferredMaintenance { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double SECAttorney { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosingCostMiscellaneous { get; set; }

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosingCostOther => (AnnualOperatingExpenses() / 6) + InsuranceTotal() + DeferredMaintenance + CapXTotal + SECAttorney + ClosingCostMiscellaneous;

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double DebtCoverage { get; set; }

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Raise => ClosingCostOther + ClosingCosts + AquisitionFee + ((1 - LTV) * PurchasePrice);

        public UnderwritingStatus Status { get; set; }

        public List<UnderwritingAnalysisLineItem> Sellers { get; set; }
        public List<UnderwritingAnalysisLineItem> Ours { get; set; }
        public List<UnderwritingAnalysisMortgage> Mortgages { get; set; }
        public List<UnderwritingAnalysisNote> Notes { get; set; }

        private double AnnualOperatingExpenses()
        {
            if (Ours is null || !Ours.Any(x => x.Category.GetLineItemType() == UnderwritingType.Expense))
                return 0;

            return Ours.Where(x => x.Category.GetLineItemType() == UnderwritingType.Expense && x.Category != UnderwritingCategory.Insurance)
                .Sum(x => x.AnnualizedTotal);
        }

        private double InsuranceTotal()
        {
            if (Ours is null || !Ours.Any(x => x.Category == UnderwritingCategory.Insurance))
                return 0;

            return Ours.Where(x => x.Category == UnderwritingCategory.Insurance)
                .Sum(x => x.AnnualizedTotal);
        }
    }


}
