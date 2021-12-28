using MultiFamilyPortal.Dtos;

namespace MultiFamilyPortal.Services
{
    public interface ITimeZoneService
    {
        ValueTask<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime);

        DateTime GetLocalTimeByTimeZone(string timezone);

        List<TimezoneData> Timezones { get; }
    }
}
