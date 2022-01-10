using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;
using Telerik.DataSource.Extensions;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Financials
{
    public partial class UnderwritingIncomeForecast
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [CascadingParameter]
        private ClaimsPrincipal _user { get; set; }

        private bool _editable;

        private int _index = -1;

        protected override void OnInitialized()
        {
            _editable = _user.IsAuthorizedInPolicy(PortalPolicy.Underwriter);
        }

        private void OnForecastEditing(GridCommandEventArgs args)
        {
            _index = Property.IncomeForecast.IndexOf(args.Item);
        }

        private void OnForecastUpdated(GridCommandEventArgs args)
        {
            var updated = args.Item as UnderwritingAnalysisIncomeForecast;
            var existing = Property.IncomeForecast.ElementAt(_index);
            existing.FixedIncreaseOnRemainingUnits = updated.FixedIncreaseOnRemainingUnits;
            existing.IncreaseType = updated.IncreaseType;
            existing.OtherIncomePercent = updated.OtherIncomePercent;
            existing.OtherLossesPercent = updated.OtherLossesPercent;
            existing.PerUnitIncrease = updated.PerUnitIncrease;
            existing.UnitsAppliedTo = updated.UnitsAppliedTo;
            existing.UtilityIncreases = updated.UtilityIncreases;
            existing.Vacancy = updated.Vacancy;
            existing.Year = updated.Year;
            _index = -1;
        }
    }
}
