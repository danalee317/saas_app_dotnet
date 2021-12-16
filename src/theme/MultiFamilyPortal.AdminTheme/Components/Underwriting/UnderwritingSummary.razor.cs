using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos.Underwriting;
using ReactiveUI;

namespace MultiFamilyPortal.AdminTheme.Components.Underwriting;

public partial class UnderwritingSummary
{
    [Parameter]
    public UnderwritingAnalysis Property { get; set; }

    private double _capRate;
    private double _debtCoverageRatio;
    private double _sellerCashOnCash;

    private ReactiveCommand<Unit, Unit> RefreshCommand;
    private CompositeDisposable _disposables = new CompositeDisposable();
    private const string _lowerColor = "#FFFF99";
    private const string _middleColor = "lightgreen";
    private const string _higherColor = "salmon";

    protected override void OnInitialized()
    {
        RefreshCommand = ReactiveCommand.CreateFromTask(Refresh).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.CapRate).Select(_ => Unit.Default).InvokeCommand(RefreshCommand).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.DebtCoverage).Select(_ => Unit.Default).InvokeCommand(RefreshCommand).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.SellerCashOnCash).Select(_ => Unit.Default).InvokeCommand(RefreshCommand).DisposeWith(_disposables);
    }

    private string GetDebtCoverageColor() => _debtCoverageRatio < 1.2 ? _higherColor :
        _debtCoverageRatio < 1.5 ? _lowerColor : _middleColor;
    private string GetCoCColor() =>
        _sellerCashOnCash < 0.10 ? _higherColor : (_sellerCashOnCash >= .10 && _sellerCashOnCash <= .12) ? _lowerColor : _middleColor;
    private async Task Refresh()
    {
        if (_capRate != Property.CapRate)
        {
            _capRate = Property.CapRate;
            await InvokeAsync(StateHasChanged);
        }

        if (_debtCoverageRatio != Property.DebtCoverage)
        {
            _debtCoverageRatio = Property.DebtCoverage;
            await InvokeAsync(StateHasChanged);
        }

        if (_sellerCashOnCash != Property.SellerCashOnCash)
        {
            _sellerCashOnCash = Property.SellerCashOnCash;
            await InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        if (!_disposables.IsDisposed)
            _disposables.Dispose();

        GC.SuppressFinalize(this);
    }
}
