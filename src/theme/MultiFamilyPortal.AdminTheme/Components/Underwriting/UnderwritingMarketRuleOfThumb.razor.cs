using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Security.Claims;
using Humanizer;
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
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Extensions;
using MultiFamilyPortal.SideBarTheme.Components;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;
using Telerik.Blazor.Components.Upload;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingMarketRuleOfThumb
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        [Inject]
        private HttpClient _client { get; set; }

        private ObservableRangeCollection<UnderwritingGuidance> _rules = new();

        protected override async Task OnInitializedAsync()
        {

        }
    }
}
