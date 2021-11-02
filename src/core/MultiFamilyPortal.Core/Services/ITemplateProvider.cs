using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Services
{
    public interface ITemplateProvider
    {
        Task<TemplateResult> GetSubscriberNotification(SubscriberNotification notification);
        Task<TemplateResult> ContactUs(ContactFormEmailNotification notification);
    }
}
