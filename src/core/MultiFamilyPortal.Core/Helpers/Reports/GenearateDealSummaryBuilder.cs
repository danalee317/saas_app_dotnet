using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;

namespace MultiFamilyPortal.Helpers.Reports;

public static class GenearateDealSummaryBuilder
{
    public static void GenerateDealSummary(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var pageSize = new Size(1503, 1273);
        var headerSize = 18;
        var cellPadding = 22;
        var page = document.Pages.AddPage();
        page.Size = pageSize;
        var editor = new FixedContentEditor(page);

        var textFragment = page.Content.AddTextFragment();
        textFragment.Text = "Deal Summary";
        textFragment.Position.Translate(page.Size.Width / 2 - 50, 50);
        textFragment.FontSize = headerSize + 10;
        var blackBorder = new Border(1, new RgbColor(0, 0, 0));

        BasicAssumptionsTable(editor, blackBorder, property, cellPadding);
        ProjectedPerformanceTable(editor, blackBorder, property, pageSize.Width * 3 / 4, cellPadding);
        CashFlowTable(editor, blackBorder, property, cellPadding);

        // conclusion
        var dateBox = page.Content.AddTextFragment();
        dateBox.Text = $"{property.Name} - {DateTime.Now:MM/dd/yyyy}";
        dateBox.Position.Translate(page.Size.Width - 250, page.Size.Height - 10);
    }

    private static void BasicAssumptionsTable(FixedContentEditor editor, Border border, UnderwritingAnalysis property, double padding = 22)
    {
        var basicAssumptionsTable = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
            Borders = new TableBorders(border)
        };

        BasicAssumptionsHeader(basicAssumptionsTable);
        StartDateRow(basicAssumptionsTable, property);
        DesiredYieldRow(basicAssumptionsTable, property);
        HoldYearsRow(basicAssumptionsTable, property);

