using Microsoft.AspNetCore.Components;

namespace MultiFamilyPortal.CoreUI
{
    public partial class PercentageInput
    {
        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public double Value { get; set; }

        [Parameter]
        public double Step { get; set; }

        [Parameter]
        public double Min { get; set; }

        [Parameter]
        public double Max { get; set; }

        [Parameter]
        public EventCallback<double> OnValueChanged { get; set; }

        private async Task HandleValueChangedAsync()
        {
            Value /= 100;
            await OnValueChanged.InvokeAsync(Value);
        }
    }
}
