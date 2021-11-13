using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwrting;
using ReactiveUI;

namespace MultiFamilyPortal.AdminTheme.Components
{
    public partial class CashOnCashGauge : IDisposable
    {
        private bool _disposedValue;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Parameter]
        public UnderwritingAnalysis Property { get; set; }

        private double CashOnCash;
        private double Max;
        private string Color;
        private ReactiveCommand<Unit, Unit> RefreshCommand;

        protected override void OnInitialized()
        {
            RefreshCommand = ReactiveCommand.CreateFromTask(Refresh).DisposeWith(_disposables);
            Property.WhenAnyValue(x => x.CashOnCash).Select(x => Unit.Default).InvokeCommand(RefreshCommand).DisposeWith(_disposables);
        }

        private async Task Refresh()
        {
            if(CashOnCash != Property.CashOnCash)
            {
                CashOnCash = Property.CashOnCash;
                Max = GetMax();
                Color = GetColor();
                await InvokeAsync(StateHasChanged);
            }
        }

        private string GetColor()
        {
            if (CashOnCash < .07)
                return "red";
            if (CashOnCash < .12)
                return "yellow";
            return "green";
        }

        private double GetMax()
        {
            if (CashOnCash < .15)
                return .15;
            if (CashOnCash < .25)
                return .25;
            if (CashOnCash < .40)
                return .40;
            if (CashOnCash < .60)
                return .60;
            if (CashOnCash < .80)
                return .80;
            if (CashOnCash <= 1)
                return 1;
            return CashOnCash;
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
