using Microsoft.JSInterop;

namespace MultiFamilyPortal.Services
{
    internal class TimeZoneService : ITimeZoneService
    {
        private IJSRuntime _jsRuntime { get; }

        private TimeSpan? _userOffset;

        public TimeZoneService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async ValueTask<DateTimeOffset> GetLocalDateTime(DateTimeOffset dateTime)
        {
            if (_userOffset is null)
            {
                var offsetInMinutes = await _jsRuntime.InvokeAsync<int>("MFPortal.LocalTime");
                _userOffset = TimeSpan.FromMinutes(offsetInMinutes);
            }

            return dateTime.ToOffset(_userOffset.Value);
        }
    }
}
