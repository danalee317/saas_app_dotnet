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
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Extensions;
using Telerik.Blazor;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingMortgageDialog
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public bool Update { get; set; }

        [Parameter]
        public UnderwritingAnalysisMortgage Mortgage { get; set; }

        [Parameter]
        public UnderwritingLoanType LoanType { get; set; }

        [Parameter]
        public EventCallback<UnderwritingAnalysisMortgage> MortgageChanged { get; set; }

        [Parameter]
        public EventCallback<UnderwritingAnalysisMortgage> OnSave { get; set; }

        private bool showBalloon;
        private string SaveBtn => Update ? "Update" : "Save";
        private bool enableLoanAmountEdit => LoanType == UnderwritingLoanType.Custom;

        private async Task SaveMortgage()
        {
            await OnSave.InvokeAsync(Mortgage);
        }
    }
}
