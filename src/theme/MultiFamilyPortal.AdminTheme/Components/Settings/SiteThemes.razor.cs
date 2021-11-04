using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MultiFamilyPortal.AdminTheme.Components;
using MultiFamilyPortal.AdminTheme.Models;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Extensions;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class SiteThemes
    {
        [Inject]
        private HttpClient _client { get; set; }

        private readonly ObservableRangeCollection<SiteTheme> _themes = new();

        private PortalNotification notification;

        protected override Task OnInitializedAsync() => Update();

        private async Task Update()
        {
            _themes.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<SiteTheme>>("/api/admin/settings/themes"));
        }

        private async Task SetDefault(GridCommandEventArgs args)
        {
            var theme = args.Item as SiteTheme;
            using var response = await _client.PostAsJsonAsync("/api/admin/settings/themes/default", theme);

            if (response.IsSuccessStatusCode)
                notification.ShowSuccess($"Site Theme successfully updated to {theme.Id}");
            else
                notification.ShowError("An error occurred while attempting to update the default theme");

            await Update();
        }

    }
}
