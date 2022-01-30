using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using MultiFamilyPortal.Dtos;

public static class GenerateCashFlowBuilder
{
    public static void GenerateCashFlow(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var projections = property.Projections;

        var cellPadding = 10;//property.HoldYears > 5 ? 14 : 20;
        var blackBorder = new Border(1, ReportBuilder.DarkColor);
        var page = document.Pages.AddPage();
        page.Size = ReportBuilder.LetterSizeHorizontal;
   
        var editor = new FixedContentEditor(page);

        ReportBuilder.Header(page, "Cash Flow");
        IncomeTable(page, editor, projections, blackBorder, cellPadding-5);
        ExpensesTable(page, editor, projections, blackBorder, cellPadding);
        NetTable(page, editor, projections, blackBorder, cellPadding);
        ReportBuilder.Footer(page, property.Name);

/*
        var pageTwo = document.Pages.AddPage();
        var editorTwo = new FixedContentEditor(pageTwo);
       
        pageTwo.Size = page.Size;
        ReportBuilder.Footer(pageTwo, property.Name);
        */
       
    }

    private static void IncomeTable(RadFixedPage page,
                                    FixedContentEditor editor,
                                    IEnumerable<UnderwritingAnalysisProjection> projections,
                                    Border border,
                                    double padding = 15,
                                    double headerSize = 18)
    {
        var tableTitle = page.Content.AddTextFragment();
        tableTitle.FontSize = headerSize;
        tableTitle.Text = "Income";
        tableTitle.Position.Translate(page.Size.Width / 2 - 25, 120);

        var table = new Table 
        { 
            DefaultCellProperties = { Padding = new Thickness(padding) }, 
            LayoutType = TableLayoutType.FixedWidth 
        };
        table.Borders = new TableBorders(border);

        var incomeHeader = table.Rows.AddTableRow();
        incomeHeader.BasicCell("For the Years Ending", true);
        int targetIndex = 0;
        foreach (var year in projections.Select(x => x.Year))
        {
            var title = targetIndex == 0 ? "Stated in Place" : $"{year}";
            incomeHeader.BasicCell(title, true, ReportBuilder.HeaderColor);
            targetIndex++;
        }

        var actualScheduledRent = table.Rows.AddTableRow();
        actualScheduledRent.BasicCell("Actual Scheduled Rent at 100%");
        foreach (var rent in projections.Select(x => x.GrossScheduledRent))
            actualScheduledRent.BasicCell(rent.ToString("C2"), false, ReportBuilder.PrimaryColor);

        var lessEffectiveVacancy = table.Rows.AddTableRow();
        lessEffectiveVacancy.BasicCell("Less Effective Vacancy ($)");
        foreach (var vancancy in projections.Select(x => x.Vacancy))
            lessEffectiveVacancy.BasicCell(vancancy.ToString("C2"));

        var effectiveVacancy = table.Rows.AddTableRow();
        effectiveVacancy.BasicCell("Effective Vacancy (%)");
        targetIndex = 0;
        foreach (var vacancy in projections.Select(x => x.Vacancy))
        {
            var targetVacancy = projections.Select(x => x.GrossScheduledRent).ToList()[targetIndex];
            var deno = Math.Abs(targetVacancy) > 0 ? targetVacancy : 1;
            effectiveVacancy.BasicCell($"{(100 * Math.Abs(vacancy / deno)):F2}", false, ReportBuilder.PrimaryColor);
            targetIndex++;
        }

        var lessOtherLosses = table.Rows.AddTableRow();
        lessOtherLosses.BasicCell("Less Other Losses");
        foreach (var loss in projections.Select(x => x.ConcessionsNonPayment))
            lessOtherLosses.BasicCell(loss.ToString("C2"));

        var adjustedIncome = table.Rows.AddTableRow();
        adjustedIncome.BasicCell("Adjusted Income ($)");
        targetIndex = 0;
        foreach (var rent in projections.Select(x => x.GrossScheduledRent))
        {
            var dynamicCell = adjustedIncome.Cells.AddTableCell();
            dynamicCell.Background = ReportBuilder.PrimaryColor;
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.SaveGraphicProperties();
            dynamicCellBlock.GraphicProperties.FillColor = new RgbColor(13, 110, 253);
            dynamicCellBlock.InsertText($"{(rent - projections.Select(x => x.Vacancy).ToList()[targetIndex]):C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
            targetIndex++;
        }

        var utilitiesIncome = table.Rows.AddTableRow();
        utilitiesIncome.BasicCell("plus Utilities Income ($)");
        foreach (var income in projections.Select(x => x.UtilityReimbursement))
            utilitiesIncome.BasicCell(income.ToString("C2"));

        var addingOtherIncome = table.Rows.AddTableRow();
        addingOtherIncome.BasicCell("plus Other Income ($)");
        foreach (var additionalIncome in projections.Select(x => x.OtherIncome))
            addingOtherIncome.BasicCell(additionalIncome.ToString("C2"), false, ReportBuilder.PrimaryColor);

        var totalEffectiveIncome = table.Rows.AddTableRow();
        totalEffectiveIncome.BasicCell("Total Effective Income", true);
        foreach (var total in projections.Select(x => x.EffectiveGrossIncome))
            totalEffectiveIncome.BasicCell(total.ToString("C2"), true);

        editor.Position.Translate(ReportBuilder.PageMargin, 125);
        editor.DrawTable(table, new Size(page.Size.Width - 2 * ReportBuilder.PageMargin, table.Measure().Height));

    }

    private static void ExpensesTable(RadFixedPage page,
                                      FixedContentEditor editor,
                                      IEnumerable<UnderwritingAnalysisProjection> projections,
                                      Border border,
                                      double padding = 20,
                                      double headerSize = 18)
    {
        var tableTitle = page.Content.AddTextFragment();
        tableTitle.FontSize = headerSize;
        tableTitle.Text = "Expenses";
        tableTitle.Position.Translate(page.Size.Width / 2 - 30, 395);

        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            Borders = new TableBorders(border),
            LayoutType = TableLayoutType.FixedWidth
        };

        var expensesHeader = table.Rows.AddTableRow();
        expensesHeader.BasicCell("For The Years Ending", true);
        var targetIndex = 0;
        foreach (var year in projections.Select(x => x.Year))
        {
            var title = targetIndex == 0 ? "Stated in Place" : $"{year}";
            expensesHeader.BasicCell(title, true, ReportBuilder.HeaderColor);
            targetIndex++;
        }

        var totalOperatingExpenses = table.Rows.AddTableRow();
        totalOperatingExpenses.BasicCell("Total Operating Expenses");
        foreach (var expenses in projections.Select(x => x.OperatingExpenses))
            totalOperatingExpenses.BasicCell(expenses.ToString("C2"), false, ReportBuilder.PrimaryColor);

        editor.Position.Translate(ReportBuilder.PageMargin, 400);
        editor.DrawTable(table, new Size(page.Size.Width - 2 * ReportBuilder.PageMargin, table.Measure().Height));
    }

