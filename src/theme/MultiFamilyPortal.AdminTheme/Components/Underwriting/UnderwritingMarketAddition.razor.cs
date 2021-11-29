using System.Net;
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
        private readonly IEnumerable<CostType> ExpenseTypes = Enum.GetValues<CostType>();
        private readonly IEnumerable<UnderwritingCategory> ExpenseCategories = Enum.GetValues<UnderwritingCategory>();
        protected override async Task OnInitializedAsync() => await LoadMarkets();
        private UnderwritingGuidance _underwritingGuidance = new();
        private string WindowHeight = "0";
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

        private void TabChangedHandler(int newIndex)
        {
            var result = newIndex == 0 ? "900px" : "520px";
            NewHeight.InvokeAsync(result);
        }

        private void AutoFill()
        {
            var guidance = Guidance.FirstOrDefault(x => x.Category == _underwritingGuidance.Category && string.IsNullOrEmpty(x.Market));
            if (guidance != null)
            {
                _underwritingGuidance.Min = guidance.Min;
                _underwritingGuidance.Max = guidance.Max;
                _underwritingGuidance.Type = guidance.Type;
                _underwritingGuidance.IgnoreOutOfRange = guidance.IgnoreOutOfRange;
            }
        }

        private async Task OnAddMarket(UnderwritingGuidance guidance)
        {
            try
            {
                var response = await _client.PutAsJsonAsync($"/api/admin/underwriting/guidance/{guidance.Id}", guidance);
                var feedback = "";

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        feedback = "Error adding market";
                        notification.ShowWarning(feedback);
                        _logger.LogWarning(feedback);
                        break;
                    case HttpStatusCode.NoContent:
                        notification.ShowSuccess("Market successfully updated.");
                        _logger.LogInformation($"Market {guidance.Market} added to guidance {guidance.Id}");
                        await UpdateGuidance.InvokeAsync();
                        break;
                    default:
                        feedback = "An unknown error occurred while updating the market";
                        notification.ShowError(feedback);
                        _logger.LogWarning(feedback);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding market {market} to guidance {guidance}", guidance.Market, guidance.Id);
            }
        }

        private async Task OnCreateMarket()
        {
            try
            {
                var response = await _client.PostAsJsonAsync("/api/admin/underwriting/guidance", _underwritingGuidance);
                var feedback = "";

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        feedback = "Error adding market";
                        notification.ShowWarning(feedback);
                        _logger.LogWarning(feedback);
                        break;
                    case HttpStatusCode.NoContent:
                        notification.ShowSuccess("Market successfully updated.");
                        _logger.LogInformation($"Market added successfully");
                        await UpdateGuidance.InvokeAsync();
                        break;
                    default:
                        feedback = "An unknown error occurred while updating the market";
                        notification.ShowError(feedback);
                        _logger.LogWarning(feedback);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new market");
            }
        }

        private async Task OnDeleteMarket(UnderwritingGuidance guidance)
        {
            try
            {
                var response = await _client.DeleteAsync($"/api/admin/underwriting/guidance/{guidance.Id}");
                var feedback = "";

                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        feedback = "Error deleting market";
                        notification.ShowWarning(feedback);
                        _logger.LogWarning(feedback);
                        break;
                    case HttpStatusCode.NoContent:
                        notification.ShowSuccess("Market successfully deleted.");
                        _logger.LogInformation($"Market {guidance.Market} deleted from guidance {guidance.Id}");
                        await UpdateGuidance.InvokeAsync();
                        break;
                    default:
                        feedback = "An unknown error occurred while deleting the market";
                        notification.ShowError(feedback);
                        _logger.LogWarning(feedback);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting market {market} from guidance {guidance}", guidance.Market, guidance.Id);
            }
        }
    }
}