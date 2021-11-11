
namespace MultiFamilyPortal.Services
{
    public interface ITimeZoneService
    {
        ValueTask<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime);
    }
}
