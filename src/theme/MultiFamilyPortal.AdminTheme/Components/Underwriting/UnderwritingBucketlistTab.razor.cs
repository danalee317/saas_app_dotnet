using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting
{
    public partial class UnderwritingBucketlistTab
    {
        [Parameter]
        public UnderwritingAnalysis Property { get; set; }
    }
}
