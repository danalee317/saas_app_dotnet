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

        private UnderwritingGuidance _activeUnderwriting = new();
        private readonly ObservableRangeCollection<string> _markets = new();
        private PortalNotification notification;
        private readonly IEnumerable<CostType> _expenseTypes = Enum.GetValues<CostType>();
        private string _newMarket;
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
            if (string.IsNullOrWhiteSpace(_newMarket))
            { 
                notification.ShowError("Market name is required.");
                return;
            }

            try
            {
                foreach (var market in Guidance)
                {
                    market.Market = _newMarket;
                    market.Id = Guid.Empty;
                    await _client.PostAsJsonAsync($"/api/admin/underwriting/guidance", market);
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