namespace MultiFamilyPortal.Themes.Internals{    public interface IThemeFactory    {        //IPortalFrontendTheme GetFrontendTheme();
        //IPortalAdminTheme GetAdminTheme();
        IPortalTheme GetCurrentTheme();    }}