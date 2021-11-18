using System.Net.Http.Json;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MultiFamilyPortal.AdminTheme.Components.Underwriting;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
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
        private Guid PropertyId;
        private DateTimeOffset Start = DateTimeOffset.Now.AddYears(-1);
        private DateTimeOffset End = DateTimeOffset.Now;
        private ObservableRangeCollection<UnderwriterResponse> Underwriters = new ObservableRangeCollection<UnderwriterResponse> { new UnderwriterResponse { DisplayName = "All" } };
        private ObservableRangeCollection<ProspectPropertyResponse> Prospects = new ();
        private ObservableRangeCollection<ProspectPropertyResponse> FilteredProspects = new ();
        private PortalNotification notification { get; set; }
        protected override async Task OnInitializedAsync()
        {
            Profile = Underwriters.First();
            ProfileId = Profile.Id;

            Start = await _timezone.GetLocalDateTime(Start);
            End = await _timezone.GetLocalDateTime(End);

            await GetUnderwriters();
            await UpdateAsync();
            await GetUnderWrites();
        }

        private async Task GetUnderwriters()
        {
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
        }

        private async Task GetUnderWrites()
        {
            try
            {
                var start = HttpUtility.UrlEncode(Start.ToString());
                var end = HttpUtility.UrlEncode(End.ToString());
                var underwriterId = Profile?.Id;
                var properties = await _client.GetFromJsonAsync<IEnumerable<ProspectPropertyResponse>>($"/api/admin/underwriting?start={start}&end={end}&underwriterId={underwriterId}");

                Prospects.ReplaceRange(properties);
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger("Underwriting");
                string message = "An error occurred while attempting to load the properties.";
                notification.ShowError(message);
                logger.LogError($"{message} : {ex.Message}");
            }
        }

        public async Task UpdateAsync()
        {
            Profile = Underwriters.First(x => x.Id == ProfileId);
            await GetUnderWrites();
            Prospects.Add(new ProspectPropertyResponse { Name = "All", Id = Guid.Empty } );
            
            if (PropertyId != Guid.Empty)
                FilteredProspects.ReplaceRange(Prospects.Where(x => x.Id == PropertyId));
            else
                FilteredProspects.ReplaceRange(Prospects.Where(x => x.Id != Guid.Empty));
        }
    }
}
