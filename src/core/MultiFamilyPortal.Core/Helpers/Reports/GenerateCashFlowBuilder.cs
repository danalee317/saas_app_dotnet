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

        var cellPadding = 5.3;
        var blackBorder = new Border(1, ReportBuilder.DarkColor);
        var page = document.Pages.AddPage();
        page.Size = ReportBuilder.LetterSizeHorizontal;

        var editor = new FixedContentEditor(page);

        ReportBuilder.Header(page, "Cash Flow");
        IncomeTable(page, editor, projections, blackBorder, cellPadding);
        ExpensesTable(page, editor, projections, blackBorder, cellPadding);
        NetTable(page, editor, projections, blackBorder, cellPadding);
        ReportBuilder.Footer(page, property.Name);

        if (property.HoldYears > 5)
        {
            var pageTwo = document.Pages.AddPage();
            var editorTwo = new FixedContentEditor(pageTwo);
            pageTwo.Size = page.Size;

            ReportBuilder.Header(pageTwo, "Cash Flow (" + (property.HoldYears == 6 ? "6" : "6 - " + property.HoldYears.ToString()) + " Years)");
            IncomeTable(pageTwo, editorTwo, projections, blackBorder, cellPadding, true);
            ExpensesTable(pageTwo, editorTwo, projections, blackBorder, cellPadding + 5, true);
            NetTable(pageTwo, editorTwo, projections, blackBorder, cellPadding, true);
            ReportBuilder.Footer(pageTwo, property.Name);
        }
    }

    private static void IncomeTable(RadFixedPage page,
                                    FixedContentEditor editor,
                                    IEnumerable<UnderwritingAnalysisProjection> projections,
                                    Border border,
                                    double padding = 15,
                                    bool isSecondary = false,
                                    double headerSize = 18)
    {
        var tableTitle = page.Content.AddTextFragment();
        tableTitle.FontSize = headerSize;
        tableTitle.Text = "Income";
        tableTitle.Position.Translate(page.Size.Width / 2 - 25, 120);
       
        var startPoint = isSecondary ? 6 : 1;
        var breakPoint = 4;
        var holdYears = projections.Count() - 1;
        var missingYears = isSecondary ? 10 - holdYears : 5 - holdYears;

        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth
        };
        table.Borders = new TableBorders(border);

        var incomeHeader = table.Rows.AddTableRow();
        var years = projections.Select(x => x.Year);
        incomeHeader.BasicCell("For the Years Ending", true);
        incomeHeader.BasicCell("Stated in Place", true, ReportBuilder.HeaderColor);
        var targetIndex = 0;
        foreach (var year in years.Skip(startPoint))
        {
            incomeHeader.BasicCell(year.ToString(), true, ReportBuilder.HeaderColor);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                incomeHeader.BasicCell((years.LastOrDefault() + i).ToString(), true, ReportBuilder.HeaderColor);
        }

        var actualScheduledRent = table.Rows.AddTableRow();
        var rents = projections.Select(x => x.GrossScheduledRent);
        actualScheduledRent.BasicCell("Actual Scheduled Rent at 100%");
        actualScheduledRent.BasicCell(rents.FirstOrDefault().ToString("C2"), false, ReportBuilder.PrimaryColor);
        targetIndex = 0;
        foreach (var rent in rents.Skip(startPoint))
        {
            actualScheduledRent.BasicCell(rent.ToString("C2"), false, ReportBuilder.PrimaryColor);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                actualScheduledRent.BasicCell("", false, ReportBuilder.PrimaryColor);
        }

        var lessEffectiveVacancy = table.Rows.AddTableRow();
        var vacancies = projections.Select(x => x.Vacancy);
        lessEffectiveVacancy.BasicCell("Less Effective Vacancy ($)");
        lessEffectiveVacancy.BasicCell(vacancies.FirstOrDefault().ToString("C2"));
        targetIndex = 0;
        foreach (var vancancy in vacancies.Skip(startPoint))
        {
            lessEffectiveVacancy.BasicCell(vancancy.ToString("C2"));
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                lessEffectiveVacancy.BasicCell("");
        }

        var effectiveVacancy = table.Rows.AddTableRow();
        effectiveVacancy.BasicCell("Effective Vacancy (%)");
        var firstVacancy = vacancies.FirstOrDefault();
        var firstRent = rents.FirstOrDefault();
        var denom = Math.Abs(firstRent) > 0 ? firstRent : 1;
        effectiveVacancy.BasicCell($"{(100 * Math.Abs(firstVacancy / denom)):F2}", false, ReportBuilder.PrimaryColor);
        targetIndex = 0;
        foreach (var vacancy in vacancies.Skip(startPoint))
        {
            var targetVacancy = projections.Select(x => x.GrossScheduledRent).ToList()[targetIndex];
            var deno = Math.Abs(targetVacancy) > 0 ? targetVacancy : 1;
            effectiveVacancy.BasicCell($"{(100 * Math.Abs(vacancy / deno)):F2}", false, ReportBuilder.PrimaryColor);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                effectiveVacancy.BasicCell("", false, ReportBuilder.PrimaryColor);
        }

        var lessOtherLosses = table.Rows.AddTableRow();
        var otherLosses = projections.Select(x => x.ConcessionsNonPayment);
        lessOtherLosses.BasicCell("Less Other Losses");
        lessOtherLosses.BasicCell(otherLosses.FirstOrDefault().ToString("C2"));
        targetIndex = 0;
        foreach (var loss in otherLosses.Skip(startPoint))
        {
            lessOtherLosses.BasicCell(loss.ToString("C2"));
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                lessOtherLosses.BasicCell("");
        }

        var adjustedIncome = table.Rows.AddTableRow();
        var adjustedIncomes = projections.Select(x => x.GrossScheduledRent);
        adjustedIncome.BasicCell("Adjusted Income ($)");
        var tempCell = adjustedIncome.Cells.AddTableCell();
        tempCell.Background = ReportBuilder.PrimaryColor;
        var tempCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        tempCellBlock.SaveGraphicProperties();
        tempCellBlock.GraphicProperties.FillColor = new RgbColor(13, 110, 253);
        tempCellBlock.InsertText($"{(firstRent - firstVacancy):C2}");
        tempCell.Blocks.Add(tempCellBlock);

        targetIndex = 0;
        foreach (var rent in adjustedIncomes.Skip(startPoint))
        {
            var dynamicCell = adjustedIncome.Cells.AddTableCell();
            dynamicCell.Background = ReportBuilder.PrimaryColor;
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.SaveGraphicProperties();
            dynamicCellBlock.GraphicProperties.FillColor = new RgbColor(13, 110, 253);
            dynamicCellBlock.InsertText($"{(rent - projections.Select(x => x.Vacancy).ToList()[targetIndex]):C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
            {
                adjustedIncome.BasicCell("", false, ReportBuilder.PrimaryColor);
            }
        }

        var utilitiesIncome = table.Rows.AddTableRow();
        var utilitiesIncomes = projections.Select(x => x.UtilityReimbursement);
        utilitiesIncome.BasicCell("plus Utilities Income ($)");
        utilitiesIncome.BasicCell(utilitiesIncomes.FirstOrDefault().ToString("C2"));
        targetIndex = 0;
        foreach (var income in utilitiesIncomes.Skip(startPoint))
        {
            utilitiesIncome.BasicCell(income.ToString("C2"));
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                utilitiesIncome.BasicCell("");
        }

        var addingOtherIncome = table.Rows.AddTableRow();
        var additionalIncomes = projections.Select(x => x.OtherIncome);
        addingOtherIncome.BasicCell("plus Other Income ($)");
        addingOtherIncome.BasicCell(additionalIncomes.FirstOrDefault().ToString("C2"), false, ReportBuilder.PrimaryColor);
        targetIndex = 0;
        foreach (var additionalIncome in additionalIncomes.Skip(startPoint))
        {
            addingOtherIncome.BasicCell(additionalIncome.ToString("C2"), false, ReportBuilder.PrimaryColor);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                addingOtherIncome.BasicCell("", false, ReportBuilder.PrimaryColor);
        }

        var totalEffectiveIncome = table.Rows.AddTableRow();
        var totalEffectiveIncomes = projections.Select(x => x.EffectiveGrossIncome);
        totalEffectiveIncome.BasicCell("Total Effective Income", true);
        totalEffectiveIncome.BasicCell(totalEffectiveIncomes.FirstOrDefault().ToString("C2"), true);
        targetIndex = 0;
        foreach (var total in totalEffectiveIncomes.Skip(startPoint))
        {
            totalEffectiveIncome.BasicCell(total.ToString("C2"), true);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                totalEffectiveIncome.BasicCell("", true);
        }

        editor.Position.Translate(ReportBuilder.PageMargin, 125);
        editor.DrawTable(table, new Size(page.Size.Width - 2 * ReportBuilder.PageMargin, double.PositiveInfinity));
    }

    private static void ExpensesTable(RadFixedPage page,
                                      FixedContentEditor editor,
                                      IEnumerable<UnderwritingAnalysisProjection> projections,
                                      Border border,
                                      double padding = 20,
                                      bool isSecondary = false,
                                      double headerSize = 18)
    {
        var tableTitle = page.Content.AddTextFragment();
        tableTitle.FontSize = headerSize;
        tableTitle.Text = "Expenses";
        tableTitle.Position.Translate(page.Size.Width / 2 - 30, 395);
        
        var startPoint = isSecondary ? 6 : 1;
        var breakPoint = 4;
        var targetIndex = 0;
        var holdYears = projections.Count() - 1;
        var missingYears = isSecondary ? 10 - holdYears : 5 - holdYears;

        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            Borders = new TableBorders(border),
            LayoutType = TableLayoutType.FixedWidth
        };

        var expensesHeader = table.Rows.AddTableRow();
        var years = projections.Select(x => x.Year).ToList();
        expensesHeader.BasicCell("For The Years Ending", true);
        expensesHeader.BasicCell("Stated in Place", true, ReportBuilder.HeaderColor);
        foreach (var year in projections.Skip(startPoint).Select(x => x.Year))
        {
            expensesHeader.BasicCell(year.ToString(), true, ReportBuilder.HeaderColor);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                expensesHeader.BasicCell((years.LastOrDefault() + i).ToString(), true, ReportBuilder.HeaderColor);
        }

        var totalOperatingExpenses = table.Rows.AddTableRow();
        var totalOperatingExpenseses = projections.Select(x => x.OperatingExpenses);
        totalOperatingExpenses.BasicCell("Total Operating Expenses");
        totalOperatingExpenses.BasicCell(totalOperatingExpenseses.FirstOrDefault().ToString("C2"));
        targetIndex = 0;
        foreach (var expenses in totalOperatingExpenseses.Skip(startPoint))
        {
            totalOperatingExpenses.BasicCell(expenses.ToString("C2"));
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                totalOperatingExpenses.BasicCell("");
        }

        editor.Position.Translate(ReportBuilder.PageMargin, 400);
        editor.DrawTable(table, new Size(page.Size.Width - 2 * ReportBuilder.PageMargin, table.Measure().Height));
    }

    private static void NetTable(RadFixedPage page,
                                 FixedContentEditor editor,
                                 IEnumerable<UnderwritingAnalysisProjection> projections,
                                 Border border,
                                 double padding = 20,
                                 bool isSecondary = false,
                                 double headerSize = 18)
    {
        var tableTitle = page.Content.AddTextFragment();
        tableTitle.FontSize = headerSize;
        tableTitle.Text = "Net";
        tableTitle.Position.Translate(page.Size.Width / 2 - 5, 510);

        var thickness = projections.Count() - 1 > 9 ? new Thickness(0, padding + 5, 0, padding + 5) : new Thickness(padding);
        var targetIndex = 0;
        var startPoint = isSecondary ? 6 : 1;
        var breakPoint = 4;
        var holdYears = projections.Count() - 1;
        var missingYears = isSecondary ? 10 - holdYears : 5 - holdYears;
        var table = new Table
        {
            DefaultCellProperties = { Padding = thickness },
            Borders = new TableBorders(border),
            LayoutType = TableLayoutType.FixedWidth
        };


        var netHeader = table.Rows.AddTableRow();
        netHeader.BasicCell("For The Years Ending", true);
        netHeader.BasicCell("Stated in Place", true, ReportBuilder.HeaderColor);

        foreach (var year in projections.Skip(startPoint).Select(x => x.Year))
        {
            netHeader.BasicCell(year.ToString(), true, ReportBuilder.HeaderColor);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                netHeader.BasicCell((projections.LastOrDefault().Year + i).ToString(), true, ReportBuilder.HeaderColor);
        }

        var netOperatingIncome = table.Rows.AddTableRow();
        var netOperatingIncomes = projections.Select(x => x.NetOperatingIncome);
        netOperatingIncome.BasicCell("Net Operating Income");
        netOperatingIncome.BasicCell(netOperatingIncomes.FirstOrDefault().ToString("C2"), false, ReportBuilder.PrimaryColor);
        targetIndex = 0;
        foreach (var noi in netOperatingIncomes.Skip(startPoint))
        {
            netOperatingIncome.BasicCell(noi.ToString("C2"), false, ReportBuilder.PrimaryColor);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                netOperatingIncome.BasicCell("", false, ReportBuilder.PrimaryColor);
        }

        var capitalReserves = table.Rows.AddTableRow();
        var capitalReservouir = projections.Select(x => x.CapitalReserves);
        capitalReserves.BasicCell("Less Capital Reserves");
        capitalReserves.BasicCell(capitalReservouir.FirstOrDefault().ToString("C2"));
        targetIndex = 0;
        foreach (var capitalReserve in capitalReservouir.Skip(startPoint))
        {
            capitalReserves.BasicCell(capitalReserve.ToString("C2"));
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                capitalReserves.BasicCell("");
        }

        var cashBeforeDebtService = table.Rows.AddTableRow();
        var cashBeforeDebtServices = projections.Select(x => x.CashFlowBeforeDebtService);
        cashBeforeDebtService.BasicCell("Cash Before Debt Service");
        cashBeforeDebtService.BasicCell(cashBeforeDebtServices.FirstOrDefault().ToString("C2"), false, ReportBuilder.PrimaryColor);
        targetIndex = 0;
        foreach (var cash in cashBeforeDebtServices.Skip(startPoint))
        {
            cashBeforeDebtService.BasicCell(cash.ToString("C2"), false, ReportBuilder.PrimaryColor);
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                cashBeforeDebtService.BasicCell("", false, ReportBuilder.PrimaryColor);
        }

        var annualDebtService = table.Rows.AddTableRow();
        var annualDebtServices = projections.Select(x => x.DebtService);
        annualDebtService.BasicCell("Less Annual Debt Service");
        annualDebtService.BasicCell(annualDebtServices.FirstOrDefault().ToString("C2"));
        targetIndex = 0;
        foreach (var debt in annualDebtServices.Skip(startPoint))
        {
            annualDebtService.BasicCell(debt.ToString("C2"));
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                annualDebtService.BasicCell("");
        }

        var cashFlowBeforeTax = table.Rows.AddTableRow();
        var totalCashFlows = projections.Select(x => x.TotalCashFlow);
        cashFlowBeforeTax.BasicCell("Cash Flow Before Taxes");
        cashFlowBeforeTax.BasicCell(totalCashFlows.FirstOrDefault().ToString("C2"));
        targetIndex = 0;
        foreach (var cash in totalCashFlows.Skip(startPoint))
        {
            cashFlowBeforeTax.BasicCell(cash.ToString("C2"));
            if (!isSecondary && targetIndex == breakPoint)
                break;
            targetIndex++;
        }

        if (missingYears > 0)
        {
            for (var i = 1; i <= missingYears; i++)
                cashFlowBeforeTax.BasicCell("");
        }

        editor.Position.Translate(ReportBuilder.PageMargin, 515);
        editor.DrawTable(table, new Size(page.Size.Width - 2 * ReportBuilder.PageMargin, double.PositiveInfinity));
    }
}