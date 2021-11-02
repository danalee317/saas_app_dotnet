using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingPropertyInfoTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private PortalNotification notification { get; set; }

        private readonly ObservableRangeCollection<string> _markets = new ();

        private readonly CostType[] CostTypes = new[] { CostType.PerDoor, CostType.Total };

        private IEnumerable<UnderwritingLoanType> UnderwritingLoanTypes { get;} = Enum.GetValues<UnderwritingLoanType>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                _markets.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<string>>("/api/admin/underwriting/markets"));
            }
            catch (Exception ex)
            {
                notification.ShowError($"{ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}