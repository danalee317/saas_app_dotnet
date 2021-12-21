namespace MultiFamilyPortal.Configuration
{
    internal class SiteConfiguration
    {
        public string PostmarkApiKey { get; set; }

        public GoogleCaptchaOptions Captcha { get; set; }

        public StorageConfiguration Storage { get; set; }
    }
}
