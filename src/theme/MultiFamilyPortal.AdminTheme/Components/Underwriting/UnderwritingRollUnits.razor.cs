using Microsoft.AspNetCore.Components;
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
            if (Property?.Models != null)
              _rollUnits = Property.Models.Where(x => x.Units != null).SelectMany(x => x.Units);
        }
   }
}