    private static void NetTable(RadFixedPage page,
                                 FixedContentEditor editor,
                                 IEnumerable<UnderwritingAnalysisProjection> projections,
                                 Border border,
                                 double padding = 20,
                                 double headerSize = 18)
    {
        var tableTitle = page.Content.AddTextFragment();
        tableTitle.FontSize = headerSize;
        tableTitle.Text = "Net";
        tableTitle.Position.Translate(page.Size.Width / 2 - 5, 510);
        var thickness = projections.Count() - 1 > 9 ? new Thickness(0, padding+5, 0, padding+5) : new Thickness(padding);

        var table = new Table
        {
            DefaultCellProperties = { Padding = thickness },
            Borders = new TableBorders(border),
            LayoutType = TableLayoutType.FixedWidth
        };

        var netHeader = table.Rows.AddTableRow();
        netHeader.BasicCell("For The Years Ending", true);
        var targetIndex = 0;
        foreach (var year in projections.Select(x => x.Year))
        {
            var title = targetIndex == 0 ? "Stated in Place" : $"{year}";
            netHeader.BasicCell(title, true, ReportBuilder.HeaderColor);
            targetIndex++;
        }

        var netOperatingIncome = table.Rows.AddTableRow();
        netOperatingIncome.BasicCell("Net Operating Income");
        foreach (var noi in projections.Select(x => x.NetOperatingIncome))
            netOperatingIncome.BasicCell(noi.ToString("C2"), false, ReportBuilder.PrimaryColor);

        var capitalReserves = table.Rows.AddTableRow();
        capitalReserves.BasicCell("Less Capital Reserves");
        foreach (var capitalReserve in projections.Select(x => x.CapitalReserves))
            capitalReserves.BasicCell(capitalReserve.ToString("C2"));

        var cashBeforeDebtService = table.Rows.AddTableRow();
        cashBeforeDebtService.BasicCell("Cash Before Debt Service");
        foreach (var cash in projections.Select(x => x.CashFlowBeforeDebtService))
            cashBeforeDebtService.BasicCell(cash.ToString("C2"), false, ReportBuilder.PrimaryColor);

        var annualDebtService = table.Rows.AddTableRow();
        annualDebtService.BasicCell("Less Annual Debt Service");
        foreach (var debt in projections.Select(x => x.DebtService))
            annualDebtService.BasicCell(debt.ToString("C2"));

        var cashFlowBeforeTax = table.Rows.AddTableRow();
        cashFlowBeforeTax.BasicCell("Cash Flow Before Taxes");
        foreach (var cash in projections.Select(x => x.TotalCashFlow))
            cashFlowBeforeTax.BasicCell(cash.ToString("C2"), true, ReportBuilder.PrimaryColor);

        editor.Position.Translate(ReportBuilder.PageMargin, 515);
        editor.DrawTable(table, new Size(page.Size.Width-2*ReportBuilder.PageMargin, double.PositiveInfinity));
    }
}