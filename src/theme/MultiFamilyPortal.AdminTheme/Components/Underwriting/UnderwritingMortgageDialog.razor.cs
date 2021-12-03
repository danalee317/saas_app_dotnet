using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingMortgageDialog
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public bool Update { get; set; }

        [Parameter]
        public UnderwritingAnalysisMortgage Mortgage { get; set; }

        [Parameter]
        public UnderwritingLoanType LoanType { get; set; }

        [Parameter]
        public EventCallback<UnderwritingAnalysisMortgage> MortgageChanged { get; set; }

        [Parameter]
        public EventCallback<UnderwritingAnalysisMortgage> OnSave { get; set; }

        private bool showBalloon;
        private double _showDecimals = 0.01;
        private string SaveBtn => Update ? "Update" : "Save";
        private bool enableLoanAmountEdit => LoanType == UnderwritingLoanType.Custom;

        private async Task SaveMortgage()
        {
            await OnSave.InvokeAsync(Mortgage);
        }

        private void ShowRealValue() => _showDecimals = 0.01;

        private void ShowPercentage(double value)
        {
            _showDecimals = 1;
            Mortgage.InterestRate = value/100;
        }
    }
}
