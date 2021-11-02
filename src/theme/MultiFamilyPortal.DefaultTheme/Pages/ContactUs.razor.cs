using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.DefaultTheme.Pages
{
    public partial class ContactUs
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; }

        private ContactFormRequest form = new();
    }
}
