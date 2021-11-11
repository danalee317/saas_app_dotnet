using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Components.Underwriting;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.AdminTheme.Pages.Properties.Underwriting
{
    [Authorize(Policy = PortalPolicy.UnderwritingViewer)]
    public partial class Index
    {
        [Inject]
        private HttpClient _client { get; set; }

        [Inject]
        private ILoggerFactory _loggerFactory { get; set; }

        [Inject]
        private ITimeZoneService _timezone { get; set; }

        private UnderwritingList underwritingList { get; set; }
        private UnderwriterResponse Profile;
        private string ProfileId;
        private DateTimeOffset Start = DateTimeOffset.Now.AddYears(-1);
        private DateTimeOffset End = DateTimeOffset.Now;
        private ObservableRangeCollection<UnderwriterResponse> Underwriters = new ObservableRangeCollection<UnderwriterResponse> { new UnderwriterResponse { DisplayName = "All" } };

        protected override async Task OnInitializedAsync()
        {
            Profile = Underwriters.First();
            ProfileId = Profile.Id;

            Start = await _timezone.GetLocalDateTime(Start);
            End = await _timezone.GetLocalDateTime(End);

            try
            {
                var underwriters = await _client.GetFromJsonAsync<IEnumerable<UnderwriterResponse>>("/api/admin/underwriting/underwriters");
                Underwriters.AddRange(underwriters);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("Underwriting");
                logger.LogError(ex, "Error while trying to get underwriters");
            }
            await Update();
        }

        private async Task Update()
        {
            Profile = Underwriters.First(x => x.Id == ProfileId);
            await underwritingList.Update();
        }
    }
}
