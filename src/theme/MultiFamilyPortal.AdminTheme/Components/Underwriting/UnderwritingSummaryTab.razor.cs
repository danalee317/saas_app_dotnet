using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwrting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingSummaryTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        private double ourNOI;
        private double sellerNOI;
        private CostType[] _costTypes = new[] { CostType.PerDoor, CostType.Total };
        private double ourNOIAfterCapX;
        private double sellerNOIAfterCapX;

        private double debtService;
        private double secondaryDebtService;

        private double ourCashFlow;
        private double sellerCashFlow;

        protected override void OnParametersSet()
        {
            if (Property?.Mortgages?.Any() ?? false)
            {
                debtService = Property.Mortgages
                    .Select(x => x.AnnualDebtService)
                    .OrderByDescending(x => x)
                    .FirstOrDefault();

                if (Property.Mortgages.Count() > 1)
                {
                    secondaryDebtService = Property.Mortgages
                        .Select(x => x.AnnualDebtService)
                        .Skip(1)
                        .Sum();
                }
            }

            if (Property?.Ours.Any() ?? false)
            {
                var income = Property.Ours.Where(x => x.Category.GetLineItemType() == UnderwritingType.Income).Sum(x => x.AnnualizedTotal);
                var expenses = Property.Ours.Where(x => x.Category.GetLineItemType() == UnderwritingType.Expense).Sum(x => x.AnnualizedTotal);

                ourNOI = income - expenses;
                ourNOIAfterCapX = ourNOI - Property.CapXTotal;
            }

            if (Property?.Sellers.Any() ?? false)
            {
                var income = Property.Sellers.Where(x => x.Category.GetLineItemType() == UnderwritingType.Income).Sum(x => x.AnnualizedTotal);
                var expenses = Property.Sellers.Where(x => x.Category.GetLineItemType() == UnderwritingType.Expense).Sum(x => x.AnnualizedTotal);

                sellerNOI = income - expenses;
                sellerNOIAfterCapX = sellerNOI - Property.CapXTotal;
            }

            

            
        }
    }
}
