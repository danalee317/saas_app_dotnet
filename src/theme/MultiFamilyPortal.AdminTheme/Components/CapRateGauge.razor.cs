using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;
using ReactiveUI;

namespace MultiFamilyPortal.AdminTheme.Components
{
    public partial class CapRateGauge : IDisposable
    {
        private bool _disposedValue;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        private double CapRate;
        private double Max;
        private string Color;
        private ReactiveCommand<Unit, Unit> RefreshCommand;

        protected override void OnInitialized()
        {
            RefreshCommand = ReactiveCommand.CreateFromTask(Refresh).DisposeWith(_disposables);
            Property.WhenAnyValue(x => x.CapRate).Select(x => Unit.Default).InvokeCommand(RefreshCommand).DisposeWith(_disposables);
        }

        private async Task Refresh()
        {
            if (CapRate != Property.CapRate)
            {
                CapRate = Property.CapRate;
                Max = GetMax();
                Color = GetColor();
                await InvokeAsync(StateHasChanged);
            }
        }

        private string GetColor()
        {
            if (CapRate < .05)
                return "red";
            if (CapRate < .08)
                return "yellow";
            return "green";
        }

        private double GetMax()
        {
            if (CapRate < .15)
                return .15;
            if (CapRate < .25)
                return .25;
            if (CapRate < .40)
                return .40;
            if (CapRate < .60)
                return .60;
            if (CapRate < .80)
                return .80;
            if (CapRate <= 1)
                return 1;
            return CapRate;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _disposables.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
