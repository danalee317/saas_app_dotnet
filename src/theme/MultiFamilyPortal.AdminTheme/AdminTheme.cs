﻿using Microsoft.AspNetCore.Components.Routing;
using MultiFamilyPortal.Authentication;
using MultiFamilyPortal.Themes;

namespace MultiFamilyPortal.AdminTheme
{
    public class AdminTheme : SideBarTheme.SideBarTheme, IPortalAdminTheme, IMenuProvider
    {
        public AdminTheme()
        {
        }

        public override string Name { get; } = "Admin Theme";
        public override Type SideBar { get; } = typeof(Layouts.NavMenu);
        public IEnumerable<RootMenuOption> Menu { get; } = new[]
        {
            new RootMenuOption
            {
                Title = "Dashboard",
                IconClass = "fa-duotone fa-tachometer-alt-fastest",
                Link = "/admin",
                Match = NavLinkMatch.All,
                Policy = PortalPolicy.AdminPortalViewer
            },
            new RootMenuOption
            {
                Title = "Content",
                IconClass = "fa-duotone fa-pen-to-square",
                Policy = PortalPolicy.Blogger,
                Children = new []
                {
                    new ChildMenuOption
                    {
                        Title = "Pages",
                        Link = "/admin/pages",
                        RequiredRole = PortalRoles.PortalAdministrator
                    },
                    new ChildMenuOption
                    {
                        Title = "Blogs",
                        Link = "/admin/posts"
                    },
                    new ChildMenuOption
                    {
                        Title = "New Post",
                        Link = "/admin/create-post"
                    }
                }
            },
            new RootMenuOption
            {
                Title = "Activity Log",
                IconClass = "fa-duotone fa-mobile",
                Link = "/admin/activity-log",
                Policy = PortalPolicy.UnderwritingViewer
            },
            new RootMenuOption
            {
                Title = "Properties",
                IconClass = "fa-duotone fa-city",
                Policy = PortalPolicy.UnderwritingViewer,
                Children = new []
                {
                    new ChildMenuOption
                    {
                        Title = "Underwriting",
                        Link = "/admin/properties/underwriting"
                    },
                    new ChildMenuOption
                    {
                        Title = "Portfolio",
                        Link = "/admin/properties/assets-under-management",
                        Policy = PortalPolicy.Underwriter
                    }
                }
            },
            new RootMenuOption
            {
                Title = "Users",
                IconClass = "fa-duotone fa-users",
                Policy = PortalPolicy.Underwriter,
                Children = new []
                {
                    new ChildMenuOption
                    {
                        Title = "Investors",
                        Link = "/admin/investors",
                        Match = NavLinkMatch.All
                    },
                    new ChildMenuOption
                    {
                        Title = "Prospects",
                        Link = "/admin/prospects",
                        Match = NavLinkMatch.All
                    },
                    new ChildMenuOption
                    {
                        Title = "Website Users",
                        Link = "/admin/users",
                        Match = NavLinkMatch.All,
                        RequiredRole = PortalRoles.PortalAdministrator
                    },
                    new ChildMenuOption
                    {
                        Title = "Subscribers",
                        Link = "/admin/subscribers",
                        Match = NavLinkMatch.All,
                        RequiredRole = PortalRoles.PortalAdministrator
                    }
                }
            },
            new RootMenuOption
            {
                Title = "Profile",
                IconClass = "fa-duotone fa-user",
                Link = "/admin/user-profile",
                Policy = PortalPolicy.AdminPortalViewer,
            },
            new RootMenuOption
            {
                Title = "Settings",
                IconClass = "fa-duotone fa-cogs",
                Link = "/admin/settings",
                RequiredRole = PortalRoles.PortalAdministrator
            }
        };
    }
}
