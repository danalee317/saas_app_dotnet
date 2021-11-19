using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Blazor.Components;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingMarketRuleOfThumb
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private ObservableRangeCollection<UnderwritingGuidance> _rules = new();
        private PortalNotification _notification;
        private UnderwritingGuidance _selected;
        private double _currentCost;
        private double _min;
        private double _max;
        private bool showSelectedGuidance;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

            try
            {
                var guidanceList = await _client.GetFromJsonAsync<IEnumerable<UnderwritingGuidance>>($"/api/admin/underwriting/guidance?market={Property.Market}");
                _rules.ReplaceRange(guidanceList);
            }
            catch (Exception ex)
            {
                _notification.ShowError(ex.Message);
            }
        }

        private void ShowUpdateUnderwriting(GridCommandEventArgs args)
        {
            _selected = args.Item as UnderwritingGuidance;
            if(_selected.Type == CostType.PercentOfPurchase)
            {
                _min = _selected.Min * Property.PurchasePrice / 2;
                _max = _selected.Max * Property.PurchasePrice * 2;
            }
            else if(_selected.Type == CostType.PerDoor)
            {
                _min = _selected.Min * Property.Units / 2;
                _max = _selected.Max * Property.Units * 2;
            }
            else
            {
                _min = _selected.Min / 2;
                _max = _selected.Max * 2;
            }

            _currentCost = Property.Ours.Where(x => x.Category == _selected.Category)
                .Sum(x => x.AnnualizedTotal);

            if (_currentCost < _min)
                _min *= 0.7;
            if (_currentCost > _max)
                _max = _currentCost * 1.3;

            showSelectedGuidance = true;
        }
    }
}
