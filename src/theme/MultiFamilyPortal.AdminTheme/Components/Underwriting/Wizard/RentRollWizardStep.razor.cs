using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting.Wizard
{
    public partial class RentRollWizardStep
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }
    }
}
