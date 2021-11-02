using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.AdminTheme.Layouts
{
    public partial class NavMenu
    {
        [CascadingParameter]
        private ISiteInfo SiteInfo { get; set; }

        private bool collapseNavMenu = true;
        private bool showContentMenu = false;
        private bool showPropertyMenu = false;
        private bool showUsersMenu = false;
        private string NavMenuCssClass => collapseNavMenu ? "collapse" : null;
        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        private void ToggleContentMenu()
        {
            showContentMenu = !showContentMenu;
            if (showContentMenu)
                showPropertyMenu = showUsersMenu = false;
        }

        private void TogglePropertyMenu()
        {
            showPropertyMenu = !showPropertyMenu;
            if (showPropertyMenu)
                showContentMenu = showUsersMenu = false;
        }

        private void ToggleUsersMenu()
        {
            showUsersMenu = !showUsersMenu;
            if (showUsersMenu)
                showContentMenu = showPropertyMenu = false;
        }
    }
}
