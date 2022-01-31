using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;

public static class GenerateCashFlowBuilder
{
    public static void GenerateCashFlow(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var cfr = property.Projections;

        var dynamicWidth = 600 + 120 * property.HoldYears;
        var pageSize = new Size(dynamicWidth, 1423);
        var headerSize = 18;
        var cellPadding = 22;
        var pageOne = document.Pages.AddPage();
        var editor = new FixedContentEditor(pageOne);

        pageOne.Size = pageSize;
        var textFragment = pageOne.Content.AddTextFragment();
        textFragment.Text = "Cash Flow";
        textFragment.Position.Translate(pageOne.Size.Width / 2 - 50, 50);
        textFragment.FontSize = headerSize + 10;

        // Income
        var incomeTableTitle = pageOne.Content.AddTextFragment();
        incomeTableTitle.FontSize = headerSize;
        incomeTableTitle.Text = "Income";
        incomeTableTitle.Position.Translate(pageOne.Size.Width / 2 - 20, 110);

        var incomeTable = new Table { DefaultCellProperties = { Padding = new Thickness(cellPadding) } };
        var blackBorder = new Border(1, new RgbColor(0, 0, 0));
        incomeTable.Borders = new TableBorders(blackBorder);

        var incomeHeader = incomeTable.Rows.AddTableRow();
        var actualScheduledRent = incomeTable.Rows.AddTableRow();
        var lessEffectiveVacancy = incomeTable.Rows.AddTableRow();
        var effectiveVacancy = incomeTable.Rows.AddTableRow();
        var lessOtherLosses = incomeTable.Rows.AddTableRow();
        var adjustedIncome = incomeTable.Rows.AddTableRow();
        var utilitiesIncome = incomeTable.Rows.AddTableRow();
        var addingOtherIncome = incomeTable.Rows.AddTableRow();
        var totalEffectiveIncome = incomeTable.Rows.AddTableRow();

        var accountingYearTitle = incomeHeader.Cells.AddTableCell();
        var accountingYearTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right,
            TextProperties = { Font = FontsRepository.HelveticaBold }
        };
        accountingYearTitleBlock.InsertText("For the Years Ending");
        accountingYearTitle.Blocks.Add(accountingYearTitleBlock);
        int targetIndex = 0;
        foreach (var year in cfr.Select(x => x.Year))
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
        foreach (var rent in cfr.Select(x => x.GrossScheduledRent))
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
        foreach (var vancancy in cfr.Select(x => x.Vacancy))
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
        foreach (var vacancy in cfr.Select(x => x.Vacancy))
        {
            var dynamicCell = effectiveVacancy.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            var targetVacancy = cfr.Select(x => x.GrossScheduledRent).ToList()[targetIndex];
            var deno = Math.Abs(targetVacancy) > 0 ? targetVacancy : 1;
            dynamicCellBlock.InsertText($"{(100 * Math.Abs(vacancy / deno)):F2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
            targetIndex++;
        }

        var lessOtherLossesTitle = lessOtherLosses.Cells.AddTableCell();
        var lessOtherLossesTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        lessOtherLossesTitleBlock.InsertText("Less Other Losses");
        lessOtherLossesTitle.Blocks.Add(lessOtherLossesTitleBlock);
        foreach (var loss in cfr.Select(x => x.ConcessionsNonPayment))
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
        foreach (var rent in cfr.Select(x => x.GrossScheduledRent))
        {
            var dynamicCell = adjustedIncome.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.SaveGraphicProperties();
            dynamicCellBlock.GraphicProperties.FillColor = new RgbColor(13, 110, 253);
            dynamicCellBlock.InsertText($"{(rent - cfr.Select(x => x.Vacancy).ToList()[targetIndex]):C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
            targetIndex++;
        }

        var utilitiesIncomeTitle = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomeTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        utilitiesIncomeTitleBlock.InsertText("plus Utilities Income ($)");
        utilitiesIncomeTitle.Blocks.Add(utilitiesIncomeTitleBlock);
        foreach (var income in cfr.Select(x => x.UtilityReimbursement))
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
        foreach (var additionalIncome in cfr.Select(x => x.OtherIncome))
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
        foreach (var total in cfr.Select(x => x.EffectiveGrossIncome))
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

        editor.Position.Translate(pageOne.Size.Width / 2 - incomeTable.Measure().Width / 2, 120);
        editor.DrawTable(incomeTable);

        // Expenses
        var expensesTableTitle = pageOne.Content.AddTextFragment();
        expensesTableTitle.FontSize = headerSize;
        expensesTableTitle.Text = "Expenses";
        expensesTableTitle.Position.Translate(pageOne.Size.Width / 2 - 25, incomeTable.Measure().Height + 200);

        var expensesTable = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(cellPadding) },
            Borders = new TableBorders(blackBorder),
            LayoutType = TableLayoutType.FixedWidth
        };

        var expensesHeader = expensesTable.Rows.AddTableRow();
        var totalOperatingExpenses = expensesTable.Rows.AddTableRow();

