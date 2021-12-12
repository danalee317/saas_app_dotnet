using System.Collections.ObjectModel;
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
    [JsonConverter(typeof(UnderwritingAnalysisConverter))]
    public class UnderwritingAnalysis : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _disposable;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _sellersLineItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _sellersIncomeItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _sellersExpenseItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _ourLineItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _ourIncomeItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisLineItem> _ourExpenseItems;
        private ReadOnlyObservableCollection<UnderwritingAnalysisMortgage> _mortgages;
        private readonly SourceCache<UnderwritingAnalysisLineItem, Guid> _sellersCache;
        private readonly SourceCache<UnderwritingAnalysisLineItem, Guid> _oursCache;
        private readonly SourceCache<UnderwritingAnalysisMortgage, Guid> _mortgageCache;

        private ObservableAsPropertyHelper<double> _raise;
        private ObservableAsPropertyHelper<double> _debtCoverage;
        private ObservableAsPropertyHelper<double> _closingCostOther;
        private ObservableAsPropertyHelper<double> _closingCosts;
        private ObservableAsPropertyHelper<double> _aquisitionFee;
        private ObservableAsPropertyHelper<double> _costPerUnit;
        private ObservableAsPropertyHelper<double> _reversion;
        private ObservableAsPropertyHelper<double> _reversionCapRate;
        private ObservableAsPropertyHelper<double> _netPresentValue;
        private ObservableAsPropertyHelper<double> _initialRateOfReturn;
        private ObservableAsPropertyHelper<double> _totalAnnualReturn;

        // Seller
        private ObservableAsPropertyHelper<double> _sellerIncome;
        private ObservableAsPropertyHelper<double> _sellerExpenses;
        private ObservableAsPropertyHelper<double> _sellerCashOnCash;
        private ObservableAsPropertyHelper<double> _sellerCapRate;
        private ObservableAsPropertyHelper<double> _sellerNOI;

        // Buyer
        private ObservableAsPropertyHelper<double> _ourIncome;
        private ObservableAsPropertyHelper<double> _ourExpenses;
        private ObservableAsPropertyHelper<double> _capRate;
        private ObservableAsPropertyHelper<double> _cashOnCash;
        private ObservableAsPropertyHelper<double> _lossToLease;
        private ObservableAsPropertyHelper<double> _noi;
        private ObservableAsPropertyHelper<double> _capXTotal;
        private ObservableAsPropertyHelper<double> _pricePerUnit;
        private ObservableAsPropertyHelper<double> _pricePerSqFt;

        public UnderwritingAnalysis()
        {
            var throttle = TimeSpan.FromMilliseconds(150);
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

            _calculateVacancyAndManagement = ReactiveCommand.Create(OnCalculateGSRAndManagement);
            ourRefCount
                .AutoRefreshOnObservable(_ => this.WhenAnyValue(x => x.Management, x => x.MarketVacancy, x=> x.PhysicalVacancy))
                .Batch(TimeSpan.FromSeconds(0.5))
                .ToCollection()
                .Select(_ => Unit.Default)
                .InvokeCommand(_calculateVacancyAndManagement)
                .DisposeWith(_disposable);

            _downpaymentCommand = ReactiveCommand.Create(OnDownpaymentCommandExecuted);
            mortgageRefCount
                .AutoRefreshOnObservable(_ => this.WhenAnyValue(x => x.PurchasePrice, x => x.LoanType, x => x.LTV))
                .Batch(TimeSpan.FromSeconds(1))
                .ToCollection()
                .Select(_ => Unit.Default)
                .InvokeCommand(_downpaymentCommand)
                .DisposeWith(_disposable);

            _calculateLoanAmount = ReactiveCommand.Create(OnCalculateLoanAmount);
            this.WhenAnyValue(x => x.LoanType, x => x.LTV, x => x.PurchasePrice, (lt, ltv, pp) => Unit.Default)
                .Throttle(TimeSpan.FromSeconds(1))
                .InvokeCommand(_calculateLoanAmount)
                .DisposeWith(_disposable);
        }

        public Guid Id { get; set; }

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

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:P}")]
        public double ReversionCapRate => _reversionCapRate?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double Reversion => _reversion?.Value ?? 0;

        [JsonIgnore]
        [DisplayFormat(DataFormatString = "{0:N0}")]
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

        public UnderwritingAnalysisBucketList BucketList { get; set; }

        public IEnumerable<UnderwritingAnalysisLineItem> Sellers => _sellersLineItems;
        public IEnumerable<UnderwritingAnalysisLineItem> SellerIncome => _sellersIncomeItems;
        public IEnumerable<UnderwritingAnalysisLineItem> SellerExpense => _sellersExpenseItems;
        public IEnumerable<UnderwritingAnalysisLineItem> Ours => _ourLineItems;
        public IEnumerable<UnderwritingAnalysisLineItem> OurIncome => _ourIncomeItems;
        public IEnumerable<UnderwritingAnalysisLineItem> OurExpense => _ourExpenseItems;
        public IEnumerable<UnderwritingAnalysisMortgage> Mortgages => _mortgages;
        public List<UnderwritingAnalysisNote> Notes { get; set; }
        public List<UnderwritingAnalysisModel> Models { get; set; }
        public List<UnderwritingAnalysisCapitalImprovement> CapitalImprovements { get; set; }

        private ReactiveCommand<Unit, Unit> _downpaymentCommand;
        private ReactiveCommand<Unit, Unit> _calculateVacancyAndManagement;
        private ReactiveCommand<Unit, Unit> _calculateLoanAmount;
        private bool _disposedValue;

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

            var income = items.Where(x => x.Category.GetLineItemType() == UnderwritingType.Income).Sum(x => x.AnnualizedTotal);
            var expenses = items.Where(x => x.Category.GetLineItemType() == UnderwritingType.Expense).Sum(x => x.AnnualizedTotal);

            if (income <= 0 || expenses <= 0)
                return 0;

            return income - expenses;
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
