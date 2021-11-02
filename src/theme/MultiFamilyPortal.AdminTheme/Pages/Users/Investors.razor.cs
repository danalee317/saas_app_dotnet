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
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using System.Net.Http.Json;

namespace MultiFamilyPortal.AdminTheme.Pages.Users
{
    [Authorize(Policy = PortalPolicy.Underwriter)]
    public partial class Investors
    {
        [Inject]
        private HttpClient _client { get; set; }

        private readonly ObservableRangeCollection<UserAccountResponse> _data = new ObservableRangeCollection<UserAccountResponse>();

        protected override async Task OnInitializedAsync()
        {
            _data.ReplaceRange(await _client.GetFromJsonAsync<IEnumerable<UserAccountResponse>>("/api/admin/users/investors"));
        }
    }
}
