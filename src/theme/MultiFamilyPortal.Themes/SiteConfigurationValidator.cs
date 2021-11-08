namespace MultiFamilyPortal.Themes
{
    internal class SiteConfigurationValidator : ISiteConfigurationValidator
    {
        public IPortalTheme Theme { get; private set; }

        public void SetFirstRunTheme(IPortalTheme theme)
        {
            Theme = theme;
        }
    }
}
