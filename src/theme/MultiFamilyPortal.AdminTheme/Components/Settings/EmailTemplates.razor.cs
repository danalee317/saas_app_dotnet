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
    public partial class EmailTemplates
    {
        [Inject]
        private HttpClient _client { get; set; }

        private ObservableRangeCollection<EmailTemplate> _emailTemplates { get; set; } = new();

        private EmailTemplate _selected;
        private PortalNotification notification;

        protected override async Task OnInitializedAsync()
        {
            _emailTemplates.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<EmailTemplate>>("/api/admin/settings/email-templates"));
        }

        private void OnEditItem(GridCommandEventArgs args)
        {
            _selected = args.Item as EmailTemplate;
        }

        private async Task OnUpdateTemplate()
        {
            if (_selected is null)
                return;

            using var response = await _client.PostAsJsonAsync("/api/admin/settings/email-templates/update", _selected);

            if (response.IsSuccessStatusCode)
                notification.ShowSuccess("Email template was successfully updated");
            else
                notification.ShowError("Unable to update the email template");

            _selected = null;
        }
    }
}
