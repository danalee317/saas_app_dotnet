using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;

namespace MultiFamilyPortal.Helpers.Reports;
public static class GenerateAssumptionsBuilder
{
    public static void GenerateAssumptions(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var cellPadding = 22;
        var page = document.Pages.AddPage();
        var pageTwo = document.Pages.AddPage();
        var pageThree = document.Pages.AddPage();
        var pageFour = document.Pages.AddPage();

        var editor = new FixedContentEditor(page);
        var editorTwo = new FixedContentEditor(pageTwo);
        var editorThree = new FixedContentEditor(pageThree);
        var editorFour = new FixedContentEditor(pageFour);

        page.Size = ReportBuilder.LetterSizeHorizontal;
        pageTwo.Size = page.Size;
        pageThree.Size = page.Size;
        pageFour.Size = page.Size;

        var pageOneTableWidth = page.Size.Width - 2 * ReportBuilder.PageMargin - 400;
        var pageTwoTableWidth = page.Size.Width / 2 - 2 * ReportBuilder.PageMargin;
        var tableWidth = page.Size.Width / 3 - 60;
        var blackBorder = new Border(1, ReportBuilder.DarkColor);

        ReportBuilder.Header(page, "Assumptions");

        CriteriaTable(page, editor, property, blackBorder, pageOneTableWidth, cellPadding);
        DistributionTable(page, editor, property, blackBorder, pageOneTableWidth, cellPadding);
        ReversionTable(page, editor, property, blackBorder, pageOneTableWidth, cellPadding);
        EquityTable(pageTwo, editorTwo, property, blackBorder, pageTwoTableWidth, cellPadding - 5);
        CashFlowTable(pageTwo, editorTwo, property, blackBorder, pageTwoTableWidth, cellPadding - 5);
        PrimaryTable(pageThree, editorThree, property, blackBorder, tableWidth, cellPadding - 8);
        SupplementalTable(pageThree, editorThree, property, blackBorder, tableWidth, cellPadding - 8);
        RefinanceTable(pageThree, editorThree, property, blackBorder, tableWidth, cellPadding - 8);
        CapitalImprovementsTable(pageFour, editorFour, property, blackBorder, tableWidth, cellPadding);

        ReportBuilder.Footer(page, property.Name);
        ReportBuilder.Footer(pageTwo, property.Name);
        ReportBuilder.Footer(pageThree, property.Name);
        ReportBuilder.Footer(pageFour, property.Name);
    }

