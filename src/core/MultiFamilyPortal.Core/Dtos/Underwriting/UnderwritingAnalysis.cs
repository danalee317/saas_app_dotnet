using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text.Json.Serialization;
using DynamicData;
using MultiFamilyPortal.Converters;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace MultiFamilyPortal.Dtos.Underwriting
{
    [JsonConverter(typeof(ReactiveObjectConverter<UnderwritingAnalysis>))]
    public class UnderwritingAnalysis : ReactiveObject, IDisposable
    {
        public static object locker = new object();
        private readonly CompositeDisposable _disposable;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _sellersLineItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _sellersIncomeItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _sellersExpenseItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _ourLineItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _ourIncomeItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _ourExpenseItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisMortgage> _mortgages;
        private ReadOnlyObservableCollection<UnderwritingAnalysisModel> _models;
        private ReadOnlyObservableCollection<UnderwritingAnalysisIncomeForecast> _forecast;
        private ReadOnlyObservableCollection<UnderwritingAnalysisProjection> _projections;
        private readonly SourceCache<UnderwritingAnalysisLineItem, Guid> _sellersCache;
        private readonly SourceCache<UnderwritingAnalysisLineItem, Guid> _oursCache;
        private readonly SourceCache<UnderwritingAnalysisMortgage, Guid> _mortgageCache;
        private readonly SourceCache<UnderwritingAnalysisModel, Guid> _modelsCache;
        private readonly SourceCache<UnderwritingAnalysisIncomeForecast, int> _forecastCache;
        private readonly SourceCache<UnderwritingAnalysisProjection, int> _projectionCache;

        private ObservableAsPropertyHelper<double> _raise;
        private ObservableAsPropertyHelper<double> _debtCoverage;
        private ObservableAsPropertyHelper<double> _closingCostOther;
        private ObservableAsPropertyHelper<double> _closingCosts;
        private ObservableAsPropertyHelper<double> _aquisitionFee;
        private ObservableAsPropertyHelper<double> _costPerUnit;
        private ObservableAsPropertyHelper<double> _netPresentValue;
        private ObservableAsPropertyHelper<double> _initialRateOfReturn;
        private ObservableAsPropertyHelper<double> _totalAnnualReturn;

        // Seller
        private ObservableAsPropertyHelper<double> _sellerCashOnCash;
        private ObservableAsPropertyHelper<double> _sellerCapRate;
        private ObservableAsPropertyHelper<double> _sellerNOI;

        // Buyer
        private ObservableAsPropertyHelper<double> _capRate;
        private ObservableAsPropertyHelper<double> _cashOnCash;
        private ObservableAsPropertyHelper<double> _lossToLease;
        private ObservableAsPropertyHelper<double> _noi;
        private ObservableAsPropertyHelper<double> _capXTotal;
        private ObservableAsPropertyHelper<double> _pricePerUnit;
        private ObservableAsPropertyHelper<double> _pricePerSqFt;

        public UnderwritingAnalysis()
        {
            var throttle = TimeSpan.FromMilliseconds(100);
            _disposable = new();
            Notes = new List<UnderwritingAnalysisNote>();

            _sellersCache = new SourceCache<UnderwritingAnalysisLineItem, Guid>(x => x.Id);
            var sellersRefCount = _sellersCache.Connect()
                .RefCount()
                .AutoRefresh(x => x.AnnualizedTotal)
                .AutoRefresh(x => x.Category);
            sellersRefCount
                .SortBy(x => x.Category)
                .Bind(out _sellersLineItems)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposable);

            sellersRefCount
                .Filter(x => x.Category.GetLineItemType() == UnderwritingType.Income)
                .SortBy(x => x.Category)
                .Bind(out _sellersIncomeItems)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposable);

            sellersRefCount
                .Filter(x => x.Category.GetLineItemType() == UnderwritingType.Expense)
                .SortBy(x => x.Category)
                .Bind(out _sellersExpenseItems)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposable);

            _oursCache = new SourceCache<UnderwritingAnalysisLineItem, Guid>(x => x.Id);
            var ourRefCount = _oursCache.Connect()
                .RefCount()
                .AutoRefresh(x => x.AnnualizedTotal)
                .AutoRefresh(x => x.Category);
            ourRefCount
                .SortBy(x => x.Category)
                .Bind(out _ourLineItems)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposable);

            ourRefCount
                .Filter(x => x.Category.GetLineItemType() == UnderwritingType.Income)
                .SortBy(x => x.Category)
                .Bind(out _ourIncomeItems)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposable);

            ourRefCount
                .Filter(x => x.Category.GetLineItemType() == UnderwritingType.Expense)
                .SortBy(x => x.Category)
                .Bind(out _ourExpenseItems)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposable);

            _mortgageCache = new SourceCache<UnderwritingAnalysisMortgage, Guid>(x => x.Id);
            var mortgageRefCount = _mortgageCache.Connect()
                .RefCount()
                .AutoRefresh(x => x.AnnualDebtService);

            mortgageRefCount
                .Bind(out _mortgages)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposable);

            _modelsCache = new SourceCache<UnderwritingAnalysisModel, Guid>(x => x.Id);
            var modelRefCount = _modelsCache.Connect()
                .RefCount();
            modelRefCount
                .Bind(out _models)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposable);

            _forecastCache = new SourceCache<UnderwritingAnalysisIncomeForecast, int>(x => x.Year);
            _forecastCache.Connect()
                .RefCount()
                .AutoRefresh(x => x.Changed)
                .Bind(out _forecast)
                .DisposeMany()
                .Subscribe()
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.Units, x => x.PurchasePrice, CalculatePricePerUnit)
                .ToProperty(this, nameof(PricePerUnit), out _pricePerUnit)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.RentableSqFt, x => x.PurchasePrice, CalculatePricePerSqFt)
                .ToProperty(this, nameof(PricePerSqFt), out _pricePerSqFt)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.CapX, x => x.CapXType, x => x.Units, x => x.PurchasePrice, CalculateCapX)
                .ToProperty(this, nameof(CapXTotal), out _capXTotal)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.NOI, x => x.Raise, CalculateCashOnCash)
                .ToProperty(this, nameof(CashOnCash), out _cashOnCash)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.SellerNOI, x => x.Raise, CalculateCashOnCash)
                .ToProperty(this, nameof(SellerCashOnCash), out _sellerCashOnCash)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.AquisitionFeePercent, x => x.PurchasePrice,CalculateAquisitionFee)
                .ToProperty(this, nameof(AquisitionFee), out _aquisitionFee)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.ClosingCosts, x => x.ClosingCostOther, x => x.AquisitionFee, x => x.PurchasePrice, x => x.LTV, CalculateRaise)
                .ToProperty(this, nameof(Raise), out _raise)
                .DisposeWith(_disposable);

            ourRefCount
                .AutoRefreshOnObservable(_ =>
                    this.WhenAnyValue(x => x.DeferredMaintenance, x => x.CapXTotal, x => x.SECAttorney, x => x.ClosingCostMiscellaneous))
                .Batch(throttle)
                .ToCollection()
                .WithLatestFrom(this.WhenAnyValue(x => x.DeferredMaintenance, x => x.CapXTotal, x => x.SECAttorney, x => x.ClosingCostMiscellaneous, (deferredMaintenance, capX, secAttorney, closingMisc) => (deferredMaintenance, capX, secAttorney, closingMisc)), (collection, value) => (collection, value))
                .Select(x => CalculateClosingCostOther(x.collection, x.value.deferredMaintenance, x.value.capX, x.value.secAttorney, x.value.closingMisc))
                .ToProperty(this, nameof(ClosingCostOther), out _closingCostOther)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.PurchasePrice, x => x.ClosingCostPercent, CalculateClosingCosts)
                .ToProperty(this, nameof(ClosingCosts), out _closingCosts)
                .DisposeWith(_disposable);

            sellersRefCount
                .Batch(throttle)
                .ToCollection()
                .Select(CalculateNOI)
                .ToProperty(this, nameof(SellerNOI), out _sellerNOI)
                .DisposeWith(_disposable);

            ourRefCount
                .Batch(throttle)
                .ToCollection()
                .Select(CalculateNOI)
                .ToProperty(this, nameof(NOI), out _noi)
                .DisposeWith(_disposable);

            mortgageRefCount
                .AutoRefreshOnObservable(_ => this.WhenAnyValue(x => x.NOI))
                .Batch(throttle)
                .ToCollection()
                .WithLatestFrom(this.WhenAnyValue(x => x.NOI), (mortgages, noi) => (mortgages, noi))
                .Select(x => CalculateDebtCoverage(x.noi, x.mortgages))
                .ToProperty(this, nameof(DebtCoverage), out _debtCoverage)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.NOI, x => x.PurchasePrice, CalculateCapRate)
                .ToProperty(this, nameof(CapRate), out _capRate)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.SellerNOI, x => x.PurchasePrice, CalculateCapRate)
                .ToProperty(this, nameof(SellerCapRate), out _sellerCapRate)
                .DisposeWith(_disposable);

            this.WhenAnyValue(x => x.PurchasePrice, x => x.Units, (p, u) => p / u)
                .ToProperty(this, nameof(CostPerUnit), out _costPerUnit)
                .DisposeWith(_disposable);

            ourRefCount
                .AutoRefreshOnObservable(_ => this.WhenAnyValue(x => x.GrossPotentialRent))
                .Batch(throttle)
                .ToCollection()
                .WithLatestFrom(this.WhenAnyValue(x => x.GrossPotentialRent), (ours, gpr) => (ours, gpr))
                .Select(x => CalculateLossToLease(x.gpr, x.ours))
                .ToProperty(this, nameof(LossToLease), out _lossToLease)
                .DisposeWith(_disposable);

            _calculateVacancyAndManagement = ReactiveCommand.Create(OnCalculateGSRAndManagement)
                .DisposeWith(_disposable);
            ourRefCount
                .AutoRefreshOnObservable(_ => this.WhenAnyValue(x => x.Management, x => x.MarketVacancy, x=> x.PhysicalVacancy))
                .Batch(throttle)
                .ToCollection()
                .Select(_ => Unit.Default)
                .InvokeCommand(_calculateVacancyAndManagement)
                .DisposeWith(_disposable);

            _downpaymentCommand = ReactiveCommand.Create(OnDownpaymentCommandExecuted)
                .DisposeWith(_disposable);
            mortgageRefCount
                .AutoRefreshOnObservable(_ => this.WhenAnyValue(x => x.PurchasePrice, x => x.LoanType, x => x.LTV))
                .Batch(throttle)
                .ToCollection()
                .Select(_ => Unit.Default)
                .InvokeCommand(_downpaymentCommand)
                .DisposeWith(_disposable);

            _calculateLoanAmount = ReactiveCommand.Create(OnCalculateLoanAmount)
                .DisposeWith(_disposable);
            this.WhenAnyValue(x => x.LoanType, x => x.LTV, x => x.PurchasePrice, (lt, ltv, pp) => Unit.Default)
                .Throttle(throttle)
                .InvokeCommand(_calculateLoanAmount)
                .DisposeWith(_disposable);

            _updateIncomeForecast = ReactiveCommand.Create(OnUpdateIncomeForecast)
                .DisposeWith(_disposable);
            this.WhenAnyValue(x => x.HoldYears, x => x.IncomeForecast, (h, i) => Unit.Default)
                .Throttle(throttle)
                .InvokeCommand(_updateIncomeForecast)
                .DisposeWith(_disposable);

            _updateProjections = ReactiveCommand.Create(OnUpdateProjections);
        }

        public Guid Id { get; set; }

        public Guid? AssetId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        [Reactive]
        public DateTimeOffset StartDate { get; set; }

        public string Name { get; set; }

        public string Underwriter { get; set; }

        public string UnderwriterEmail { get; set; }

        public string Address { get; set; }

        public string Market { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double DesiredYield { get; set; }

        [Reactive]
        public int HoldYears { get; set; }

        [Reactive]
        public int Units { get; set; }

        [DisplayFormat(DataFormatString = "{0:0000}")]
        public int Vintage { get; set; }

        [Reactive]
        public UnderwritingLoanType LoanType { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double LTV { get; set; } = 0.8;

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double AskingPrice { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double StrikePrice { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OfferPrice { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PurchasePrice { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Downpayment { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:G}")]
        public int RentableSqFt { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public double GrossPotentialRent { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double PhysicalVacancy { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double MarketVacancy { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Management { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double CapX { get; set; }

        [Reactive]
        public CostType CapXType { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double OurEquityOfCF { get; set; } = 0.25;

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double AquisitionFeePercent { get; set; } = 0.05;

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double ClosingCostPercent { get; set; } = 0.03;

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double DeferredMaintenance { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double SECAttorney { get; set; }

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosingCostMiscellaneous { get; set; }

        public UnderwritingStatus Status { get; set; }

        public PropertyClass PropertyClass { get; set; }

        public PropertyClass NeighborhoodClass { get; set; }

        #region Bucketlist Notes

        public string GrossPotentialRentNotes { get; set; }

        public string LossToLeaseNotes { get; set; }

        public string GrossScheduledRentNotes { get; set; }

        public string PhysicalVacancyNotes { get; set; }

        public string ConcessionsNonPaymentNotes { get; set; }

        public string UtilityReimbursementNotes { get; set; }

        public string OtherIncomeNotes { get; set; }

        public string TaxesNotes { get; set; }

        public string MarketingNotes { get; set; }

        public string InsuranceNotes { get; set; }

        public string UtilityNotes { get; set; }

        public string RepairsMaintenanceNotes { get; set; }

        public string ContractServicesNotes { get; set; }

        public string PayrollNotes { get; set; }

        public string GeneralAdminNotes { get; set; }

        public string ManagementNotes { get; set; }

        #endregion Bucketlist Notes

        [Reactive]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double ReversionCapRate { get; set; }

        [Reactive]
        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Reversion { get; private set; }

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double NetPresentValue => _netPresentValue?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double InitialRateOfReturn => _initialRateOfReturn?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double TotalAnnualReturn => _totalAnnualReturn?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double CapXTotal => _capXTotal?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double CostPerUnit => _costPerUnit?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PricePerSqFt => _pricePerSqFt?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double PricePerUnit => _pricePerUnit?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double NOI => _noi?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double SellerNOI => _sellerNOI?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double LossToLease => _lossToLease?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CapRate => _capRate?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double SellerCapRate => _sellerCapRate?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CashOnCash => _cashOnCash?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double SellerCashOnCash => _sellerCashOnCash?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double AquisitionFee => _aquisitionFee?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosingCosts => _closingCosts?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double ClosingCostOther => _closingCostOther?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double DebtCoverage => _debtCoverage?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double Raise => _raise?.Value ?? 0;

        public UnderwritingAnalysisDealAnalysis DealAnalysis { get; set; }

        [AddMethod(nameof(AddSellerItems))]
        public IEnumerable<UnderwritingAnalysisLineItem> Sellers => _sellersLineItems;

        [JsonIgnore]
        public IEnumerable<UnderwritingAnalysisLineItem> SellerIncome => _sellersIncomeItems;

        [JsonIgnore]
        public IEnumerable<UnderwritingAnalysisLineItem> SellerExpense => _sellersExpenseItems;

        [AddMethod(nameof(AddOurItems))]
        public IEnumerable<UnderwritingAnalysisLineItem> Ours => _ourLineItems;

        [JsonIgnore]
        public IEnumerable<UnderwritingAnalysisLineItem> OurIncome => _ourIncomeItems;

        [JsonIgnore]
        public IEnumerable<UnderwritingAnalysisLineItem> OurExpense => _ourExpenseItems;

        [AddMethod(nameof(AddMortgages))]
        public IEnumerable<UnderwritingAnalysisMortgage> Mortgages => _mortgages;

        public List<UnderwritingAnalysisNote> Notes { get; set; }

        [AddMethod(nameof(AddModels))]
        public IEnumerable<UnderwritingAnalysisModel> Models => _models;

        public List<UnderwritingAnalysisCapitalImprovement> CapitalImprovements { get; set; }

        [AddMethod(nameof(ReplaceForecast))]
        public IEnumerable<UnderwritingAnalysisIncomeForecast> IncomeForecast => _forecast;

        [JsonIgnore]
        public IEnumerable<UnderwritingAnalysisProjection> Projections => _projections;

        private ReactiveCommand<Unit, Unit> _downpaymentCommand;
        private ReactiveCommand<Unit, Unit> _calculateVacancyAndManagement;
        private ReactiveCommand<Unit, Unit> _calculateLoanAmount;
        private ReactiveCommand<Unit, Unit> _updateIncomeForecast;
        private ReactiveCommand<Unit, Unit> _updateProjections;
        private bool _disposedValue;

        public void AddModel(UnderwritingAnalysisModel item)
        {
            if (item.Id == default)
                item.Id = Guid.NewGuid();

            _modelsCache.AddOrUpdate(item);
        }

        public void AddModels(IEnumerable<UnderwritingAnalysisModel> items)
        {
            foreach (var model in items.Where(x => x.Id == default))
                model.Id = Guid.NewGuid();

            _modelsCache.Edit(x => x.AddOrUpdate(items));
        }

        public void RemoveModel(UnderwritingAnalysisModel model)
        {
            _modelsCache.Remove(model);
        }

        public void AddMortgage(UnderwritingAnalysisMortgage mortgage)
        {
            _mortgageCache.AddOrUpdate(mortgage);
        }

        public void AddMortgages(IEnumerable<UnderwritingAnalysisMortgage> item)
        {
            _mortgageCache.Edit(x => x.AddOrUpdate(item));
        }

        public void RemoveMortgage(UnderwritingAnalysisMortgage mortgage)
        {
            _mortgageCache.Remove(mortgage.Id);
        }

        public void AddSellerItem(UnderwritingAnalysisLineItem item)
        {
            _sellersCache.AddOrUpdate(item);
        }

        public void AddSellerItems(IEnumerable<UnderwritingAnalysisLineItem> items)
        {
            _sellersCache.Edit(x => x.AddOrUpdate(items));
        }

        public void RemoveSellerItem(UnderwritingAnalysisLineItem item)
        {
            _sellersCache.Remove(item.Id);
        }

        public void AddOurItem(UnderwritingAnalysisLineItem item)
        {
            _oursCache.AddOrUpdate(item);
        }

        public void AddOurItems(IEnumerable<UnderwritingAnalysisLineItem> items)
        {
            _oursCache.Edit(x => x.AddOrUpdate(items));
        }

        public void ReplaceOurItems(IEnumerable<UnderwritingAnalysisLineItem> items)
        {
            _oursCache.Edit(x =>
            {
                x.Clear();
                x.AddOrUpdate(items.Select(x =>
                {
                    x.Id = Guid.NewGuid();
                    return x;
                }));
            });
        }

        public void RemoveOurItem(UnderwritingAnalysisLineItem item)
        {
            _oursCache.Remove(item.Id);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ReplaceForecast(IEnumerable<UnderwritingAnalysisIncomeForecast> items)
        {
            _forecastCache.Edit(x =>
            {
                x.Clear();
                x.AddOrUpdate(items.OrderBy(i => i.Year));
            });
        }

        private void OnCalculateLoanAmount()
        {
            if (Mortgages is null || LoanType != UnderwritingLoanType.Automatic)
                return;

            if(PurchasePrice <= 0)
            {
                if(Mortgages.Any())
                    _mortgageCache.Clear();
                return;
            }

            var loanAmount = PurchasePrice * LTV;
            var mortgage = Mortgages.FirstOrDefault();
            if(mortgage is null)
            {
                _mortgageCache.AddOrUpdate(new UnderwritingAnalysisMortgage
                {
                    InterestRate = 0.04,
                    LoanAmount = loanAmount,
                    Points = 0.01,
                    TermInYears = 30
                });
            }
            else if(mortgage.LoanAmount != loanAmount)
            {
                mortgage.LoanAmount = loanAmount;
            }
        }

        private void OnDownpaymentCommandExecuted()
        {
            if (PurchasePrice > 0 && LoanType == UnderwritingLoanType.NewLoan && Mortgages != null && Mortgages.Any())
            {
                var mortgageTotal = Mortgages.Sum(x => x.LoanAmount);
                LTV = mortgageTotal / PurchasePrice;
                Downpayment = PurchasePrice - mortgageTotal;
            }
            else if (PurchasePrice > 0 && LoanType == UnderwritingLoanType.Automatic)
            {
                Downpayment = PurchasePrice - (PurchasePrice * LTV);
            }
        }

        private void OnCalculateGSRAndManagement()
        {
            if (Ours is null)
                return;

            var gsr = Ours.Where(x => x.Category == UnderwritingCategory.GrossScheduledRent).Sum(x => x.AnnualizedTotal);

            if (gsr <= 0)
                return;

            var rate = Math.Max(0.05, Math.Min(MarketVacancy, PhysicalVacancy));
            var amount = rate * gsr;
            var vacancy = Ours.FirstOrDefault(x => x.Category == UnderwritingCategory.PhysicalVacancy);
            if (vacancy is null)
            {
                AddOurItem(new UnderwritingAnalysisLineItem
                {
                    Amount = amount,
                    Category = UnderwritingCategory.PhysicalVacancy,
                    Description = UnderwritingCategory.PhysicalVacancy.GetDisplayName(),
                    ExpenseType = ExpenseSheetType.T12,
                });
            }
            else if (vacancy.Amount != amount)
            {
                vacancy.Amount = amount;
            }

            var managementAmount = gsr * Management;
            var management = Ours.FirstOrDefault(x => x.Category == UnderwritingCategory.Management);
            if (management is null)
            {
                AddOurItem(new UnderwritingAnalysisLineItem
                {
                    Amount = managementAmount,
                    Category = UnderwritingCategory.Management,
                    Description = UnderwritingCategory.Management.GetDisplayName(),
                    ExpenseType = ExpenseSheetType.T12
                });
            }
            else if (management.Amount != managementAmount)
            {
                management.Amount = managementAmount;
            }
        }

        private static double CalculateDebtCoverage(double noi, IEnumerable<UnderwritingAnalysisMortgage> mortgages)
        {
            if (noi <= 0 || mortgages is null)
                return 0;

            var mortgageTotal = mortgages.Sum(x => x.AnnualDebtService);
            if (mortgageTotal > 0)
                return noi / mortgageTotal;

            return 0;
        }

        private double CalculateNOI(IEnumerable<UnderwritingAnalysisLineItem> items)
        {
            if (items is null)
                return 0;

            var gsr = items.Where(x => x.Category == UnderwritingCategory.GrossScheduledRent)
                .Sum(x => x.AnnualizedTotal);
            var vacancy = items.Where(x => x.Category == UnderwritingCategory.PhysicalVacancy)
                .Sum(x => x.AnnualizedTotal);
            var concessions = items.Where(x => x.Category == UnderwritingCategory.ConsessionsNonPayment)
                .Sum(x => x.AnnualizedTotal);
            var utilityReimbursement = items.Where(x => x.Category == UnderwritingCategory.UtilityReimbursement)
                .Sum(x => x.AnnualizedTotal);
            var otherIncome = items.Where(x => x.Category == UnderwritingCategory.OtherIncome || x.Category == UnderwritingCategory.OtherIncomeBad || x.Category == UnderwritingCategory.OtherIncomeOneTime)
                .Sum(x => x.AnnualizedTotal);

            var income = gsr - vacancy - concessions + utilityReimbursement + otherIncome;
            var expenses = items.Where(x => x.Category.GetLineItemType() == UnderwritingType.Expense).Sum(x => x.AnnualizedTotal);

            return income - expenses;
        }

        private void OnUpdateIncomeForecast()
        {
            var list = new List<UnderwritingAnalysisIncomeForecast>();

            if(IncomeForecast.Count() != HoldYears + 1)
            {
                var temp = IncomeForecast.ToArray();
                for (int i = 0; i < HoldYears + 1; i++)
                {
                    var forecast = temp.FirstOrDefault(x => x.Year == i);
                    list.Insert(i, forecast ?? new UnderwritingAnalysisIncomeForecast
                    {
                        Year = i,
                    });
                }

                ReplaceForecast(list);
            }
        }

        private void OnUpdateProjections()
        {
            if (Units < 1)
                return;

            var list = new List<UnderwritingAnalysisProjection>();
            var grossScheduledRent = Ours.Where(x => x.Category == UnderwritingCategory.GrossScheduledRent)
                .Sum(x => x.AnnualizedTotal);
            var concessionsNonPayment = Ours.Where(x => x.Category == UnderwritingCategory.ConsessionsNonPayment)
                .Sum(x => x.AnnualizedTotal);
            var otherIncome = Ours.Where(x => x.Category == UnderwritingCategory.OtherIncome || x.Category == UnderwritingCategory.OtherIncomeBad)
                .Sum(x => x.AnnualizedTotal);
            var utilityReimbursement = Ours.Where(x => x.Category == UnderwritingCategory.UtilityReimbursement)
                .Sum(x => x.AnnualizedTotal);

            var taxes = Ours.Where(x => x.Category == UnderwritingCategory.Taxes)
                .Sum(x => x.AnnualizedTotal);
            var insurance = Ours.Where(x => x.Category == UnderwritingCategory.Insurance)
                .Sum(x => x.AnnualizedTotal);
            var repairsMaint = Ours.Where(x => x.Category == UnderwritingCategory.RepairsMaintenance)
                .Sum(x => x.AnnualizedTotal);
            var generalAdmin = Ours.Where(x => x.Category == UnderwritingCategory.GeneralAdmin)
                .Sum(x => x.AnnualizedTotal);
            var marketing = Ours.Where(x => x.Category == UnderwritingCategory.Marketing)
                .Sum(x => x.AnnualizedTotal);
            var utility = Ours.Where(x => x.Category == UnderwritingCategory.UtilityReimbursement)
                .Sum(x => x.AnnualizedTotal);
            var contractServices = Ours.Where(x => x.Category == UnderwritingCategory.ContractServices)
                .Sum(x => x.AnnualizedTotal);
            var payroll = Ours.Where(x => x.Category == UnderwritingCategory.Payroll)
                .Sum(x => x.AnnualizedTotal);

            var debtService = Mortgages.Sum(x => x.AnnualDebtService);
            var capitalReserves = CapXTotal;

            for (int i = 0; i < HoldYears + 1; i++)
            {
                var factor = 1.0;
                if (i == 0)
                {
                    factor = (new DateTime(StartDate.Year, 1, 1) - StartDate.DateTime) / TimeSpan.FromDays(365);
                }

                var forecast = IncomeForecast.ElementAtOrDefault(i);
                if (forecast is null)
                    break;

                if ((forecast.UnitsAppliedTo > 0 && forecast.PerUnitIncrease > 0) || forecast.FixedIncreaseOnRemainingUnits > 0)
                {
                    var increase = forecast.IncreaseType switch
                    {
                        IncomeForecastIncreaseType.FixedAmount => forecast.UnitsAppliedTo * forecast.PerUnitIncrease,
                        _ => (forecast.UnitsAppliedTo / Units) * (grossScheduledRent * forecast.PerUnitIncrease),
                    };
                    increase += (Units - forecast.UnitsAppliedTo) * forecast.FixedIncreaseOnRemainingUnits;
                    grossScheduledRent += increase;
                }

                if (forecast.OtherIncomePercent > 0)
                {
                    otherIncome += otherIncome * forecast.OtherIncomePercent;
                }

                if (forecast.OtherLossesPercent > 0)
                {
                    // TODO: Determine how we want to apply Other Losses
                }

                utility += forecast.UtilityIncreases;

                var vacancyRate = forecast.Vacancy > 0 ? forecast.Vacancy : PhysicalVacancy;
                var vacancy = (grossScheduledRent * vacancyRate);
                var netCollectedRent = grossScheduledRent - vacancy;
                var management = Management * netCollectedRent;

                var egi = grossScheduledRent - vacancy - concessionsNonPayment + otherIncome + utilityReimbursement;
                var expenses = taxes + insurance + repairsMaint + generalAdmin + management + marketing + utility + contractServices + payroll;
                var noi = egi - expenses;
                var capRate = ReversionCapRate > 0 ? ReversionCapRate : CapRate - 0.01;
                if (capRate < 0)
                    capRate = 0.06;

                list.Add(new UnderwritingAnalysisProjection
                {
                    CapitalReserves = capitalReserves * factor,
                    ConcessionsNonPayment = concessionsNonPayment * factor,
                    ContractServices = contractServices * factor,
                    DebtService = debtService * factor,
                    GeneralAdmin = generalAdmin * factor,
                    GrossScheduledRent = grossScheduledRent * factor,
                    Insurance = insurance * factor,
                    Management = management * factor,
                    Marketing = marketing * factor,
                    OtherIncome = otherIncome * factor,
                    Payroll = payroll * factor,
                    RepairsMaintenance = repairsMaint * factor,
                    SalesPrice = noi / capRate,
                    Taxes = taxes * factor,
                    UtilityReimbursement = utilityReimbursement * factor,
                    Vacancy = vacancy * factor,
                    Year = StartDate.Year + i
                });
            }

            _projectionCache.Edit(x =>
            {
                x.Clear();
                x.AddOrUpdate(list);
            });

            Reversion = Projections.LastOrDefault()?.SalesPrice ?? PurchasePrice;
        }


        private static double CalculateCapRate(double noi, double purchasePrice)
        {
            if (purchasePrice <= 0)
                return 0;

            return noi / purchasePrice;
        }

        private static double CalculateCapX(double capXbasis, CostType costType, int units, double purchasePrice)
        {
            return costType switch
            {
                CostType.PercentOfPurchase => capXbasis * purchasePrice,
                CostType.PerDoor => capXbasis * units,
                _ => capXbasis
            };
        }

        public static double CalculateCashOnCash(double noi, double raise)
        {
            return noi / raise;
        }

        private static double CalculateClosingCostOther(IEnumerable<UnderwritingAnalysisLineItem> lineItems, double deferredMaintenance, double capXTotal, double secAttorney, double closingCostMiscellaneous)
        {
            return (AnnualOperatingExpenses(lineItems) / 6) + InsuranceTotal(lineItems) + deferredMaintenance + capXTotal + secAttorney + closingCostMiscellaneous;
        }

        private static double AnnualOperatingExpenses(IEnumerable<UnderwritingAnalysisLineItem> lineItems)
        {
            if (lineItems is null || !lineItems.Any(x => x.Category.GetLineItemType() == UnderwritingType.Expense))
                return 0;

            return lineItems.Where(x => x.Category.GetLineItemType() == UnderwritingType.Expense && x.Category != UnderwritingCategory.Insurance)
                .Sum(x => x.AnnualizedTotal);
        }

        private static double InsuranceTotal(IEnumerable<UnderwritingAnalysisLineItem> lineItems)
        {
            if (lineItems is null || !lineItems.Any(x => x.Category == UnderwritingCategory.Insurance))
                return 0;

            return lineItems.Where(x => x.Category == UnderwritingCategory.Insurance)
                .Sum(x => x.AnnualizedTotal);
        }

        private static double CalculateLossToLease(double grossPotentialRent, IEnumerable<UnderwritingAnalysisLineItem> lineItems)
        {
            if (grossPotentialRent <= 0 || lineItems is null || !lineItems.Any())
                return 0;

            var grossScheduledRent = lineItems.Where(x => x.Category == UnderwritingCategory.GrossScheduledRent).Sum(x => x.AnnualizedTotal);
            return grossPotentialRent - grossScheduledRent;
        }

        private static double CalculateRaise(double closingCosts, double closingCostOther, double aquisitionFee, double purchasePrice, double ltv)
        {
            return closingCostOther + closingCosts + aquisitionFee + ((1 - ltv) * purchasePrice);
        }

        private static double CalculatePricePerUnit(int units, double purchasePrice)
        {
            if (units < 1)
                return 0;

            return purchasePrice / units;
        }

        private static double CalculatePricePerSqFt(int rentableSqFt, double purchasePrice)
        {
            if (rentableSqFt < 1)
                return 0;

            return purchasePrice / rentableSqFt;
        }

        private static double CalculateAquisitionFee(double percent, double purchasePrice)
        {
            return percent * purchasePrice;
        }

        private static double CalculateClosingCosts(double purchasePrice, double percent)
        {
            return percent * purchasePrice;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _disposable.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~UnderwritingAnalysis()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
