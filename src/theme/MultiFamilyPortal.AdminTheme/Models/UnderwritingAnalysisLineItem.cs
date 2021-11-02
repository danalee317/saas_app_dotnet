using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class UnderwritingAnalysisLineItem
    {
        public string Description { get; set; }

        public UnderwritingCategory Category { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Amount { get; set; }

        public ExpenseSheetType ExpenseType { get; set; }

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double AnnualizedTotal =>
            ExpenseType switch {
                ExpenseSheetType.T1 => Calculated(Amount * 12),
                ExpenseSheetType.T3 => Calculated(Amount * 4),
                ExpenseSheetType.T4 => Calculated(Amount * 3),
                ExpenseSheetType.T6 => Calculated(Amount * 2),
                _ => Amount
            };

        private double Calculated(double value)
        {
            var abs = Math.Abs(value);
            if (Category == UnderwritingCategory.ConsessionsNonPayment || Category.GetLineItemType() == UnderwritingType.Expense)
                return abs * -1;

            return abs;
        }

        public void UpdateFromGuidance(UnderwritingGuidance guidance, UnderwritingAnalysis analysis)
        {
            switch (guidance.Type)
            {
                case CostType.PerDoor:
                    Amount = guidance.Max * analysis.Units;
                    break;
                case CostType.PercentOfPurchase:
                    Amount = guidance.Max * analysis.PurchasePrice;
                    break;
                default:
                    Amount = guidance.Max;
                    break;
            }
        }
    }
}
