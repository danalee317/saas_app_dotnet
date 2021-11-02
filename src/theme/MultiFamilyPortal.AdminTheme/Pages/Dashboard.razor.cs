using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.CoreUI;

namespace MultiFamilyPortal.AdminTheme.Pages
{
    [Authorize(Policy = PortalPolicy.AdminPortalViewer)]
    public partial class Dashboard
    {
        [Inject]
        private HttpClient Client { get; set; }

        [Inject]
        private IHttpContextAccessor ContextAccessor { get; set; }

        public HttpContext HttpContext => ContextAccessor.HttpContext;

        private DashboardAnalyticsResponse Analytics { get; set; } = new DashboardAnalyticsResponse();

        private PortalNotification notification { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Analytics = await Client.GetFromJsonAsync<DashboardAnalyticsResponse>("/api/admin/dashboard/analytics");
            }
            catch (Exception ex)
            {
                notification.ShowError($"{ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}
