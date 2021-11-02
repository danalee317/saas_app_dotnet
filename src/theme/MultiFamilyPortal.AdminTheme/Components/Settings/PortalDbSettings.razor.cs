using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.AdminTheme.Components;
using MultiFamilyPortal.AdminTheme.Models;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;
using MultiFamilyPortal.Collections;
using System.Net.Http.Json;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Settings
{
    public partial class PortalDbSettings
    {
        [Inject]
        private HttpClient Client { get; set; }

        private ObservableRangeCollection<Setting> SiteSettings { get; set; } = new();
        protected override async Task OnInitializedAsync()
        {
            SiteSettings.ReplaceRange(await Client.GetFromJsonAsync<IEnumerable<Setting>>("/api/admin/settings"));
        }

        private async Task UpdateSetting(GridCommandEventArgs args)
        {
            var setting = args.Item as Setting;
            await Client.PostAsJsonAsync($"/api/admin/settings/save/{setting.Key}", setting);
            SiteSettings.ReplaceRange(await Client.GetFromJsonAsync<IEnumerable<Setting>>("/api/admin/settings"));
        }
    }
}
