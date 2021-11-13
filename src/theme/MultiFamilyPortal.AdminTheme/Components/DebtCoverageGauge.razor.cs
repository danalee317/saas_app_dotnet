using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwrting;
using ReactiveUI;

namespace MultiFamilyPortal.AdminTheme.Components
{
    public partial class DebtCoverageGauge : IDisposable
    {
        private bool _disposedValue;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        private double DebtCoverageRatio { get; set; }
        private double Max;
        private string Color;
        private ReactiveCommand<Unit, Unit> RefreshCommand;

        protected override void OnInitialized()
        {
            RefreshCommand = ReactiveCommand.CreateFromTask(Refresh).DisposeWith(_disposables);
            Property.WhenAnyValue(x => x.DebtCoverage).Select(x => Unit.Default).InvokeCommand(RefreshCommand).DisposeWith(_disposables);
        }

        private async Task Refresh()
        {
            if (DebtCoverageRatio != Property.DebtCoverage)
            {
                DebtCoverageRatio = Property.DebtCoverage;
                Max = GetMax();
                Color = GetColor();
                await InvokeAsync(StateHasChanged);
            }
        }

        private string GetColor()
        {
            if (DebtCoverageRatio < 1.2)
                return "red";
            if (DebtCoverageRatio < 1.5)
                return "yellow";
            return "green";
        }

        private double GetMax()
        {
            if (DebtCoverageRatio < 2)
                return 2;
            if (DebtCoverageRatio < 4)
                return 4;
            if (DebtCoverageRatio < 6)
                return 6;
            if (DebtCoverageRatio < 10)
                return 10;
            return DebtCoverageRatio;
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
