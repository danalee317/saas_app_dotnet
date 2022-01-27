using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;
using MultiFamilyPortal.Dtos;

public static class GenerateCashFlowBuilder
{
    public static void GenerateCashFlow(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var projections = property.Projections;

        var cellPadding = property.HoldYears > 5 ? 13 : 22;
        var blackBorder = new Border(1, ReportBuilder.DarkColor);
        var page = document.Pages.AddPage();
        var pageTwo = document.Pages.AddPage();
        var editorOne = new FixedContentEditor(page);
        var editorTwo = new FixedContentEditor(pageTwo);
        page.Size = ReportBuilder.LetterSizeHorizontal;
        pageTwo.Size = page.Size;

        ReportBuilder.Header(page, "Cash Flow");
        IncomeTable(page, editorOne, projections, blackBorder);
        ExpensesTable(pageTwo, editorTwo, projections, blackBorder);
        NetTable(pageTwo, editorTwo, projections, blackBorder);
        ReportBuilder.Footer(page, property.Name);
        ReportBuilder.Footer(pageTwo, property.Name);
    }

    private static void IncomeTable(RadFixedPage page, FixedContentEditor editor, IEnumerable<UnderwritingAnalysisProjection> projections, Border border, double padding = 15, double headerSize = 18)
    {
        // Income
        var tableTitle = page.Content.AddTextFragment();
        tableTitle.FontSize = headerSize;
        tableTitle.Text = "Income";
        tableTitle.Position.Translate(page.Size.Width / 2 - 25, 135);

        var table = new Table { DefaultCellProperties = { Padding = new Thickness(padding) } };

        table.Borders = new TableBorders(border);

        var incomeHeader = table.Rows.AddTableRow();
        var actualScheduledRent = table.Rows.AddTableRow();
        var lessEffectiveVacancy = table.Rows.AddTableRow();
        var effectiveVacancy = table.Rows.AddTableRow();
        var lessOtherLosses = table.Rows.AddTableRow();
        var adjustedIncome = table.Rows.AddTableRow();
        var utilitiesIncome = table.Rows.AddTableRow();
        var addingOtherIncome = table.Rows.AddTableRow();
        var totalEffectiveIncome = table.Rows.AddTableRow();

        var accountingYearTitle = incomeHeader.Cells.AddTableCell();
        var accountingYearTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            TextProperties = { Font = FontsRepository.HelveticaBold }
        };
        accountingYearTitleBlock.InsertText("For the Years Ending");
        accountingYearTitle.Blocks.Add(accountingYearTitleBlock);
        int targetIndex = 0;
        foreach (var year in projections.Select(x => x.Year))
        {
            var dynamicCell = incomeHeader.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(137, 207, 240);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.TextProperties.Font = FontsRepository.HelveticaBold;
            var title = targetIndex == 0 ? "Stated in Place" : $"{year}";
            dynamicCellBlock.InsertText(title);
            dynamicCell.Blocks.Add(dynamicCellBlock);
            targetIndex++;
        }

