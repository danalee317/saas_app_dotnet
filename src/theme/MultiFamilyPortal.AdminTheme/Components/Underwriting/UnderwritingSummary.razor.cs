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
    private CompositeDisposable _disposables = new();

    protected override void OnInitialized()
    {
        RefreshCommand = ReactiveCommand.CreateFromTask(Refresh).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.CapRate).Select(_ => Unit.Default).InvokeCommand(RefreshCommand).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.DebtCoverage).Select(_ => Unit.Default).InvokeCommand(RefreshCommand).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.SellerCashOnCash).Select(_ => Unit.Default).InvokeCommand(RefreshCommand).DisposeWith(_disposables);
    }

    private static string GetColor(_colorCodes status) => status switch
    {
        _colorCodes.Info => "var(--bs-info)",
        _colorCodes.Warning => "salmon",
        _colorCodes.Default => "#ffd800",
        _colorCodes.Success => "#0e952f",
        _colorCodes.Light => "var(--bs-light)",
        _colorCodes.Dark => "var(--bs-dark)",
    };

    private string GetDebtCoverageColor() => _debtCoverageRatio < 1.2 ? GetColor(_colorCodes.Warning) :
        _debtCoverageRatio < 1.5 ? GetColor(_colorCodes.Default) : GetColor(_colorCodes.Success);
    private string GetCoCColor() => _sellerCashOnCash < 0.10 ? GetColor(_colorCodes.Warning) : 
        (_sellerCashOnCash >= .10 && _sellerCashOnCash <= .12) ? GetColor(_colorCodes.Default) : 
        GetColor(_colorCodes.Success);

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

    private enum _colorCodes
    {
        Info,
        Warning,
        Default,
        Success,
        Light,
        Dark,
    }
}
