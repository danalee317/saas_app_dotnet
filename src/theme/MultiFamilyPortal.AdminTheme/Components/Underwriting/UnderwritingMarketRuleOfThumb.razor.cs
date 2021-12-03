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
        private double _smallStep;
        private double _largeStep;
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

            var propertyExpenses = Property.Ours.Where(x => x.Category == _selected.Category);
            if (propertyExpenses.Any())
                _currentCost = propertyExpenses.Sum(x => x.AnnualizedTotal);
            else
                _currentCost = _min;


            if (_currentCost < _min)
                _min *= 0.7;
            if (_currentCost > _max)
                _max = _currentCost * 1.3;

            var numberOfMajorPoints = 50;
            var numberOfMinorPoints = 10;

            _smallStep = Math.Round(Math.Floor((_max - _min) / numberOfMajorPoints), 2);
            _largeStep = Math.Round(Math.Floor((_max - _min) / numberOfMinorPoints), 2);
            showSelectedGuidance = true;
        }
        
        private void InnerUpdate() =>  Property.Ours.Where(x => x.Category == _selected.Category)
            .FirstOrDefault().Amount = _currentCost;
    }
}
