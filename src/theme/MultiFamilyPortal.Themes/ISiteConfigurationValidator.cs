namespace MultiFamilyPortal.Themes
{
    public interface ISiteConfigurationValidator
    {
        IPortalTheme Theme { get; }

        void SetFirstRunTheme(IPortalTheme theme);
    }
}
