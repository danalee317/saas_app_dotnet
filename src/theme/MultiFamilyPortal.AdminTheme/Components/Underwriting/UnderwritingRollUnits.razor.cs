using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Collections;
using MultiFamilyPortal.CoreUI;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingRollUnits
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        private IEnumerable<UnderwritingAnalysisUnit> _rollUnits = Array.Empty<UnderwritingAnalysisUnit>();

        protected override void OnParametersSet()
        {
            if(Property?.Models?.SelectMany(x => x.Units) is not null) // Work on this later, don't waste time
              _rollUnits = Property?.Models?.SelectMany(x => x.Units);
        }
   }  
}
