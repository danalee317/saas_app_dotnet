using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.AspNetCore.Components;
using MultiFamilyPortal.Dtos;
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

    private ReactiveCommand<Unit, Unit> _refreshCommand;
    private CompositeDisposable _disposables = new();

    protected override void OnInitialized()
    {
        _refreshCommand = ReactiveCommand.CreateFromTask(Refresh).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.CapRate).Select(_ => Unit.Default).InvokeCommand(_refreshCommand).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.DebtCoverage).Select(_ => Unit.Default).InvokeCommand(_refreshCommand).DisposeWith(_disposables);
        Property.WhenAnyValue(x => x.SellerCashOnCash).Select(_ => Unit.Default).InvokeCommand(_refreshCommand).DisposeWith(_disposables);
    }

    private ColorCode GetDebtCoverageColor() => _debtCoverageRatio < 1.2 ? ColorCode.Warning :
        _debtCoverageRatio < 1.5 ? ColorCode.Default : ColorCode.Success;
    private ColorCode GetCoCColor() => _sellerCashOnCash < 0.10 ? ColorCode.Warning :
        _sellerCashOnCash is >= .10 and <= .12 ? ColorCode.Default : ColorCode.Success;

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
