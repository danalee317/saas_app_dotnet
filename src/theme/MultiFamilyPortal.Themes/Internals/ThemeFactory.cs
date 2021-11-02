﻿using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Data;

namespace MultiFamilyPortal.Themes.Internals
        private NavigationManager _navigationManager { get; }
        private IHttpContextAccessor _contextAccessor { get; }
            _navigationManager = navigationManager;
            _contextAccessor = contextAccessor;

        private IPortalFrontendTheme GetFrontendTheme()
        {
            var defaultTheme = _dbContext.SiteThemes.FirstOrDefault(x => x.IsDefault == true);
            return _themes.OfType<IPortalFrontendTheme>().FirstOrDefault(x => x.Name == defaultTheme.Id);
        }

        private IPortalAdminTheme GetAdminTheme()
        {
            return _themes.OfType<IPortalAdminTheme>().FirstOrDefault();
        }
        {
            var uri = new Uri(_navigationManager.Uri);


#if DEBUG
            if (uri.AbsolutePath.StartsWith("/admin"))
#else
            var user = _contextAccessor.HttpContext.User;
                user.IsInAnyRole(PortalRoles.Mentor, PortalRoles.Underwriter, PortalRoles.BlogAuthor, PortalRoles.PortalAdministrator))
#endif
        }