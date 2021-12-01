using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingMarketAddition
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILogger<UnderwritingMarketAddition> _logger { get; set; }

        [Parameter]
        public ObservableRangeCollection<UnderwritingGuidance> Guidance { get; set; }

        [Parameter]
        public EventCallback UpdateGuidance { get; set; }

        [Parameter]
        public EventCallback<string> NewHeight { get; set; }

        private readonly ObservableRangeCollection<string> _markets = new();
        private PortalNotification notification;
        private readonly IEnumerable<CostType> _expenseTypes = Enum.GetValues<CostType>();
        private readonly IEnumerable<UnderwritingCategory> _expenseCategories = Enum.GetValues<UnderwritingCategory>();
        protected override async Task OnInitializedAsync() => await LoadMarkets();
        
        private async Task LoadMarkets()
        {
            try
            {
                _markets.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<string>>("/api/admin/underwriting/markets"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acquring markets for autocomplete.");
            }
        }

        private async Task OnAddMarket()
        {
            try
            {
                foreach (var market in Guidance)
                {
                   await _client.PutAsJsonAsync($"/api/admin/underwriting/guidance/{market.Id}", market);
                }
                
                notification.ShowSuccess("Markets successfully updated.");
                _logger.LogInformation($"Markets added to guidance");
                await UpdateGuidance.InvokeAsync();
            }
            catch (Exception ex)
            {
                notification.ShowError("An unknown error occurred while updating the market");
                _logger.LogError(ex, "Error updating markets");
            }
        }
    }
}