        editor.Position.Translate(50, 100);
        editor.DrawTable(basicAssumptionsTable);
    }

    private static void ProjectedPerformanceTable(FixedContentEditor editor, Border border, UnderwritingAnalysis property, double widthStart, double padding = 22)
    {
        var projectedPerformanceTable = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.AutoFit,
            Borders = new TableBorders(border)
        };

        ProjectedPerformanceHeader(projectedPerformanceTable);
        ProjectedPerformanceHeaderTwo(projectedPerformanceTable);
        ProjectedPerformancePrice(projectedPerformanceTable, property);
        ProjectedPerformanceUnits(projectedPerformanceTable, property);
        ProjectedPerformancePricePerUnit(projectedPerformanceTable, property);
        ProjectedPerformanceYearBuilt(projectedPerformanceTable, property);
        ProjectedPerformanceActualCapRate(projectedPerformanceTable, property);
        ProjectedPerformanceActualPDSCR(projectedPerformanceTable, property);
        ProjectedPerformancePCoC(projectedPerformanceTable, property);
        ProjectedPerformanceHeaderThree(projectedPerformanceTable);
        ProjectedPerformanceReversionValue(projectedPerformanceTable, property);
        ProjectedPerformanceReversionCapRate(projectedPerformanceTable, property);
        ProjectedPerformanceNPV(projectedPerformanceTable, property);
        ProjectedPerformanceIRR(projectedPerformanceTable, property);
        ProjectedPerformanceRCoC(projectedPerformanceTable, property);
        ProjectedPerformanceTotalReturn(projectedPerformanceTable, property);

        editor.Position.Translate(widthStart, 100);
        editor.DrawTable(projectedPerformanceTable);
    }

    private static void CashFlowTable(FixedContentEditor editor, Border border, UnderwritingAnalysis property, double padding = 22)
    {
        var scheduledRentYear0 = property.Projections.Select(x => x.GrossScheduledRent).FirstOrDefault();
        var scheduledRentYear1 = property.Projections.Select(x => x.GrossScheduledRent).ToList()[1];
        scheduledRentYear0 = Math.Abs(scheduledRentYear0) == 0 ? 1:scheduledRentYear0 ;
        scheduledRentYear1 = Math.Abs(scheduledRentYear1) == 0 ? 1: scheduledRentYear1 ;
        var numberOfUnits = property.Units;
        var lineBorder = new TableCellBorders(new Border(1, new RgbColor(100, 100, 100)), null, null, null);

        var cashFlowTable = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.AutoFit,
            Borders = new TableBorders(border)
        };

        CashFlowHeaderOne(cashFlowTable);
        CashFlowHeaderTwo(cashFlowTable, lineBorder);
        Rent(cashFlowTable, lineBorder, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        Vacancy(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        OtherLosses(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        UtilitiesIncome(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        OtherIncome(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        TotalEffectiveIncome(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        OperatingExpenses(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        NOI(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        CapitalReserves(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        CFBeforeDebtService(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        AnnualDebtService(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);
        CashFlowBeforeTaxes(cashFlowTable, lineBorder, property, scheduledRentYear0, scheduledRentYear1, numberOfUnits);

        editor.Position.Translate(50, 400);
        editor.DrawTable(cashFlowTable);
    }

    #region Basic Assumptions Rows
    private static void BasicAssumptionsHeader(Table table)
    {
        var basicAssumptionsHeader = table.Rows.AddTableRow();
        var basicAssumptionsTitle = basicAssumptionsHeader.Cells.AddTableCell();
        basicAssumptionsTitle.ColumnSpan = 2;
        basicAssumptionsTitle.Background = new RgbColor(137, 207, 240);
        var basicAssumptionsTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        basicAssumptionsTitleBlock.InsertText("Basic Assumptions");
        basicAssumptionsTitle.Blocks.Add(basicAssumptionsTitleBlock);
    }

    private static void StartDateRow(Table table, UnderwritingAnalysis property)
    {
        var basicAssumptionsStartDate = table.Rows.AddTableRow();
        var basicAssumptionsStartDateTitle = basicAssumptionsStartDate.Cells.AddTableCell();
        basicAssumptionsStartDateTitle.Background = new RgbColor(248, 249, 250);
        var basicAssumptionsStartDateTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        basicAssumptionsStartDateTitleBlock.InsertText("Start Date");
        basicAssumptionsStartDateTitle.Blocks.Add(basicAssumptionsStartDateTitleBlock);
        var basicAssumptionsStartDateValue = basicAssumptionsStartDate.Cells.AddTableCell();
        basicAssumptionsStartDateValue.Background = new RgbColor(248, 249, 250);
        var basicAssumptionsStartDateValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        basicAssumptionsStartDateValueBlock.InsertText(property.StartDate.ToString("MM/dd/yyyy"));
        basicAssumptionsStartDateValue.Blocks.Add(basicAssumptionsStartDateValueBlock);
    }

    private static void DesiredYieldRow(Table table, UnderwritingAnalysis property)
    {
        var basicAssumptionsDesiredYield = table.Rows.AddTableRow();
        var basicAssumptionsDesiredYieldTitle = basicAssumptionsDesiredYield.Cells.AddTableCell();
        var basicAssumptionsDesiredYieldTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        basicAssumptionsDesiredYieldTitleBlock.InsertText("Desired Yield");
        basicAssumptionsDesiredYieldTitle.Blocks.Add(basicAssumptionsDesiredYieldTitleBlock);
        var basicAssumptionsDesiredYieldValue = basicAssumptionsDesiredYield.Cells.AddTableCell();
        var basicAssumptionsDesiredYieldValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        basicAssumptionsDesiredYieldValueBlock.InsertText(property.DesiredYield.ToString("P2"));
        basicAssumptionsDesiredYieldValue.Blocks.Add(basicAssumptionsDesiredYieldValueBlock);
    }

    private static void HoldYearsRow(Table table, UnderwritingAnalysis property)
    {
        var basicAssumptionsHoldPeriod = table.Rows.AddTableRow();
        var basicAssumptionsHoldPeriodTitle = basicAssumptionsHoldPeriod.Cells.AddTableCell();
        basicAssumptionsHoldPeriodTitle.Background = new RgbColor(248, 249, 250);
        var basicAssumptionsHoldPeriodTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        basicAssumptionsHoldPeriodTitleBlock.InsertText("Projected Hold Period");
        basicAssumptionsHoldPeriodTitle.Blocks.Add(basicAssumptionsHoldPeriodTitleBlock);
        var basicAssumptionsHoldPeriodValue = basicAssumptionsHoldPeriod.Cells.AddTableCell();
        basicAssumptionsHoldPeriodValue.Background = new RgbColor(248, 249, 250);
        var basicAssumptionsHoldPeriodValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        basicAssumptionsHoldPeriodValueBlock.InsertText(property.HoldYears.ToString() + " years");
        basicAssumptionsHoldPeriodValue.Blocks.Add(basicAssumptionsHoldPeriodValueBlock);

    }
    # endregion

    # region Projected Performance Rows

    private static void ProjectedPerformanceHeader(Table table)
    {
        var projectedPerformanceHeader = table.Rows.AddTableRow();
        var projectedPerformanceTitle = projectedPerformanceHeader.Cells.AddTableCell();
        projectedPerformanceTitle.ColumnSpan = 2;
        projectedPerformanceTitle.Background = new RgbColor(137, 207, 240);
        var projectedPerformanceTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        projectedPerformanceTitleBlock.InsertText("Projected Performance");
        projectedPerformanceTitle.Blocks.Add(projectedPerformanceTitleBlock);
    }

    private static void ProjectedPerformanceHeaderTwo(Table table)
    {
        var projectedPerformanceHeaderTwo = table.Rows.AddTableRow();
        var projectedPerformanceHeaderTwoTitle = projectedPerformanceHeaderTwo.Cells.AddTableCell();
        projectedPerformanceHeaderTwoTitle.ColumnSpan = 2;
        projectedPerformanceHeaderTwoTitle.Background = new RgbColor(239, 222, 205);
        var projectedPerformanceHeaderTwoTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        projectedPerformanceHeaderTwoTitleBlock.InsertText("Purchase");
        projectedPerformanceHeaderTwoTitle.Blocks.Add(projectedPerformanceHeaderTwoTitleBlock);
    }

    private static void ProjectedPerformancePrice(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformancePrice = table.Rows.AddTableRow();
        var projectedPerformancePriceTitle = projectedPerformancePrice.Cells.AddTableCell();
        projectedPerformancePriceTitle.Background = new RgbColor(248, 249, 250);
        var projectedPerformancePriceTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformancePriceTitleBlock.InsertText("Price");
        projectedPerformancePriceTitle.Blocks.Add(projectedPerformancePriceTitleBlock);
        var projectedPerformancePriceValue = projectedPerformancePrice.Cells.AddTableCell();
        projectedPerformancePriceValue.Background = new RgbColor(248, 249, 250);
        var projectedPerformancePriceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformancePriceValueBlock.InsertText(property.PurchasePrice.ToString("C2"));
        projectedPerformancePriceValue.Blocks.Add(projectedPerformancePriceValueBlock);
    }

    private static void ProjectedPerformanceUnits(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceUnits = table.Rows.AddTableRow();
        var projectedPerformanceUnitsTitle = projectedPerformanceUnits.Cells.AddTableCell();
        var projectedPerformanceUnitsTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceUnitsTitleBlock.InsertText("# of Units");
        projectedPerformanceUnitsTitle.Blocks.Add(projectedPerformanceUnitsTitleBlock);
        var projectedPerformanceUnitsValue = projectedPerformanceUnits.Cells.AddTableCell();
        var projectedPerformanceUnitsValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceUnitsValueBlock.InsertText(property.Units.ToString());
        projectedPerformanceUnitsValue.Blocks.Add(projectedPerformanceUnitsValueBlock);
    }

    private static void ProjectedPerformancePricePerUnit(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformancePricePerUnit = table.Rows.AddTableRow();
        var projectedPerformancePricePerUnitTitle = projectedPerformancePricePerUnit.Cells.AddTableCell();
        projectedPerformancePricePerUnitTitle.Background = new RgbColor(248, 249, 250);
        var projectedPerformancePricePerUnitTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformancePricePerUnitTitleBlock.InsertText("Price Per Unit");

        projectedPerformancePricePerUnitTitle.Blocks.Add(projectedPerformancePricePerUnitTitleBlock);
        var projectedPerformancePricePerUnitValue = projectedPerformancePricePerUnit.Cells.AddTableCell();
        projectedPerformancePricePerUnitValue.Background = new RgbColor(248, 249, 250);
        var projectedPerformancePricePerUnitValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformancePricePerUnitValueBlock.InsertText(property.CostPerUnit.ToString("C2"));
        projectedPerformancePricePerUnitValue.Blocks.Add(projectedPerformancePricePerUnitValueBlock);
    }

    private static void ProjectedPerformanceYearBuilt(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceYearBuilt = table.Rows.AddTableRow();
        var projectedPerformanceYearBuiltTitle = projectedPerformanceYearBuilt.Cells.AddTableCell();
        var projectedPerformanceYearBuiltTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceYearBuiltTitleBlock.InsertText("Year Built");
        projectedPerformanceYearBuiltTitle.Blocks.Add(projectedPerformanceYearBuiltTitleBlock);
        var projectedPerformanceYearBuiltValue = projectedPerformanceYearBuilt.Cells.AddTableCell();
        var projectedPerformanceYearBuiltValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceYearBuiltValueBlock.InsertText(property.Vintage.ToString());
        projectedPerformanceYearBuiltValue.Blocks.Add(projectedPerformanceYearBuiltValueBlock);
    }

    private static void ProjectedPerformanceActualCapRate(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceActualCapRate = table.Rows.AddTableRow();
        var projectedPerformanceActualCapRateTitle = projectedPerformanceActualCapRate.Cells.AddTableCell();
        projectedPerformanceActualCapRateTitle.Background = new RgbColor(248, 249, 250);
        var projectedPerformanceActualCapRateTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceActualCapRateTitleBlock.InsertText("Actual Cap Rate");
        projectedPerformanceActualCapRateTitle.Blocks.Add(projectedPerformanceActualCapRateTitleBlock);
        var projectedPerformanceActualCapRateValue = projectedPerformanceActualCapRate.Cells.AddTableCell();
        projectedPerformanceActualCapRateValue.Background = new RgbColor(248, 249, 250);
        var projectedPerformanceActualCapRateValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceActualCapRateValueBlock.InsertText(property.CapRate.ToString("P2"));
        projectedPerformanceActualCapRateValue.Blocks.Add(projectedPerformanceActualCapRateValueBlock);

    }

    private static void ProjectedPerformanceActualPDSCR(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceActualPDSCR = table.Rows.AddTableRow();
        var projectedPerformanceActualPDSCRTitle = projectedPerformanceActualPDSCR.Cells.AddTableCell();
        var projectedPerformanceActualPDSCRTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceActualPDSCRTitleBlock.InsertText("Purchase DSCR");
        projectedPerformanceActualPDSCRTitle.Blocks.Add(projectedPerformanceActualPDSCRTitleBlock);
        var projectedPerformanceActualPDSCRValue = projectedPerformanceActualPDSCR.Cells.AddTableCell();
        var projectedPerformanceActualPDSCRValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceActualPDSCRValueBlock.InsertText(property.DebtCoverage.ToString("N2"));
        projectedPerformanceActualPDSCRValue.Blocks.Add(projectedPerformanceActualPDSCRValueBlock);

    }

    private static void ProjectedPerformancePCoC(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformancePCoC = table.Rows.AddTableRow();
        var projectedPerformancePCoCTitle = projectedPerformancePCoC.Cells.AddTableCell();
        projectedPerformancePCoCTitle.Background = new RgbColor(248, 249, 250);
        var projectedPerformancePCoCTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformancePCoCTitleBlock.InsertText("Purchase CoC Return");
        projectedPerformancePCoCTitle.Blocks.Add(projectedPerformancePCoCTitleBlock);
        var projectedPerformancePCoCValue = projectedPerformancePCoC.Cells.AddTableCell();
        projectedPerformancePCoCValue.Background = new RgbColor(248, 249, 250);
        var projectedPerformancePCoCValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformancePCoCValueBlock.InsertText(property.CashOnCash.ToString("P2"));
        projectedPerformancePCoCValue.Blocks.Add(projectedPerformancePCoCValueBlock);

    }

    private static void ProjectedPerformanceHeaderThree(Table table)
    {
        var projectedPerformanceHeaderThree = table.Rows.AddTableRow();
        var projectedPerformanceSaleTitle = projectedPerformanceHeaderThree.Cells.AddTableCell();
        projectedPerformanceSaleTitle.ColumnSpan = 2;
        projectedPerformanceSaleTitle.Background = new RgbColor(239, 222, 205);
        var projectedPerformanceSaleTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        projectedPerformanceSaleTitleBlock.InsertText("Sale");
        projectedPerformanceSaleTitle.Blocks.Add(projectedPerformanceSaleTitleBlock);
    }

    private static void ProjectedPerformanceReversionValue(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceReversionValue = table.Rows.AddTableRow();
        var projectedPerformanceReversionValueTitle = projectedPerformanceReversionValue.Cells.AddTableCell();
        projectedPerformanceReversionValueTitle.Background = new RgbColor(248, 249, 250);
        var projectedPerformanceReversionValueTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceReversionValueTitleBlock.InsertText("Reversion Value");
        projectedPerformanceReversionValueTitle.Blocks.Add(projectedPerformanceReversionValueTitleBlock);
        var projectedPerformanceReversionValueValue = projectedPerformanceReversionValue.Cells.AddTableCell();
        projectedPerformanceReversionValueValue.Background = new RgbColor(248, 249, 250);
        var projectedPerformanceReversionValueValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceReversionValueValueBlock.InsertText(property.Reversion.ToString("C2"));
        projectedPerformanceReversionValueValue.Blocks.Add(projectedPerformanceReversionValueValueBlock);
    }

    private static void ProjectedPerformanceReversionCapRate(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceReversionCapRate = table.Rows.AddTableRow();
        var projectedPerformanceReversionCapRateTitle = projectedPerformanceReversionCapRate.Cells.AddTableCell();
        var projectedPerformanceReversionCapRateTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceReversionCapRateTitleBlock.InsertText("Reversion Cap Rate");
        projectedPerformanceReversionCapRateTitle.Blocks.Add(projectedPerformanceReversionCapRateTitleBlock);
        var projectedPerformanceReversionCapRateValue = projectedPerformanceReversionCapRate.Cells.AddTableCell();
        var projectedPerformanceReversionCapRateValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceReversionCapRateValueBlock.InsertText(property.ReversionCapRate.ToString("P2"));
        projectedPerformanceReversionCapRateValue.Blocks.Add(projectedPerformanceReversionCapRateValueBlock);
    }

    private static void ProjectedPerformanceNPV(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceNPV = table.Rows.AddTableRow();
        var projectedPerformanceNPVTitle = projectedPerformanceNPV.Cells.AddTableCell();
        projectedPerformanceNPVTitle.Background = new RgbColor(248, 249, 250);
        var projectedPerformanceNPVTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceNPVTitleBlock.InsertText("NPV");
        projectedPerformanceNPVTitle.Blocks.Add(projectedPerformanceNPVTitleBlock);
        var projectedPerformanceNPVValue = projectedPerformanceNPV.Cells.AddTableCell();
        projectedPerformanceNPVValue.Background = new RgbColor(248, 249, 250);
        var projectedPerformanceNPVValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceNPVValueBlock.InsertText(property.NetPresentValue.ToString("C2"));
        projectedPerformanceNPVValue.Blocks.Add(projectedPerformanceNPVValueBlock);
    }

    private static void ProjectedPerformanceIRR(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceIRR = table.Rows.AddTableRow();
        var projectedPerformanceIRRTitle = projectedPerformanceIRR.Cells.AddTableCell();
        var projectedPerformanceIRRTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceIRRTitleBlock.InsertText("IRR");
        projectedPerformanceIRRTitle.Blocks.Add(projectedPerformanceIRRTitleBlock);
        var projectedPerformanceIRRValue = projectedPerformanceIRR.Cells.AddTableCell();
        var projectedPerformanceIRRValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceIRRValueBlock.InsertText(property.InternalRateOfReturn.ToString("P2"));
        projectedPerformanceIRRValue.Blocks.Add(projectedPerformanceIRRValueBlock);

    }

    private static void ProjectedPerformanceRCoC(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceRCoC = table.Rows.AddTableRow();
        var projectedPerformanceRCoCTitle = projectedPerformanceRCoC.Cells.AddTableCell();
        projectedPerformanceRCoCTitle.Background = new RgbColor(248, 249, 250);
        var projectedPerformanceRCoCTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceRCoCTitleBlock.InsertText("CoC Return");
        projectedPerformanceRCoCTitle.Blocks.Add(projectedPerformanceRCoCTitleBlock);
        projectedPerformanceRCoCTitle.Background = new RgbColor(248, 249, 250);
        var projectedPerformanceRCoCValue = projectedPerformanceRCoC.Cells.AddTableCell();
        projectedPerformanceRCoCValue.Background = new RgbColor(248, 249, 250);
        var projectedPerformanceRCoCValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        var gdsr = new DealSummaryReport(property);
        projectedPerformanceRCoCValueBlock.InsertText(gdsr.OurEquityPartnerCoC.ToString("P2"));
        projectedPerformanceRCoCValue.Blocks.Add(projectedPerformanceRCoCValueBlock);

    }

    private static void ProjectedPerformanceTotalReturn(Table table, UnderwritingAnalysis property)
    {
        var projectedPerformanceTotalReturn = table.Rows.AddTableRow();
        var projectedPerformanceTotalReturnTitle = projectedPerformanceTotalReturn.Cells.AddTableCell();
        var projectedPerformanceTotalReturnTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceTotalReturnTitleBlock.InsertText("Total Return");
        projectedPerformanceTotalReturnTitle.Blocks.Add(projectedPerformanceTotalReturnTitleBlock);
        var projectedPerformanceTotalReturnValue = projectedPerformanceTotalReturn.Cells.AddTableCell();
        var projectedPerformanceTotalReturnValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceTotalReturnValueBlock.InsertText("0.00%"); // TODO: Calculate this total Return
        projectedPerformanceTotalReturnValue.Blocks.Add(projectedPerformanceTotalReturnValueBlock);
    }
    #endregion

    #region Cash Flows Rows
    private static void CashFlowHeaderOne(Table table)
    {
        var cashFlowHeaderOne = table.Rows.AddTableRow();
        var cashFlowHeader = cashFlowHeaderOne.Cells.AddTableCell();
        cashFlowHeader.Background = new RgbColor(137, 207, 240);
        var cashFlowHeaderBlock = new Block();
        cashFlowHeader.Blocks.Add(cashFlowHeaderBlock);
        var cashFlowHeaderOneTitle = cashFlowHeaderOne.Cells.AddTableCell();
        cashFlowHeaderOneTitle.Background = new RgbColor(137, 207, 240);
        cashFlowHeaderOneTitle.ColumnSpan = 3;
        var cashFlowHeaderOneTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        cashFlowHeaderOneTitleBlock.InsertText("Cash Flow");
        cashFlowHeaderOneTitle.Blocks.Add(cashFlowHeaderOneTitleBlock);
        var cashFlowHeaderThree = cashFlowHeaderOne.Cells.AddTableCell();
        cashFlowHeaderThree.ColumnSpan = 3;
        cashFlowHeaderThree.Background = new RgbColor(137, 207, 240);
        var cashFlowHeaderThreeBlock = new Block();
        cashFlowHeader.Blocks.Add(cashFlowHeaderThreeBlock);
    }

    private static void CashFlowHeaderTwo(Table table, TableCellBorders lineBorder)
    {
        var cashFlowHeaderTwo = table.Rows.AddTableRow();
        var cashFlowHeaderFour = cashFlowHeaderTwo.Cells.AddTableCell();
        var cashFlowHeaderFourBlock = new Block();
        cashFlowHeaderFour.Blocks.Add(cashFlowHeaderFourBlock);
        var cashFlowHeaderFourTitle = cashFlowHeaderTwo.Cells.AddTableCell();
        cashFlowHeaderFourTitle.Background = new RgbColor(239, 222, 205);
        var cashFlowHeaderFourTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowHeaderFourTitleBlock.InsertText("Stated In Place");
        cashFlowHeaderFourTitle.Blocks.Add(cashFlowHeaderFourTitleBlock);
        var cashFlowHeaderFiveTitle = cashFlowHeaderTwo.Cells.AddTableCell();
        cashFlowHeaderFiveTitle.Background = new RgbColor(239, 222, 205);
        var cashFlowHeaderFiveTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowHeaderFiveTitleBlock.InsertText("Per Unit");
        cashFlowHeaderFiveTitle.Blocks.Add(cashFlowHeaderFiveTitleBlock);
        var cashFlowHeaderSixTitle = cashFlowHeaderTwo.Cells.AddTableCell();
        cashFlowHeaderSixTitle.Background = new RgbColor(239, 222, 205);
        var cashFlowHeaderSixTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowHeaderSixTitleBlock.InsertText("% of ASR");
        cashFlowHeaderSixTitle.Blocks.Add(cashFlowHeaderSixTitleBlock);
        var cashFlowHeaderSevenTitle = cashFlowHeaderTwo.Cells.AddTableCell();
        cashFlowHeaderSevenTitle.Borders = lineBorder;
        cashFlowHeaderSevenTitle.Background = new RgbColor(64, 224, 208);
        var cashFlowHeaderSevenTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowHeaderSevenTitleBlock.InsertText("Year 1");
        cashFlowHeaderSevenTitle.Blocks.Add(cashFlowHeaderSevenTitleBlock);
        var cashFlowHeaderEightTitle = cashFlowHeaderTwo.Cells.AddTableCell();
        cashFlowHeaderEightTitle.Background = new RgbColor(64, 224, 208);
        var cashFlowHeaderEightTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowHeaderEightTitleBlock.InsertText("Per Unit");
        cashFlowHeaderEightTitle.Blocks.Add(cashFlowHeaderEightTitleBlock);
        var cashFlowHeaderNineTitle = cashFlowHeaderTwo.Cells.AddTableCell();
        cashFlowHeaderNineTitle.Background = new RgbColor(64, 224, 208);
        var cashFlowHeaderNineTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowHeaderNineTitleBlock.InsertText("% of ASR");
        cashFlowHeaderNineTitle.Blocks.Add(cashFlowHeaderNineTitleBlock);
    }

    private static void Rent(Table table, TableCellBorders lineBorder, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var rent = table.Rows.AddTableRow();
        var rentTitle = rent.Cells.AddTableCell();
        var rentTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Left
        };
        rentTitleBlock.InsertText("Actual Scheduled Rents (ASR) @ 100%");
        rentTitle.Blocks.Add(rentTitleBlock);
        var rentStatedInPlace = rent.Cells.AddTableCell();
        rentStatedInPlace.Background = new RgbColor(248, 249, 250);
        var rentStatedInPlaceBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentStatedInPlaceBlock.InsertText(scheduledRentYear0.ToString("C2"));
        rentStatedInPlace.Blocks.Add(rentStatedInPlaceBlock);
        var rentPerUnit = rent.Cells.AddTableCell();
        rentPerUnit.Background = new RgbColor(248, 249, 250);
        var rentPerUnitBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentPerUnitBlock.InsertText((scheduledRentYear0 / numberOfUnits).ToString("C2"));
        rentPerUnit.Blocks.Add(rentPerUnitBlock);
        var rentPercentage = rent.Cells.AddTableCell();
        rentPercentage.Background = new RgbColor(248, 249, 250);
        var rentPercentageBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentPercentageBlock.InsertText((scheduledRentYear0 / scheduledRentYear0).ToString("P2"));
        rentPercentage.Blocks.Add(rentPercentageBlock);
        var rentYear1 = rent.Cells.AddTableCell();
        rentYear1.Borders = lineBorder;
        rentYear1.Background = new RgbColor(248, 249, 250);
        var rentYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentYear1Block.InsertText(scheduledRentYear1.ToString("C2"));
        rentYear1.Blocks.Add(rentYear1Block);
        var rentPerUnitYear1 = rent.Cells.AddTableCell();
        rentPerUnitYear1.Background = new RgbColor(248, 249, 250);
        var rentPerUnitYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentPerUnitYear1Block.InsertText((scheduledRentYear1 / numberOfUnits).ToString("C2"));
        rentPerUnitYear1.Blocks.Add(rentPerUnitYear1Block);
        var rentPercentageYear1 = rent.Cells.AddTableCell();
        rentPercentageYear1.Background = new RgbColor(248, 249, 250);
        var rentPercentageYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentPercentageYear1Block.InsertText((scheduledRentYear1 / scheduledRentYear1).ToString("P2"));
        rentPercentageYear1.Blocks.Add(rentPercentageYear1Block);
    }

    private static void Vacancy(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var vacancy = table.Rows.AddTableRow();
        var vacancyTitle = vacancy.Cells.AddTableCell();
        var vacancyTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyTitleBlock.InsertText("less Vacancy");
        vacancyTitle.Blocks.Add(vacancyTitleBlock);
        var vacancyStatedInPlace = vacancy.Cells.AddTableCell();
        var vacancyStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.Vacancy).FirstOrDefault().ToString("C2"));
        vacancyStatedInPlace.Blocks.Add(vacancyStatedInPlaceBlock);
        var vacancyPerUnit = vacancy.Cells.AddTableCell();
        var vacancyPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyPerUnitBlock.InsertText((property.Projections.Select(x => x.Vacancy).FirstOrDefault() / numberOfUnits).ToString("C2"));
        vacancyPerUnit.Blocks.Add(vacancyPerUnitBlock);
        var vacancyPercentage = vacancy.Cells.AddTableCell();
        var vacancyPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyPercentageBlock.InsertText((property.Projections.Select(x => x.Vacancy).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        vacancyPercentage.Blocks.Add(vacancyPercentageBlock);
        var vacancyYear1 = vacancy.Cells.AddTableCell();
        vacancyYear1.Borders = lineBorder;
        var vacancyYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyYear1Block.InsertText(property.Projections.Select(x => x.Vacancy).Skip(1).FirstOrDefault().ToString("C2"));
        vacancyYear1.Blocks.Add(vacancyYear1Block);
        var vacancyPerUnitYear1 = vacancy.Cells.AddTableCell();
        var vacancyPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyPerUnitYear1Block.InsertText((property.Projections.Select(x => x.Vacancy).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        vacancyPerUnitYear1.Blocks.Add(vacancyPerUnitYear1Block);
        var vacancyPercentageYear1 = vacancy.Cells.AddTableCell();
        var vacancyPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyPercentageYear1Block.InsertText((property.Projections.Select(x => x.Vacancy).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        vacancyPercentageYear1.Blocks.Add(vacancyPercentageYear1Block);
    }

    private static void OtherLosses(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var otherLosses = table.Rows.AddTableRow();
        var otherLossesTitle = otherLosses.Cells.AddTableCell();
        var otherLossesTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesTitleBlock.InsertText("less Other Losses");
        otherLossesTitle.Blocks.Add(otherLossesTitleBlock);
        var otherLossesStatedInPlace = otherLosses.Cells.AddTableCell();
        otherLossesStatedInPlace.Background = new RgbColor(248, 249, 250);
        var otherLossesStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.ConcessionsNonPayment).FirstOrDefault().ToString("C2"));
        otherLossesStatedInPlace.Blocks.Add(otherLossesStatedInPlaceBlock);
        var otherLossesPerUnit = otherLosses.Cells.AddTableCell();
        otherLossesPerUnit.Background = new RgbColor(248, 249, 250);
        var otherLossesPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesPerUnitBlock.InsertText((property.Projections.Select(x => x.ConcessionsNonPayment).FirstOrDefault() / numberOfUnits).ToString("C2"));
        otherLossesPerUnit.Blocks.Add(otherLossesPerUnitBlock);
        var otherLossesPercentage = otherLosses.Cells.AddTableCell();
        otherLossesPercentage.Background = new RgbColor(248, 249, 250);
        var otherLossesPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesPercentageBlock.InsertText((property.Projections.Select(x => x.ConcessionsNonPayment).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        otherLossesPercentage.Blocks.Add(otherLossesPercentageBlock);
        var otherLossesYear1 = otherLosses.Cells.AddTableCell();
        otherLossesYear1.Borders = lineBorder;
        otherLossesYear1.Background = new RgbColor(248, 249, 250);
        var otherLossesYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesYear1Block.InsertText(property.Projections.Select(x => x.ConcessionsNonPayment).Skip(1).FirstOrDefault().ToString("C2"));
        otherLossesYear1.Blocks.Add(otherLossesYear1Block);
        var otherLossesPerUnitYear1 = otherLosses.Cells.AddTableCell();
        otherLossesPerUnitYear1.Background = new RgbColor(248, 249, 250);
        var otherLossesPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesPerUnitYear1Block.InsertText((property.Projections.Select(x => x.ConcessionsNonPayment).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        otherLossesPerUnitYear1.Blocks.Add(otherLossesPerUnitYear1Block);
        var otherLossesPercentageYear1 = otherLosses.Cells.AddTableCell();
        otherLossesPercentageYear1.Background = new RgbColor(248, 249, 250);
        var otherLossesPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesPercentageYear1Block.InsertText((property.Projections.Select(x => x.ConcessionsNonPayment).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        otherLossesPercentageYear1.Blocks.Add(otherLossesPercentageYear1Block);
    }

    private static void UtilitiesIncome(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var utilitiesIncome = table.Rows.AddTableRow();
        var utilitiesIncomeTitle = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomeTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomeTitleBlock.InsertText("plus Utilities Income");
        utilitiesIncomeTitle.Blocks.Add(utilitiesIncomeTitleBlock);
        var utilitiesIncomeStatedInPlace = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomeStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomeStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.UtilityReimbursement).FirstOrDefault().ToString("C2"));
        utilitiesIncomeStatedInPlace.Blocks.Add(utilitiesIncomeStatedInPlaceBlock);
        var utilitiesIncomePerUnit = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomePerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomePerUnitBlock.InsertText((property.Projections.Select(x => x.UtilityReimbursement).FirstOrDefault() / numberOfUnits).ToString("C2"));
        utilitiesIncomePerUnit.Blocks.Add(utilitiesIncomePerUnitBlock);
        var utilitiesIncomePercentage = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomePercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomePercentageBlock.InsertText((property.Projections.Select(x => x.UtilityReimbursement).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        utilitiesIncomePercentage.Blocks.Add(utilitiesIncomePercentageBlock);
        var utilitiesIncomeYear1 = utilitiesIncome.Cells.AddTableCell();
        utilitiesIncomeYear1.Borders = lineBorder;
        var utilitiesIncomeYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomeYear1Block.InsertText(property.Projections.Select(x => x.UtilityReimbursement).Skip(1).FirstOrDefault().ToString("C2"));
        utilitiesIncomeYear1.Blocks.Add(utilitiesIncomeYear1Block);
        var utilitiesIncomePerUnitYear1 = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomePerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomePerUnitYear1Block.InsertText((property.Projections.Select(x => x.UtilityReimbursement).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        utilitiesIncomePerUnitYear1.Blocks.Add(utilitiesIncomePerUnitYear1Block);
        var utilitiesIncomePercentageYear1 = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomePercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomePercentageYear1Block.InsertText((property.Projections.Select(x => x.UtilityReimbursement).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        utilitiesIncomePercentageYear1.Blocks.Add(utilitiesIncomePercentageYear1Block);
    }

    private static void OtherIncome(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var otherIncome = table.Rows.AddTableRow();
        var otherIncomeTitle = otherIncome.Cells.AddTableCell();
        var otherIncomeTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomeTitleBlock.InsertText("plus Other Income");
        otherIncomeTitle.Blocks.Add(otherIncomeTitleBlock);
        var otherIncomeStatedInPlace = otherIncome.Cells.AddTableCell();
        otherIncomeStatedInPlace.Background = new RgbColor(248, 249, 250);
        var otherIncomeStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomeStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.OtherIncome).FirstOrDefault().ToString("C2"));
        otherIncomeStatedInPlace.Blocks.Add(otherIncomeStatedInPlaceBlock);
        var otherIncomePerUnit = otherIncome.Cells.AddTableCell();
        otherIncomePerUnit.Background = new RgbColor(248, 249, 250);
        var otherIncomePerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomePerUnitBlock.InsertText((property.Projections.Select(x => x.OtherIncome).FirstOrDefault() / numberOfUnits).ToString("C2"));
        otherIncomePerUnit.Blocks.Add(otherIncomePerUnitBlock);
        var otherIncomePercentage = otherIncome.Cells.AddTableCell();
        otherIncomePercentage.Background = new RgbColor(248, 249, 250);
        var otherIncomePercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomePercentageBlock.InsertText((property.Projections.Select(x => x.OtherIncome).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        otherIncomePercentage.Blocks.Add(otherIncomePercentageBlock);
        var otherIncomeYear1 = otherIncome.Cells.AddTableCell();
        otherIncomeYear1.Borders = lineBorder;
        otherIncomeYear1.Background = new RgbColor(248, 249, 250);
        var otherIncomeYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomeYear1Block.InsertText(property.Projections.Select(x => x.OtherIncome).Skip(1).FirstOrDefault().ToString("C2"));
        otherIncomeYear1.Blocks.Add(otherIncomeYear1Block);
        var otherIncomePerUnitYear1 = otherIncome.Cells.AddTableCell();
        otherIncomePerUnitYear1.Background = new RgbColor(248, 249, 250);
        var otherIncomePerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomePerUnitYear1Block.InsertText((property.Projections.Select(x => x.OtherIncome).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        otherIncomePerUnitYear1.Blocks.Add(otherIncomePerUnitYear1Block);
        var otherIncomePercentageYear1 = otherIncome.Cells.AddTableCell();
        otherIncomePercentageYear1.Background = new RgbColor(248, 249, 250);
        var otherIncomePercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomePercentageYear1Block.InsertText((property.Projections.Select(x => x.OtherIncome).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        otherIncomePercentageYear1.Blocks.Add(otherIncomePercentageYear1Block);
    }

    private static void TotalEffectiveIncome(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var totalEffectiveIncome = table.Rows.AddTableRow();
        var totalEffectiveIncomeTitle = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomeTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomeTitleBlock.InsertText("Total Effective Income");
        totalEffectiveIncomeTitle.Blocks.Add(totalEffectiveIncomeTitleBlock);
        var totalEffectiveIncomeStatedInPlace = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomeStatedInPlaceBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomeStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.EffectiveGrossIncome).FirstOrDefault().ToString("C2"));
        totalEffectiveIncomeStatedInPlace.Blocks.Add(totalEffectiveIncomeStatedInPlaceBlock);
        var totalEffectiveIncomePerUnit = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomePerUnitBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomePerUnitBlock.InsertText((property.Projections.Select(x => x.EffectiveGrossIncome).FirstOrDefault() / numberOfUnits).ToString("C2"));
        totalEffectiveIncomePerUnit.Blocks.Add(totalEffectiveIncomePerUnitBlock);
        var totalEffectiveIncomePercentage = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomePercentageBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomePercentageBlock.InsertText((property.Projections.Select(x => x.EffectiveGrossIncome).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        totalEffectiveIncomePercentage.Blocks.Add(totalEffectiveIncomePercentageBlock);
        var totalEffectiveIncomeYear1 = totalEffectiveIncome.Cells.AddTableCell();
        totalEffectiveIncomeYear1.Borders = lineBorder;
        var totalEffectiveIncomeYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomeYear1Block.InsertText(property.Projections.Select(x => x.EffectiveGrossIncome).Skip(1).FirstOrDefault().ToString("C2"));
        totalEffectiveIncomeYear1.Blocks.Add(totalEffectiveIncomeYear1Block);
        var totalEffectiveIncomePerUnitYear1 = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomePerUnitYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomePerUnitYear1Block.InsertText((property.Projections.Select(x => x.EffectiveGrossIncome).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        totalEffectiveIncomePerUnitYear1.Blocks.Add(totalEffectiveIncomePerUnitYear1Block);
        var totalEffectiveIncomePercentageYear1 = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomePercentageYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomePercentageYear1Block.InsertText((property.Projections.Select(x => x.EffectiveGrossIncome).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        totalEffectiveIncomePercentageYear1.Blocks.Add(totalEffectiveIncomePercentageYear1Block);
    }

    private static void OperatingExpenses(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var operatingExpenses = table.Rows.AddTableRow();
        var operatingExpensesTitle = operatingExpenses.Cells.AddTableCell();
        var operatingExpensesTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesTitleBlock.InsertText("less Operating Expenses");
        operatingExpensesTitle.Blocks.Add(operatingExpensesTitleBlock);
        var operatingExpensesStatedInPlace = operatingExpenses.Cells.AddTableCell();
        operatingExpensesStatedInPlace.Background = new RgbColor(248, 249, 250);
        var operatingExpensesStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.OperatingExpenses).FirstOrDefault().ToString("C2"));
        operatingExpensesStatedInPlace.Blocks.Add(operatingExpensesStatedInPlaceBlock);
        var operatingExpensesPerUnit = operatingExpenses.Cells.AddTableCell();
        operatingExpensesPerUnit.Background = new RgbColor(248, 249, 250);
        var operatingExpensesPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesPerUnitBlock.InsertText((property.Projections.Select(x => x.OperatingExpenses).FirstOrDefault() / numberOfUnits).ToString("C2"));
        operatingExpensesPerUnit.Blocks.Add(operatingExpensesPerUnitBlock);
        var operatingExpensesPercentage = operatingExpenses.Cells.AddTableCell();
        operatingExpensesPercentage.Background = new RgbColor(248, 249, 250);
        var operatingExpensesPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesPercentageBlock.InsertText((property.Projections.Select(x => x.OperatingExpenses).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        operatingExpensesPercentage.Blocks.Add(operatingExpensesPercentageBlock);
        var operatingExpensesYear1 = operatingExpenses.Cells.AddTableCell();
        operatingExpensesYear1.Borders = lineBorder;
        operatingExpensesYear1.Background = new RgbColor(248, 249, 250);
        var operatingExpensesYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesYear1Block.InsertText(property.Projections.Select(x => x.OperatingExpenses).Skip(1).FirstOrDefault().ToString("C2"));
        operatingExpensesYear1.Blocks.Add(operatingExpensesYear1Block);
        var operatingExpensesPerUnitYear1 = operatingExpenses.Cells.AddTableCell();
        operatingExpensesPerUnitYear1.Background = new RgbColor(248, 249, 250);
        var operatingExpensesPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesPerUnitYear1Block.InsertText((property.Projections.Select(x => x.OperatingExpenses).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        operatingExpensesPerUnitYear1.Blocks.Add(operatingExpensesPerUnitYear1Block);
        var operatingExpensesPercentageYear1 = operatingExpenses.Cells.AddTableCell();
        operatingExpensesPercentageYear1.Background = new RgbColor(248, 249, 250);
        var operatingExpensesPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesPercentageYear1Block.InsertText((property.Projections.Select(x => x.OperatingExpenses).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        operatingExpensesPercentageYear1.Blocks.Add(operatingExpensesPercentageYear1Block);


    }

    private static void NOI(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var NOI = table.Rows.AddTableRow();
        var noiTitle = NOI.Cells.AddTableCell();
        var noiTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiTitleBlock.InsertText("Net Operating Income");
        noiTitle.Blocks.Add(noiTitleBlock);
        var noiStatedInPlace = NOI.Cells.AddTableCell();
        var noiStatedInPlaceBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.NetOperatingIncome).FirstOrDefault().ToString("C2"));
        noiStatedInPlace.Blocks.Add(noiStatedInPlaceBlock);
        var noiPerUnit = NOI.Cells.AddTableCell();
        var noiPerUnitBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiPerUnitBlock.InsertText((property.Projections.Select(x => x.NetOperatingIncome).FirstOrDefault() / numberOfUnits).ToString("C2"));
        noiPerUnit.Blocks.Add(noiPerUnitBlock);
        var noiPercentage = NOI.Cells.AddTableCell();
        var noiPercentageBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiPercentageBlock.InsertText((property.Projections.Select(x => x.NetOperatingIncome).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        noiPercentage.Blocks.Add(noiPercentageBlock);
        var noiYear1 = NOI.Cells.AddTableCell();
        noiYear1.Borders = lineBorder;
        var noiYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiYear1Block.InsertText(property.Projections.Select(x => x.NetOperatingIncome).Skip(1).FirstOrDefault().ToString("C2"));
        noiYear1.Blocks.Add(noiYear1Block);
        var noiPerUnitYear1 = NOI.Cells.AddTableCell();
        var noiPerUnitYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiPerUnitYear1Block.InsertText((property.Projections.Select(x => x.NetOperatingIncome).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        noiPerUnitYear1.Blocks.Add(noiPerUnitYear1Block);
        var noiPercentageYear1 = NOI.Cells.AddTableCell();
        var noiPercentageYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiPercentageYear1Block.InsertText((property.Projections.Select(x => x.NetOperatingIncome).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        noiPercentageYear1.Blocks.Add(noiPercentageYear1Block);

    }

    private static void CapitalReserves(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var captialReserves = table.Rows.AddTableRow();
        var captialReservesTitle = captialReserves.Cells.AddTableCell();
        var captialReservesTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesTitleBlock.InsertText("less Capital Reserves");
        captialReservesTitle.Blocks.Add(captialReservesTitleBlock);
        var captialReservesStatedInPlace = captialReserves.Cells.AddTableCell();
        captialReservesStatedInPlace.Background = new RgbColor(248, 249, 250);
        var captialReservesStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.CapitalReserves).FirstOrDefault().ToString("C2"));
        captialReservesStatedInPlace.Blocks.Add(captialReservesStatedInPlaceBlock);
        var captialReservesPerUnit = captialReserves.Cells.AddTableCell();
        captialReservesPerUnit.Background = new RgbColor(248, 249, 250);
        var captialReservesPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesPerUnitBlock.InsertText((property.Projections.Select(x => x.CapitalReserves).FirstOrDefault() / numberOfUnits).ToString("C2"));
        captialReservesPerUnit.Blocks.Add(captialReservesPerUnitBlock);
        var captialReservesPercentage = captialReserves.Cells.AddTableCell();
        captialReservesPercentage.Background = new RgbColor(248, 249, 250);
        var captialReservesPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesPercentageBlock.InsertText((property.Projections.Select(x => x.CapitalReserves).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        captialReservesPercentage.Blocks.Add(captialReservesPercentageBlock);
        var captialReservesYear1 = captialReserves.Cells.AddTableCell();
        captialReservesYear1.Borders = lineBorder;
        captialReservesYear1.Background = new RgbColor(248, 249, 250);
        var captialReservesYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesYear1Block.InsertText(property.Projections.Select(x => x.CapitalReserves).Skip(1).FirstOrDefault().ToString("C2"));
        captialReservesYear1.Blocks.Add(captialReservesYear1Block);
        var captialReservesPerUnitYear1 = captialReserves.Cells.AddTableCell();
        captialReservesPerUnitYear1.Background = new RgbColor(248, 249, 250);
        var captialReservesPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesPerUnitYear1Block.InsertText((property.Projections.Select(x => x.CapitalReserves).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        captialReservesPerUnitYear1.Blocks.Add(captialReservesPerUnitYear1Block);
        var captialReservesPercentageYear1 = captialReserves.Cells.AddTableCell();
        captialReservesPercentageYear1.Background = new RgbColor(248, 249, 250);
        var captialReservesPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesPercentageYear1Block.InsertText((property.Projections.Select(x => x.CapitalReserves).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        captialReservesPercentageYear1.Blocks.Add(captialReservesPercentageYear1Block);

    }

    private static void CFBeforeDebtService(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var cFBeforeDebtService = table.Rows.AddTableRow();
        var cFBeforeDebtServiceTitle = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServiceTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServiceTitleBlock.InsertText("Cash Flow Before Debt Service");
        cFBeforeDebtServiceTitle.Blocks.Add(cFBeforeDebtServiceTitleBlock);
        var cFBeforeDebtServiceStatedInPlace = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServiceStatedInPlaceBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServiceStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.CashFlowBeforeDebtService).FirstOrDefault().ToString("C2"));
        cFBeforeDebtServiceStatedInPlace.Blocks.Add(cFBeforeDebtServiceStatedInPlaceBlock);
        var cFBeforeDebtServicePerUnit = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServicePerUnitBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServicePerUnitBlock.InsertText((property.Projections.Select(x => x.CashFlowBeforeDebtService).FirstOrDefault() / numberOfUnits).ToString("C2"));
        cFBeforeDebtServicePerUnit.Blocks.Add(cFBeforeDebtServicePerUnitBlock);
        var cFBeforeDebtServicePercentage = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServicePercentageBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServicePercentageBlock.InsertText((property.Projections.Select(x => x.CashFlowBeforeDebtService).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        cFBeforeDebtServicePercentage.Blocks.Add(cFBeforeDebtServicePercentageBlock);
        var cFBeforeDebtServiceYear1 = cFBeforeDebtService.Cells.AddTableCell();
        cFBeforeDebtServiceYear1.Borders = lineBorder;
        var cFBeforeDebtServiceYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServiceYear1Block.InsertText(property.Projections.Select(x => x.CashFlowBeforeDebtService).Skip(1).FirstOrDefault().ToString("C2"));
        cFBeforeDebtServiceYear1.Blocks.Add(cFBeforeDebtServiceYear1Block);
        var cFBeforeDebtServicePerUnitYear1 = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServicePerUnitYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServicePerUnitYear1Block.InsertText((property.Projections.Select(x => x.CashFlowBeforeDebtService).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        cFBeforeDebtServicePerUnitYear1.Blocks.Add(cFBeforeDebtServicePerUnitYear1Block);
        var cFBeforeDebtServicePercentageYear1 = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServicePercentageYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServicePercentageYear1Block.InsertText((property.Projections.Select(x => x.CashFlowBeforeDebtService).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        cFBeforeDebtServicePercentageYear1.Blocks.Add(cFBeforeDebtServicePercentageYear1Block);
    }

    private static void AnnualDebtService(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var annualDebtService = table.Rows.AddTableRow();
        var annualDebtServiceTitle = annualDebtService.Cells.AddTableCell();
        var annualDebtServiceTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServiceTitleBlock.InsertText("less Annual Debt Service");
        annualDebtServiceTitle.Blocks.Add(annualDebtServiceTitleBlock);
        var annualDebtServiceStatedInPlace = annualDebtService.Cells.AddTableCell();
        annualDebtServiceStatedInPlace.Background = new RgbColor(248, 249, 250);
        var annualDebtServiceStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServiceStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.DebtService).FirstOrDefault().ToString("C2"));
        annualDebtServiceStatedInPlace.Blocks.Add(annualDebtServiceStatedInPlaceBlock);
        var annualDebtServicePerUnit = annualDebtService.Cells.AddTableCell();
        annualDebtServicePerUnit.Background = new RgbColor(248, 249, 250);
        var annualDebtServicePerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServicePerUnitBlock.InsertText((property.Projections.Select(x => x.DebtService).FirstOrDefault() / numberOfUnits).ToString("C2"));
        annualDebtServicePerUnit.Blocks.Add(annualDebtServicePerUnitBlock);
        var annualDebtServicePercentage = annualDebtService.Cells.AddTableCell();
        annualDebtServicePercentage.Background = new RgbColor(248, 249, 250);
        var annualDebtServicePercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServicePercentageBlock.InsertText((property.Projections.Select(x => x.DebtService).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        annualDebtServicePercentage.Blocks.Add(annualDebtServicePercentageBlock);
        var annualDebtServiceYear1 = annualDebtService.Cells.AddTableCell();
        annualDebtServiceYear1.Borders = lineBorder;
        annualDebtServiceYear1.Background = new RgbColor(248, 249, 250);
        var annualDebtServiceYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServiceYear1Block.InsertText(property.Projections.Select(x => x.DebtService).Skip(1).FirstOrDefault().ToString("C2"));
        annualDebtServiceYear1.Blocks.Add(annualDebtServiceYear1Block);
        var annualDebtServicePerUnitYear1 = annualDebtService.Cells.AddTableCell();
        annualDebtServicePerUnitYear1.Background = new RgbColor(248, 249, 250);
        var annualDebtServicePerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServicePerUnitYear1Block.InsertText((property.Projections.Select(x => x.DebtService).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        annualDebtServicePerUnitYear1.Blocks.Add(annualDebtServicePerUnitYear1Block);
        var annualDebtServicePercentageYear1 = annualDebtService.Cells.AddTableCell();
        annualDebtServicePercentageYear1.Background = new RgbColor(248, 249, 250);
        var annualDebtServicePercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServicePercentageYear1Block.InsertText((property.Projections.Select(x => x.DebtService).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        annualDebtServicePercentageYear1.Blocks.Add(annualDebtServicePercentageYear1Block);
    }

    private static void CashFlowBeforeTaxes(Table table, TableCellBorders lineBorder, UnderwritingAnalysis property, double scheduledRentYear0, double scheduledRentYear1, int numberOfUnits)
    {
        var cashFlowBeforeTaxes = table.Rows.AddTableRow();
        var cashFlowBeforeTaxesTitle = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesTitleBlock.InsertText("Cash Flow Before Taxes");
        cashFlowBeforeTaxesTitle.Blocks.Add(cashFlowBeforeTaxesTitleBlock);
        var cashFlowBeforeTaxesStatedInPlace = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesStatedInPlaceBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesStatedInPlaceBlock.InsertText(property.Projections.Select(x => x.TotalCashFlow).FirstOrDefault().ToString("C2"));
        cashFlowBeforeTaxesStatedInPlace.Blocks.Add(cashFlowBeforeTaxesStatedInPlaceBlock);
        var cashFlowBeforeTaxesPerUnit = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesPerUnitBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesPerUnitBlock.InsertText((property.Projections.Select(x => x.TotalCashFlow).FirstOrDefault() / numberOfUnits).ToString("C2"));
        cashFlowBeforeTaxesPerUnit.Blocks.Add(cashFlowBeforeTaxesPerUnitBlock);
        var cashFlowBeforeTaxesPercentage = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesPercentageBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesPercentageBlock.InsertText((property.Projections.Select(x => x.TotalCashFlow).FirstOrDefault() / scheduledRentYear0).ToString("P2"));
        cashFlowBeforeTaxesPercentage.Blocks.Add(cashFlowBeforeTaxesPercentageBlock);
        var cashFlowBeforeTaxesYear1 = cashFlowBeforeTaxes.Cells.AddTableCell();
        cashFlowBeforeTaxesYear1.Borders = lineBorder;
        var cashFlowBeforeTaxesYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesYear1Block.InsertText(property.Projections.Select(x => x.TotalCashFlow).Skip(1).FirstOrDefault().ToString("C2"));
        cashFlowBeforeTaxesYear1.Blocks.Add(cashFlowBeforeTaxesYear1Block);
        var cashFlowBeforeTaxesPerUnitYear1 = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesPerUnitYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesPerUnitYear1Block.InsertText((property.Projections.Select(x => x.TotalCashFlow).Skip(1).FirstOrDefault() / numberOfUnits).ToString("C2"));
        cashFlowBeforeTaxesPerUnitYear1.Blocks.Add(cashFlowBeforeTaxesPerUnitYear1Block);
        var cashFlowBeforeTaxesPercentageYear1 = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesPercentageYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesPercentageYear1Block.InsertText((property.Projections.Select(x => x.TotalCashFlow).Skip(1).FirstOrDefault() / scheduledRentYear1).ToString("P2"));
        cashFlowBeforeTaxesPercentageYear1.Blocks.Add(cashFlowBeforeTaxesPercentageYear1Block);
    }
    # endregion
}
