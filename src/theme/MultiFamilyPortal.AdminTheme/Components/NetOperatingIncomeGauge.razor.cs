using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components
{
    public partial class NetOperatingIncomeGauge
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        private double Income => Property?.Ours is null ? 0 : Property.Ours.Where(x => x.Category.GetLineItemType() == UnderwritingType.Income).Sum(x => x.AnnualizedTotal);
        private double Expenses => Property?.Ours is null ? 0 : Property.Ours.Where(x => x.Category.GetLineItemType() == UnderwritingType.Expense).Sum(x => x.AnnualizedTotal);
        private string Color()
        {
            var ratio = Expenses / Income;
            if (ratio < 0.45)
                return "green";
            else if (ratio < 0.5)
                return "yellow";
            return "red";
        }
    }
}
