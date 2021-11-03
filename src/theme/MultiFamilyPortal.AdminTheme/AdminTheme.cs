using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.AdminTheme
{
    //public class AdminTheme : IPortalAdminTheme
    //{    //    public Type Layout { get; } = typeof(MainLayout);    //    public string Name { get; } = "Admin Theme";

    //    public Type _404 { get; } = typeof(Components._404);
    //    public string[] RequiredStyles { get; } = new[] { "_content/MultiFamilyPortal.AdminTheme/css/site.css" };
    //}

    public class AdminTheme : InspiniaTheme.InspiniaTheme, IPortalAdminTheme
    {
        public AdminTheme()
        {
        }

        public override string Name { get; } = "Admin Theme";
        public override Type SideBar { get; } = typeof(Layouts.InspiniaNavBar);
    }
}
