using System.Security.Claims;
using Microsoft.AspNetCore.Components;using Microsoft.AspNetCore.Http;
using MultiFamilyPortal.Data;

namespace MultiFamilyPortal.Themes.Internals{    internal class ThemeFactory : IThemeFactory    {        private IEnumerable<IPortalTheme> _themes { get; }        private IMFPContext _dbContext { get; }
        private NavigationManager _navigationManager { get; }
        private IHttpContextAccessor _contextAccessor { get; }        public ThemeFactory(IEnumerable<IPortalTheme> themes, IMFPContext dbContext, NavigationManager navigationManager, IHttpContextAccessor contextAccessor)        {            _themes = themes;            _dbContext = dbContext;
            _navigationManager = navigationManager;
            _contextAccessor = contextAccessor;        }

        private IPortalFrontendTheme GetFrontendTheme()
        {
            var defaultTheme = _dbContext.SiteThemes.FirstOrDefault(x => x.IsDefault == true);
            return _themes.OfType<IPortalFrontendTheme>().FirstOrDefault(x => x.Name == defaultTheme.Id);
        }

        private IPortalAdminTheme GetAdminTheme()
        {
            return _themes.OfType<IPortalAdminTheme>().FirstOrDefault();
        }        public IPortalTheme GetCurrentTheme()
        {
            var uri = new Uri(_navigationManager.Uri);


#if DEBUG
            if (uri.AbsolutePath.StartsWith("/admin"))
#else
            var user = _contextAccessor.HttpContext.User;            if (uri.AbsolutePath.StartsWith("/admin") &&
                user.IsInAnyRole(PortalRoles.Mentor, PortalRoles.Underwriter, PortalRoles.BlogAuthor, PortalRoles.PortalAdministrator))
#endif            {                return GetAdminTheme();            }            return GetFrontendTheme();
        }    }}