    private static void CriteriaTable(RadFixedPage page,
                                      FixedContentEditor editor,
                                      UnderwritingAnalysis property,
                                      Border border,
                                      double width,
                                      double padding = 20,
                                      double headerSize = 18)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
        };

        table.Borders = new TableBorders(border);

        var header = table.Rows.AddTableRow();
        header.BasicCell("Investment Criteria", true, ReportBuilder.HeaderColor, HorizontalAlignment.Left, 2);

        var data = table.Rows.AddTableRow();
        data.BasicCell("Hold Period", false);
        data.BasicCell(property.HoldYears.ToString(), false);

        editor.Position.Translate(page.Size.Width / 2 - width / 2, 150);
        editor.DrawTable(table, new Size(width, double.PositiveInfinity));
    }

    private static void DistributionTable(RadFixedPage page,
                                          FixedContentEditor editor,
                                          UnderwritingAnalysis property,
                                          Border border,
                                          double width,
                                          double padding = 20,
                                          double headerSize = 18)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
        };
        table.Borders = new TableBorders(border);

        var header = table.Rows.AddTableRow();
        header.BasicCell("Equity Distribution", true, ReportBuilder.HeaderColor, HorizontalAlignment.Left, 2);

        var assetEquity = table.Rows.AddTableRow();
        assetEquity.BasicCell("Asset Mgr Equity", false);
        assetEquity.BasicCell(property.OurEquityOfCF.ToString("P2"), false);

        var investorEquity = table.Rows.AddTableRow();
        investorEquity.BasicCell("Investor Equity", false, ReportBuilder.PrimaryColor);
        investorEquity.BasicCell((1 - property.OurEquityOfCF).ToString("P2"), false, ReportBuilder.PrimaryColor);

        editor.Position.Translate(page.Size.Width / 2 - width / 2, 300);
        editor.DrawTable(table, new Size(width, double.PositiveInfinity));
    }

    private static void ReversionTable(RadFixedPage page,
                                      FixedContentEditor editor,
                                      UnderwritingAnalysis property,
                                      Border border,
                                      double width,
                                      double padding = 20,
                                      double headerSize = 18)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
        };
        table.Borders = new TableBorders(border);

        var header = table.Rows.AddTableRow();
        header.BasicCell("Reversion", true, ReportBuilder.HeaderColor, HorizontalAlignment.Left, 2);

        var capRate = table.Rows.AddTableRow();
        capRate.BasicCell("Reversion Cap Rate", false);
        capRate.BasicCell(property.ReversionCapRate.ToString("P2"), false);

        var commission = table.Rows.AddTableRow();
        commission.BasicCell("Commission", false, ReportBuilder.PrimaryColor);
        commission.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Commission

        var reversionTransferTax = table.Rows.AddTableRow();
        reversionTransferTax.BasicCell("Transfer Tax", false);
        reversionTransferTax.BasicCell(" - ", false); // TODO : Add Transfer Tax

        editor.Position.Translate(page.Size.Width / 2 - width / 2, 500);
        editor.DrawTable(table, new Size(width, double.PositiveInfinity));
    }

    private static void EquityTable(RadFixedPage page,
                                    FixedContentEditor editor,
                                    UnderwritingAnalysis property,
                                    Border border,
                                    double width,
                                    double padding = 20,
                                    double headerSize = 18)
    {
        var asr = new AssumptionsReport(property);
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
        };
        table.Borders = new TableBorders(border);

        var header = table.Rows.AddTableRow();
        var headerTitle = header.Cells.AddTableCell();
        headerTitle.ColumnSpan = 2;
        headerTitle.Background = ReportBuilder.HeaderColor;
        headerTitle.PreferredWidth = width;
        var headerTitleBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
        headerTitleBlock.InsertText("Equity (Acqusition costs)");
        headerTitle.Blocks.Add(headerTitleBlock);

        var closingCosts = table.Rows.AddTableRow();
        closingCosts.BasicCell("Closing Costs", false);
        closingCosts.BasicCell(asr.ClosingCosts.ToString("C2"), false);

        var loanPoints = table.Rows.AddTableRow();
        loanPoints.BasicCell("Loan Points", false, ReportBuilder.PrimaryColor);
        loanPoints.BasicCell(asr.LoanPoints.ToString("C2"), false, ReportBuilder.PrimaryColor);

        var acquisitionFee = table.Rows.AddTableRow();
        acquisitionFee.BasicCell("Aquisition Fee", false);
        acquisitionFee.BasicCell(property.AquisitionFee.ToString("C2"), false);

        var totalClosingCosts = table.Rows.AddTableRow();
        totalClosingCosts.BasicCell("Total Closing Costs", false, ReportBuilder.PrimaryColor);
        totalClosingCosts.BasicCell((property.AquisitionFee + asr.ClosingCosts + asr.LoanPoints).ToString("C2"), false, ReportBuilder.PrimaryColor);

        var downPaymentPercentage = table.Rows.AddTableRow();
        downPaymentPercentage.BasicCell("Down Payment", false);
        downPaymentPercentage.BasicCell(asr.DownpaymentPercentage.ToString("P2"), false);

        var downPayment = table.Rows.AddTableRow();
        downPayment.BasicCell("Down Payment", false, ReportBuilder.PrimaryColor);
        downPayment.BasicCell(asr.Downpayment.ToString("C2"), false, ReportBuilder.PrimaryColor);

        var insurancePremium = table.Rows.AddTableRow();
        insurancePremium.BasicCell("Insurance Premium", false);
        insurancePremium.BasicCell(asr.InsurancePremium.ToString("C2"), false);

        var intialCapitalImprovements = table.Rows.AddTableRow();
        intialCapitalImprovements.BasicCell("Initial Capital Improvements (see breakdown below)", false, ReportBuilder.PrimaryColor);
        intialCapitalImprovements.BasicCell(asr.CapitalImprovementsBreakDown.Sum(x => x.Value).ToString("C2"), false, ReportBuilder.PrimaryColor);

        var totalEquityAcqusitionCosts = table.Rows.AddTableRow();
        totalEquityAcqusitionCosts.BasicCell("Total Acqusition Costs", true);
        totalEquityAcqusitionCosts.BasicCell(asr.TotalEquity.ToString("C2"), true);

        editor.Position.Translate(ReportBuilder.PageMargin + 5, 150);
        editor.DrawTable(table, new Size(width + 20, double.PositiveInfinity));

    }

    private static void CashFlowTable(RadFixedPage page,
                                      FixedContentEditor editor,
                                      UnderwritingAnalysis property,
                                      Border border,
                                      double width,
                                      double padding = 20,
                                      double headerSize = 18)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
        };
        table.Borders = new TableBorders(border);

        var title = table.Rows.AddTableRow();
        title.BasicCell("Cash Flow", true, ReportBuilder.HeaderColor, HorizontalAlignment.Left);
        title.BasicCell("Per Year", false, ReportBuilder.HeaderColor);

        var rentAbatement = table.Rows.AddTableRow();
        rentAbatement.BasicCell("Rent Abatement", false);
        rentAbatement.BasicCell(" - ", false); // TODO : Add Rent Abatement

        var reimbursementIncome = table.Rows.AddTableRow();
        reimbursementIncome.BasicCell("Annual Adjustment to Expense Reimbursement Income", false, ReportBuilder.PrimaryColor);
        reimbursementIncome.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Reimbursement Income


        var generalMinimumVacany = table.Rows.AddTableRow();
        generalMinimumVacany.BasicCell("General Minimum Vacancy", false);
        generalMinimumVacany.BasicCell(" - ", false); // TODO : Add General Minimum Vacancy

        var taxAdustments = table.Rows.AddTableRow();
        taxAdustments.BasicCell("Annual Tax Adustment", false, ReportBuilder.PrimaryColor);
        taxAdustments.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Tax Adjustments

        var expenseAdjustments = table.Rows.AddTableRow();
        expenseAdjustments.BasicCell("Annual Expense Adjustment", false);
        expenseAdjustments.BasicCell(" - ", false); // TODO : Add Expense Adjustments

        var capitalReserveAdustment = table.Rows.AddTableRow();
        capitalReserveAdustment.BasicCell("Annual Additional Capital Reserve Adjustment", false, ReportBuilder.PrimaryColor);
        capitalReserveAdustment.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Capital Reserve Adjustments

        editor.Position.Translate(page.Size.Width - width - ReportBuilder.PageMargin - 5, 150);
        editor.DrawTable(table, new Size(width, double.PositiveInfinity));
    }

    private static void PrimaryTable(RadFixedPage page,
                                     FixedContentEditor editor,
                                     UnderwritingAnalysis property,
                                     Border border,
                                     double width,
                                     double padding = 20,
                                     double headerSize = 18)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
        };
        table.Borders = new TableBorders(border);

        var debtAcqusitionCostsTitle = table.Rows.AddTableRow();

        var debtAcqusitionCostsTitleHeader = debtAcqusitionCostsTitle.Cells.AddTableCell();
        debtAcqusitionCostsTitleHeader.PreferredWidth = width / 2 + 20;
        debtAcqusitionCostsTitleHeader.Background = ReportBuilder.HeaderColor;
        var debtAcqusitionCostsTitleHeaderBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
        debtAcqusitionCostsTitleHeaderBlock.InsertText("Primary Debt (Aqusition)");
        debtAcqusitionCostsTitleHeader.Blocks.Add(debtAcqusitionCostsTitleHeaderBlock);
        var debtAcqusitionCostsTitleHeaderCell = debtAcqusitionCostsTitle.Cells.AddTableCell();
        debtAcqusitionCostsTitleHeaderCell.Background = ReportBuilder.HeaderColor;
        var debtAcqusitionCostsTitleHeaderCellBlock = new Block();
        debtAcqusitionCostsTitleHeaderCell.Blocks.Add(debtAcqusitionCostsTitleHeaderCellBlock);

        var typeOfLoan = table.Rows.AddTableRow();
        typeOfLoan.BasicCell("Type of Loan", false);
        typeOfLoan.BasicCell(property.LoanType.ToString(), false);

        var financedCapitalImprovements = table.Rows.AddTableRow();
        financedCapitalImprovements.BasicCell("Intial Capital Improvements (Financed)", false, ReportBuilder.PrimaryColor);
        financedCapitalImprovements.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Financed Capital Improvements

        var loanAmount = table.Rows.AddTableRow();
        loanAmount.BasicCell("Loan Amount", false);
        loanAmount.BasicCell(property.Mortgages.Sum(x => x.LoanAmount).ToString("C2"), false);

        var totalLoanAmount = table.Rows.AddTableRow();
        totalLoanAmount.BasicCell("Total Loan Amount", false, ReportBuilder.PrimaryColor);
        totalLoanAmount.BasicCell(property.Mortgages.Sum(x => x.LoanAmount).ToString("C2"), false, ReportBuilder.PrimaryColor);

        var loanToValue = table.Rows.AddTableRow();
        loanToValue.BasicCell("Loan to Value", false);
        loanToValue.BasicCell(property.LTV.ToString("P2"), false);

        var interestOnlyPeriod = table.Rows.AddTableRow();
        interestOnlyPeriod.BasicCell("Interest Only Period", false, ReportBuilder.PrimaryColor);
        interestOnlyPeriod.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Interest Only Period

        var interestRate = table.Rows.AddTableRow();
        interestRate.BasicCell("Interest Rate", false);
        interestRate.BasicCell(property.Mortgages.Sum(x => x.InterestRate).ToString("P2"), false);

        var amortization = table.Rows.AddTableRow();
        amortization.BasicCell("Amortization", false, ReportBuilder.PrimaryColor);
        amortization.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Amortization

        var term = table.Rows.AddTableRow();
        term.BasicCell("Term", false);
        term.BasicCell(property.Mortgages.Sum(x => x.TermInYears).ToString() + " Years", false);

        var debtServiceMonth = table.Rows.AddTableRow();
        debtServiceMonth.BasicCell("Debt Service / Month", false, ReportBuilder.PrimaryColor);
        debtServiceMonth.BasicCell((property.Mortgages.Sum(x => x.AnnualDebtService) / 12).ToString("C2"), false, ReportBuilder.PrimaryColor);

        var debtServiceYear = table.Rows.AddTableRow();
        debtServiceYear.BasicCell("Debt Service / Year", false);
        debtServiceYear.BasicCell(property.Mortgages.Sum(x => x.AnnualDebtService).ToString("C2"), false);

        editor.Position.Translate(ReportBuilder.PageMargin + 5, 150);
        editor.DrawTable(table, new Size(width, double.PositiveInfinity));
    }

    private static void SupplementalTable(RadFixedPage page,
                                           FixedContentEditor editor,
                                           UnderwritingAnalysis property,
                                           Border border,
                                           double width,
                                           double padding = 20,
                                           double headerSize = 18)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
        };
        table.Borders = new TableBorders(border);

        var debtAcqusitionCostsTitle = table.Rows.AddTableRow();
        var debtAcqusitionCostsTitleHeader = debtAcqusitionCostsTitle.Cells.AddTableCell();
        debtAcqusitionCostsTitleHeader.PreferredWidth = width / 2 + 20;
        debtAcqusitionCostsTitleHeader.Background = ReportBuilder.HeaderColor;
        var debtAcqusitionCostsTitleHeaderBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
        debtAcqusitionCostsTitleHeaderBlock.InsertText("Supplemental Debt (Aqusition)");
        debtAcqusitionCostsTitleHeader.Blocks.Add(debtAcqusitionCostsTitleHeaderBlock);
        var debtAcqusitionCostsTitleHeaderCell = debtAcqusitionCostsTitle.Cells.AddTableCell();
        debtAcqusitionCostsTitleHeaderCell.Background = ReportBuilder.HeaderColor;
        var debtAcqusitionCostsTitleHeaderCellBlock = new Block();
        debtAcqusitionCostsTitleHeaderCell.Blocks.Add(debtAcqusitionCostsTitleHeaderCellBlock);

        var loanCost = table.Rows.AddTableRow();
        loanCost.BasicCell("Loan Cost (Financed)", false);
        loanCost.BasicCell(" - ", false); // TODO : Add Loan Cost Supplemental

        var adddedCapitalImprovements = table.Rows.AddTableRow();
        adddedCapitalImprovements.BasicCell("Added Capital Improvements (Financed)", false, ReportBuilder.PrimaryColor);
        adddedCapitalImprovements.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Added Capital Improvements Supplemental

        var loanAmount = table.Rows.AddTableRow();
        loanAmount.BasicCell("Loan Amount", false);
        loanAmount.BasicCell(" - ", false); // TODO : Add Loan Amount Supplemental

        var totalLoanAmount = table.Rows.AddTableRow();
        totalLoanAmount.BasicCell("Total Loan Amount", false, ReportBuilder.PrimaryColor);
        totalLoanAmount.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Total Loan Amount Supplemental

        var loanToValue = table.Rows.AddTableRow();
        loanToValue.BasicCell("Loan to Value", false);
        loanToValue.BasicCell(" - ", false); // TODO : Add Loan to Value Supplemental

        var interestOnlyPeriod = table.Rows.AddTableRow();
        interestOnlyPeriod.BasicCell("Interest Only Period", false, ReportBuilder.PrimaryColor);
        interestOnlyPeriod.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Interest Only Period Supplemental

        var interestRate = table.Rows.AddTableRow();
        interestRate.BasicCell("Interest Rate", false);
        interestRate.BasicCell(" - ", false); // TODO : Add Interest Rate Supplemental

        var amortization = table.Rows.AddTableRow();
        amortization.BasicCell("Amortization", false, ReportBuilder.PrimaryColor);
        amortization.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Amortization Supplemental

        var term = table.Rows.AddTableRow();
        term.BasicCell("Term", false);
        term.BasicCell(" - ", false); // TODO : Add Term Supplemental

        var debtServiceMonth = table.Rows.AddTableRow();
        debtServiceMonth.BasicCell("Debt Service / Month", false, ReportBuilder.PrimaryColor);
        debtServiceMonth.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Debt Service / Month Supplemental

        var debtServiceYear = table.Rows.AddTableRow();
        debtServiceYear.BasicCell("Debt Service / Year", false);
        debtServiceYear.BasicCell(" - ", false); // TODO : Add Debt Service / Year Supplemental

        editor.Position.Translate(width + ReportBuilder.PageMargin + 45, 150);
        editor.DrawTable(table, new Size(width, double.PositiveInfinity));
    }

    private static void RefinanceTable(RadFixedPage page,
                                       FixedContentEditor editor,
                                       UnderwritingAnalysis property,
                                       Border border,
                                       double width,
                                       double padding = 20,
                                       double headerSize = 18)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
        };
        table.Borders = new TableBorders(border);

        var title = table.Rows.AddTableRow();
        var titleHeader = title.Cells.AddTableCell();
        titleHeader.PreferredWidth = width / 2 + 20;
        titleHeader.Background = ReportBuilder.HeaderColor;
        var titleHeaderBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
        titleHeaderBlock.InsertText("Refinance");
        titleHeader.Blocks.Add(titleHeaderBlock);
        var titleCell = title.Cells.AddTableCell();
        titleCell.Background = ReportBuilder.HeaderColor;
        var titleCellBlock = new Block();
        titleCell.Blocks.Add(titleCellBlock);

        var loanStartDate = table.Rows.AddTableRow();
        loanStartDate.BasicCell("Loan Start Date", false);
        loanStartDate.BasicCell(" - ", false); // TODO : Add Loan Start Date

        var financedImprovements = table.Rows.AddTableRow();
        financedImprovements.BasicCell("Capital Improvements (Financed)", false, ReportBuilder.PrimaryColor);
        financedImprovements.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Financed Capital Improvements Refinance

        var loanCosts = table.Rows.AddTableRow();
        loanCosts.BasicCell("Loan Cost (Financed)", false);
        loanCosts.BasicCell(" - ", false); // TODO : Add Loan Cost Refinance

        var loanAmount = table.Rows.AddTableRow();
        loanAmount.BasicCell("Loan Amount", false, ReportBuilder.PrimaryColor);
        loanAmount.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Loan Amount Refinance

        var totalLoanAmount = table.Rows.AddTableRow();
        totalLoanAmount.BasicCell("Total Loan Amount", false);
        totalLoanAmount.BasicCell(" - ", false); // TODO : Add Total Loan Amount Refinance

        var loanToValue = table.Rows.AddTableRow();
        loanToValue.BasicCell("Loan to Value", false, ReportBuilder.PrimaryColor);
        loanToValue.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Loan to Value Refinance

        var interestRate = table.Rows.AddTableRow();
        interestRate.BasicCell("Interest Rate", false);
        interestRate.BasicCell(" - ", false); // TODO : Add Interest Rate Refinance

        var amortization = table.Rows.AddTableRow();
        amortization.BasicCell("Amortization", false, ReportBuilder.PrimaryColor);
        amortization.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Amortization Refinance

        var term = table.Rows.AddTableRow();
        term.BasicCell("Term", false);
        term.BasicCell(" - ", false); // TODO : Add Term Refinance

        var debtServiceMonth = table.Rows.AddTableRow();
        debtServiceMonth.BasicCell("Debt Service / Month", false, ReportBuilder.PrimaryColor);
        debtServiceMonth.BasicCell(" - ", false, ReportBuilder.PrimaryColor); // TODO : Add Debt Service / Month Refinance

        var debtServiceYear = table.Rows.AddTableRow();
        debtServiceYear.BasicCell("Debt Service / Year", false);
        debtServiceYear.BasicCell(" - ", false); // TODO : Add Debt Service / Year Refinance

        editor.Position.Translate(page.Size.Width - width - ReportBuilder.PageMargin - 5, 150);
        editor.DrawTable(table, new Size(width, double.PositiveInfinity));
    }

    private static void CapitalImprovementsTable(RadFixedPage page,
                                                 FixedContentEditor editor,
                                                 UnderwritingAnalysis property,
                                                 Border border,
                                                 double width,
                                                 double padding = 20,
                                                 double headerSize = 18)
    {
        var asr = new AssumptionsReport(property);
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
        };
        table.Borders = new TableBorders(border);

        var title = table.Rows.AddTableRow();
        title.BasicCell("Capital Improvements Breakdown", true, ReportBuilder.HeaderColor, HorizontalAlignment.Left, 2);

        var headers = table.Rows.AddTableRow();
        headers.BasicCell("Cost", true, ReportBuilder.WhiteColor, HorizontalAlignment.Left);
        headers.BasicCell("Description", true, ReportBuilder.WhiteColor, HorizontalAlignment.Left);

        var targetindex = 0;
        foreach (var capitalImprovement in asr.CapitalImprovementsBreakDown)
        {
            if (capitalImprovement.Value == 0)
                continue;

            var row = table.Rows.AddTableRow();
            row.BasicCell(capitalImprovement.Value.ToString("C"), false, targetindex % 2 == 0 ? ReportBuilder.PrimaryColor : ReportBuilder.WhiteColor, HorizontalAlignment.Left);
            row.BasicCell(capitalImprovement.Key, false, targetindex % 2 == 0 ? ReportBuilder.PrimaryColor : ReportBuilder.WhiteColor, HorizontalAlignment.Left);
            targetindex++;
        }

        editor.Position.Translate(ReportBuilder.PageMargin + 15, 150);
        editor.DrawTable(table, new Size(page.Size.Width - 2 * ReportBuilder.PageMargin - 30, double.PositiveInfinity));
    }

}