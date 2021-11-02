using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.DefaultTheme.Pages
{
    public partial class InvestWithUs
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; }

        private InvestorInquiryRequest form = new();
    }
}
