using Microsoft.AspNetCore.Identity;

namespace MultiFamilyPortal.Data.Models
{
    public class SiteUserRole : IdentityUserRole<string>
    {
        public virtual IdentityRole Role { get; set; }
        public virtual SiteUser User { get; set; }
    }
}
