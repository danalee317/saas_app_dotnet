using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Services
{
    public interface IBlogSubscriberService
    {
        Task<SubscriberResult> PostPublished(Post post, Uri baseUri);
        Task<SubscriberResult> CommentNotificationEmail(CommentNotification commentNotification);
    }
}
