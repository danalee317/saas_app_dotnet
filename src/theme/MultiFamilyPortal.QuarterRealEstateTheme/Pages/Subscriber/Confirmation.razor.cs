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
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.QuarterRealEstateTheme.Components;
using MultiFamilyPortal.Services;

namespace MultiFamilyPortal.QuarterRealEstateTheme.Pages.Subscriber
{
    public partial class Confirmation
    {
        [Parameter]
        public Guid confirmationCode { get; set; }

        [Inject]
        private IBlogSubscriberService _subscriberService { get; set; }

        private string email;

        protected override async Task OnInitializedAsync()
        {
            email = await _subscriberService.SubscriberConfirmation(confirmationCode);
        }
    }
}