        var actualScheduledRentTitle = actualScheduledRent.Cells.AddTableCell();
        var actualScheduledRentTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        actualScheduledRentTitleBlock.InsertText("Actual Scheduled Rent at 100%");
        actualScheduledRentTitle.Blocks.Add(actualScheduledRentTitleBlock);
        foreach (var rent in projections.Select(x => x.GrossScheduledRent))
        {
            var dynamicCell = actualScheduledRent.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{rent:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        var lessEffectiveVacancyTitle = lessEffectiveVacancy.Cells.AddTableCell();
        var lessEffectiveVacancyTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        lessEffectiveVacancyTitleBlock.InsertText("Less Effective Vacancy ($)");
        lessEffectiveVacancyTitle.Blocks.Add(lessEffectiveVacancyTitleBlock);
        foreach (var vancancy in projections.Select(x => x.Vacancy))
        {
            var dynamicCell = lessEffectiveVacancy.Cells.AddTableCell();
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{vancancy:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        var effectiveVacancyTitle = effectiveVacancy.Cells.AddTableCell();
        var effectiveVacancyTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        effectiveVacancyTitleBlock.InsertText("Effective Vacancy (%)");
        effectiveVacancyTitle.Blocks.Add(effectiveVacancyTitleBlock);
        targetIndex = 0;
        foreach (var vacancy in projections.Select(x => x.Vacancy))
        {
            var dynamicCell = effectiveVacancy.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            var targetVacancy = projections.Select(x => x.GrossScheduledRent).ToList()[targetIndex];
            var deno = Math.Abs(targetVacancy) > 0 ? targetVacancy : 1;
            dynamicCellBlock.InsertText($"{(100 * Math.Abs(vacancy / deno)):F2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
            targetIndex++;
        }

        var lessOtherLossesTitle = lessOtherLosses.Cells.AddTableCell();
        var lessOtherLossesTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        lessOtherLossesTitleBlock.InsertText("Less Other Losses");
        lessOtherLossesTitle.Blocks.Add(lessOtherLossesTitleBlock);
        foreach (var loss in projections.Select(x => x.ConcessionsNonPayment))
        {
            var dynamicCell = lessOtherLosses.Cells.AddTableCell();
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{loss:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        var adjustedIncomeTitle = adjustedIncome.Cells.AddTableCell();
        var adjustedIncomeTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        adjustedIncomeTitleBlock.InsertText("Adjusted Income ($)");
        adjustedIncomeTitle.Blocks.Add(adjustedIncomeTitleBlock);
        targetIndex = 0;
        foreach (var rent in projections.Select(x => x.GrossScheduledRent))
        {
            var dynamicCell = adjustedIncome.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.SaveGraphicProperties();
            dynamicCellBlock.GraphicProperties.FillColor = new RgbColor(13, 110, 253);
            dynamicCellBlock.InsertText($"{(rent - projections.Select(x => x.Vacancy).ToList()[targetIndex]):C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
            targetIndex++;
        }

        var utilitiesIncomeTitle = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomeTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        utilitiesIncomeTitleBlock.InsertText("plus Utilities Income ($)");
        utilitiesIncomeTitle.Blocks.Add(utilitiesIncomeTitleBlock);
        foreach (var income in projections.Select(x => x.UtilityReimbursement))
        {
            var dynamicCell = utilitiesIncome.Cells.AddTableCell();
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{income:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        var addingOtherIncomeTitle = addingOtherIncome.Cells.AddTableCell();
        var addingOtherIncomeTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        addingOtherIncomeTitleBlock.InsertText("plus Other Income ($)");
        addingOtherIncomeTitle.Blocks.Add(addingOtherIncomeTitleBlock);
        foreach (var additionalIncome in projections.Select(x => x.OtherIncome))
        {
            var dynamicCell = addingOtherIncome.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{additionalIncome:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        var totalEffectiveIncomeTitle = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomeTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        totalEffectiveIncomeTitleBlock.TextProperties.Font = FontsRepository.HelveticaBold;
        totalEffectiveIncomeTitleBlock.InsertText("Total Effective Income");
        totalEffectiveIncomeTitle.Blocks.Add(totalEffectiveIncomeTitleBlock);
        foreach (var total in projections.Select(x => x.EffectiveGrossIncome))
        {
            var dynamicCell = totalEffectiveIncome.Cells.AddTableCell();
            var dynamicCellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                TextProperties = { Font = FontsRepository.HelveticaBold }
            };
            dynamicCellBlock.InsertText($"{total:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        editor.Position.Translate(page.Size.Width / 2 - table.Measure().Width / 2, 150);
        editor.DrawTable(table, new Size(page.Size.Width - 2 * ReportBuilder.PageMargin, table.Measure().Height));

    }

    private static void ExpensesTable(RadFixedPage page, FixedContentEditor editor, IEnumerable<UnderwritingAnalysisProjection> projections, Border border, double padding = 20, double headerSize = 18)
    {
        var tableTitle = page.Content.AddTextFragment();
        tableTitle.FontSize = headerSize;
        tableTitle.Text = "Expenses";
        tableTitle.Position.Translate(page.Size.Width / 2 - 30, 135);

        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            Borders = new TableBorders(border),
            LayoutType = TableLayoutType.FixedWidth
        };

        var expensesHeader = table.Rows.AddTableRow();
        var totalOperatingExpenses = table.Rows.AddTableRow();

        var expensesHeaderTitle = expensesHeader.Cells.AddTableCell();
        var expensesHeaderTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        expensesHeaderTitleBlock.TextProperties.Font = FontsRepository.HelveticaBold;
        expensesHeaderTitleBlock.InsertText("For The Years Ending");
        expensesHeaderTitle.Blocks.Add(expensesHeaderTitleBlock);

        var targetIndex = 0;
        foreach (var year in projections.Select(x => x.Year))
        {
            var dynamicCell = expensesHeader.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(137, 207, 240);
            var dynamicCellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                TextProperties = { Font = FontsRepository.HelveticaBold }
            };
            var title = targetIndex == 0 ? "Stated in Place" : $"{year}";
            dynamicCellBlock.InsertText(title);
            dynamicCell.Blocks.Add(dynamicCellBlock);
            targetIndex++;
        }

        var totalOperatingExpensesTitle = totalOperatingExpenses.Cells.AddTableCell();
        var totalOperatingExpensesTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        totalOperatingExpensesTitleBlock.InsertText("Total Operating Expenses");
        totalOperatingExpensesTitle.Blocks.Add(totalOperatingExpensesTitleBlock);
        foreach (var expenses in projections.Select(x => x.OperatingExpenses))
        {
            var dynamicCell = totalOperatingExpenses.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{expenses:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        editor.Position.Translate(page.Size.Width / 2 - table.Measure().Width / 2, 150);
        editor.DrawTable(table);
    }

    private static void NetTable(RadFixedPage page, FixedContentEditor editor, IEnumerable<UnderwritingAnalysisProjection> projections, Border border, double padding = 20, double headerSize = 18)
    {
        // Net Income
        var tableTitle = page.Content.AddTextFragment();
        tableTitle.FontSize = headerSize;
        tableTitle.Text = "Net";
        tableTitle.Position.Translate(page.Size.Width / 2 - 5, 335);

        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            Borders = new TableBorders(border),
            LayoutType = TableLayoutType.FixedWidth
        };

        var netHeader = table.Rows.AddTableRow();
        var netOperatingIncome = table.Rows.AddTableRow();
        var capitalReserves = table.Rows.AddTableRow();
        var cashBeforeDebtService = table.Rows.AddTableRow();
        var annualDebtService = table.Rows.AddTableRow();
        var cashFlowBeforeTax = table.Rows.AddTableRow();
        
        ReportBuilder.BasicCell(netHeader, "For The Years Ending", ReportBuilder.WhiteColor, true);
        var targetIndex = 0;
        foreach (var year in projections.Select(x => x.Year))
        {
              var title = targetIndex == 0 ? "Stated in Place" : $"{year}";
            ReportBuilder.BasicCell(netHeader, title, ReportBuilder.HeaderColor, true);
            targetIndex++;
        }

        ReportBuilder.BasicCell(netOperatingIncome, "Net Operating Income (NOI)", ReportBuilder.WhiteColor);
        foreach (var noi in projections.Select(x => x.NetOperatingIncome))
            ReportBuilder.BasicCell(netOperatingIncome, noi.ToString("C2"),ReportBuilder.PrimaryColor);
        
        ReportBuilder.BasicCell(capitalReserves, "Less Capital Reserves", ReportBuilder.WhiteColor);
        foreach (var capitalReserve in projections.Select(x => x.CapitalReserves))
            ReportBuilder.BasicCell(capitalReserves, capitalReserve.ToString("C2"),ReportBuilder.WhiteColor);
          
        ReportBuilder.BasicCell(cashBeforeDebtService, "Cash Before Debt Service",ReportBuilder.WhiteColor);
        foreach (var cash in projections.Select(x => x.CashFlowBeforeDebtService))
            ReportBuilder.BasicCell(cashBeforeDebtService, cash.ToString("C2"), ReportBuilder.PrimaryColor);
        
        ReportBuilder.BasicCell(annualDebtService, "Less Annual Debt Service",ReportBuilder.WhiteColor);
        foreach (var debt in projections.Select(x => x.DebtService))
            ReportBuilder.BasicCell(annualDebtService, debt.ToString("C2"), ReportBuilder.WhiteColor);
        
        ReportBuilder.BasicCell(cashFlowBeforeTax, "Cash Flow Before Taxes", ReportBuilder.WhiteColor);
        foreach (var cash in projections.Select(x => x.TotalCashFlow))
            ReportBuilder.BasicCell(cashFlowBeforeTax, cash.ToString("C2"), ReportBuilder.PrimaryColor, true);
        
        editor.Position.Translate(page.Size.Width / 2 - table.Measure().Width / 2, 350);
        editor.DrawTable(table, new Size(table.Measure().Width, double.PositiveInfinity));
    }
}