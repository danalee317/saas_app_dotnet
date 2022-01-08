using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class CapXInput
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Parameter]
        public bool Enabled { get; set; } = true;

        [Parameter]
        public string Id { get; set; }

        protected override void OnParametersSet() => _capx = Property.CapX.ToString("C0");
        private string _capx;
        private readonly CostType[] _costTypes = {CostType.PerDoor, CostType.Total};

        private void FormatToPercentage()
        {
            Property.CapX = double.Parse(_capx, NumberStyles.AllowCurrencySymbol);
            _capx = Property.CapX.ToString("C0");
        }

        private void FormatToNumber()
        {
            _capx = double.Parse(_capx, NumberStyles.AllowCurrencySymbol).ToString("N0");
        }
    }
}