        var expensesHeaderTitle = expensesHeader.Cells.AddTableCell();
        var expensesHeaderTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        expensesHeaderTitleBlock.TextProperties.Font = FontsRepository.HelveticaBold;
        expensesHeaderTitleBlock.InsertText("For The Years Ending");
        expensesHeaderTitle.Blocks.Add(expensesHeaderTitleBlock);
        targetIndex = 0;
        foreach (var year in cfr.Select(x => x.Year))
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
        foreach (var expenses in cfr.Select(x => x.OperatingExpenses))
        {
            var dynamicCell = totalOperatingExpenses.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{expenses:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        editor.Position.Translate(pageOne.Size.Width / 2 - incomeTable.Measure().Width / 2, incomeTable.Measure().Height + 210);
        editor.DrawTable(expensesTable, new Size(incomeTable.Measure().Width, double.PositiveInfinity));

        // Net Income
        var netTableTitle = pageOne.Content.AddTextFragment();
        netTableTitle.FontSize = headerSize;
        netTableTitle.Text = "Net";
        netTableTitle.Position.Translate(pageOne.Size.Width / 2 - 5, incomeTable.Measure().Height + expensesTable.Measure().Height + 300);

        var netTable = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(cellPadding) },
            Borders = new TableBorders(blackBorder),
            LayoutType = TableLayoutType.FixedWidth
        };

        var netHeader = netTable.Rows.AddTableRow();
        var netOperatingIncome = netTable.Rows.AddTableRow();
        var capitalReserves = netTable.Rows.AddTableRow();
        var cashBeforeDebtService = netTable.Rows.AddTableRow();
        var annualDebtService = netTable.Rows.AddTableRow();
        var cashFlowBeforeTax = netTable.Rows.AddTableRow();

        var netHeaderTitle = netHeader.Cells.AddTableCell();
        var netHeaderTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        netHeaderTitleBlock.TextProperties.Font = FontsRepository.HelveticaBold;
        netHeaderTitleBlock.InsertText("For The Years Ending");
        netHeaderTitle.Blocks.Add(netHeaderTitleBlock);
        targetIndex = 0;
        foreach (var year in cfr.Select(x => x.Year))
        {
            var dynamicCell = netHeader.Cells.AddTableCell();
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

        var netOperatingIncomeTitle = netOperatingIncome.Cells.AddTableCell();
        var netOperatingIncomeTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        netOperatingIncomeTitleBlock.InsertText("Net Operating Income (NOI)");
        netOperatingIncomeTitle.Blocks.Add(netOperatingIncomeTitleBlock);
        foreach (var noi in cfr.Select(x => x.NetOperatingIncome))
        {
            var dynamicCell = netOperatingIncome.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{noi:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        var capitalReservesTitle = capitalReserves.Cells.AddTableCell();
        var capitalReservesTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        capitalReservesTitleBlock.InsertText("Less Capital Reserves");
        capitalReservesTitle.Blocks.Add(capitalReservesTitleBlock);
        foreach (var capitalReserve in cfr.Select(x => x.CapitalReserves))
        {
            var dynamicCell = capitalReserves.Cells.AddTableCell();
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{capitalReserve:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        var cashbBeforeDebtServiceTitle = cashBeforeDebtService.Cells.AddTableCell();
        var cashbBeforeDebtServiceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        cashbBeforeDebtServiceTitleBlock.InsertText("Cash Before Debt Service");
        cashbBeforeDebtServiceTitle.Blocks.Add(cashbBeforeDebtServiceTitleBlock);
        foreach (var cash in cfr.Select(x => x.CashFlowBeforeDebtService))
        {
            var dynamicCell = cashBeforeDebtService.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{cash:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        var annualDebtServiceTitle = annualDebtService.Cells.AddTableCell();
        var annualDebtServiceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        annualDebtServiceTitleBlock.InsertText("Less Debt Service - Annual");
        annualDebtServiceTitle.Blocks.Add(annualDebtServiceTitleBlock);
        foreach (var debt in cfr.Select(x => x.DebtService))
        {
            var dynamicCell = annualDebtService.Cells.AddTableCell();
            var dynamicCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            dynamicCellBlock.InsertText($"{debt:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        var cashFlowBeforeTaxTitle = cashFlowBeforeTax.Cells.AddTableCell();
        var cashFlowBeforeTaxTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        cashFlowBeforeTaxTitleBlock.TextProperties.Font = FontsRepository.HelveticaBold;
        cashFlowBeforeTaxTitleBlock.InsertText("Cash Flow Before Taxes");
        cashFlowBeforeTaxTitle.Blocks.Add(cashFlowBeforeTaxTitleBlock);
        foreach (var cash in cfr.Select(x => x.TotalCashFlow))
        {
            var dynamicCell = cashFlowBeforeTax.Cells.AddTableCell();
            dynamicCell.Background = new RgbColor(248, 249, 250);
            var dynamicCellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                TextProperties = { Font = FontsRepository.HelveticaBold }
            };
            dynamicCellBlock.InsertText($"{cash:C2}");
            dynamicCell.Blocks.Add(dynamicCellBlock);
        }

        editor.Position.Translate(pageOne.Size.Width / 2 - incomeTable.Measure().Width / 2, incomeTable.Measure().Height + expensesTable.Measure().Height + 310);
        editor.DrawTable(netTable, new Size(incomeTable.Measure().Width, double.PositiveInfinity));

        // conclusion
        var dateBox = pageOne.Content.AddTextFragment();
        dateBox.Text = $"{property.Name} - {DateTime.Now:MM/dd/yyyy}";
        dateBox.Position.Translate(pageOne.Size.Width - 250, pageOne.Size.Height - 10);
    }
}