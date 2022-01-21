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

public static class GenearateDealSumaryBuilder
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

        // Basic Assumptions
        var basicAssumptionsTable = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(cellPadding) },
            LayoutType = TableLayoutType.FixedWidth,
        };
        basicAssumptionsTable.Borders = new TableBorders(blackBorder);

        var basicAssumptionsHeader = basicAssumptionsTable.Rows.AddTableRow();
        var basicAssumptionsStartDate = basicAssumptionsTable.Rows.AddTableRow();
        var basicAssumptionsDesiredYield = basicAssumptionsTable.Rows.AddTableRow();
        var basicAssumptionsHoldPeriod = basicAssumptionsTable.Rows.AddTableRow();

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

        var basicAssumptionsStartDateTitle = basicAssumptionsStartDate.Cells.AddTableCell();
        var basicAssumptionsStartDateTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        basicAssumptionsStartDateTitleBlock.InsertText("Start Date");
        basicAssumptionsStartDateTitle.Blocks.Add(basicAssumptionsStartDateTitleBlock);
        var basicAssumptionsStartDateValue = basicAssumptionsStartDate.Cells.AddTableCell();
        var basicAssumptionsStartDateValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        basicAssumptionsStartDateValueBlock.InsertText(property.StartDate.ToString("MM/dd/yyyy"));
        basicAssumptionsStartDateValue.Blocks.Add(basicAssumptionsStartDateValueBlock);

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

        var basicAssumptionsHoldPeriodTitle = basicAssumptionsHoldPeriod.Cells.AddTableCell();
        var basicAssumptionsHoldPeriodTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        basicAssumptionsHoldPeriodTitleBlock.InsertText("Projected Hold Period");
        basicAssumptionsHoldPeriodTitle.Blocks.Add(basicAssumptionsHoldPeriodTitleBlock);
        var basicAssumptionsHoldPeriodValue = basicAssumptionsHoldPeriod.Cells.AddTableCell();
        var basicAssumptionsHoldPeriodValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        basicAssumptionsHoldPeriodValueBlock.InsertText(property.HoldYears.ToString() + " years");
        basicAssumptionsHoldPeriodValue.Blocks.Add(basicAssumptionsHoldPeriodValueBlock);

        editor.Position.Translate(50, 100);
        editor.DrawTable(basicAssumptionsTable);

        // Projected Performance
        var projectedPerformanceTable = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(cellPadding) },
            LayoutType = TableLayoutType.AutoFit,
        };

        projectedPerformanceTable.Borders = new TableBorders(blackBorder);

        var projectedPerformanceHeader = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceHeaderTwo = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformancePrice = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceUnits = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformancePricePerUnit = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceYearBuilt = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceActualCapRate = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceActualPDSCR = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformancePCoC = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceHeaderThree = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceReversionValue = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceReversionCapRate = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceNPV = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceIRR = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceRCoC = projectedPerformanceTable.Rows.AddTableRow();
        var projectedPerformanceTotalReturn = projectedPerformanceTable.Rows.AddTableRow();

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

        var projectedPerformancePriceTitle = projectedPerformancePrice.Cells.AddTableCell();
        var projectedPerformancePriceTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformancePriceTitleBlock.InsertText("Price");
        projectedPerformancePriceTitle.Blocks.Add(projectedPerformancePriceTitleBlock);
        var projectedPerformancePriceValue = projectedPerformancePrice.Cells.AddTableCell();
        var projectedPerformancePriceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformancePriceValueBlock.InsertText(property.PurchasePrice.ToString("C2"));
        projectedPerformancePriceValue.Blocks.Add(projectedPerformancePriceValueBlock);

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

        var projectedPerformancePricePerUnitTitle = projectedPerformancePricePerUnit.Cells.AddTableCell();
        var projectedPerformancePricePerUnitTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformancePricePerUnitTitleBlock.InsertText("Price Per Unit");

        projectedPerformancePricePerUnitTitle.Blocks.Add(projectedPerformancePricePerUnitTitleBlock);
        var projectedPerformancePricePerUnitValue = projectedPerformancePricePerUnit.Cells.AddTableCell();
        var projectedPerformancePricePerUnitValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformancePricePerUnitValueBlock.InsertText(property.CostPerUnit.ToString("C2"));
        projectedPerformancePricePerUnitValue.Blocks.Add(projectedPerformancePricePerUnitValueBlock);

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

        var projectedPerformanceActualCapRateTitle = projectedPerformanceActualCapRate.Cells.AddTableCell();
        var projectedPerformanceActualCapRateTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceActualCapRateTitleBlock.InsertText("Actual Cap Rate");
        projectedPerformanceActualCapRateTitle.Blocks.Add(projectedPerformanceActualCapRateTitleBlock);
        var projectedPerformanceActualCapRateValue = projectedPerformanceActualCapRate.Cells.AddTableCell();
        var projectedPerformanceActualCapRateValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceActualCapRateValueBlock.InsertText(property.CapRate.ToString("P2"));
        projectedPerformanceActualCapRateValue.Blocks.Add(projectedPerformanceActualCapRateValueBlock);

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

        var projectedPerformancePCoCTitle = projectedPerformancePCoC.Cells.AddTableCell();
        var projectedPerformancePCoCTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformancePCoCTitleBlock.InsertText("Purchase CoC Return");
        projectedPerformancePCoCTitle.Blocks.Add(projectedPerformancePCoCTitleBlock);
        var projectedPerformancePCoCValue = projectedPerformancePCoC.Cells.AddTableCell();
        var projectedPerformancePCoCValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformancePCoCValueBlock.InsertText(property.CashOnCash.ToString("P2"));
        projectedPerformancePCoCValue.Blocks.Add(projectedPerformancePCoCValueBlock);

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

        var projectedPerformanceReversionValueTitle = projectedPerformanceReversionValue.Cells.AddTableCell();
        var projectedPerformanceReversionValueTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceReversionValueTitleBlock.InsertText("Reversion Value");
        projectedPerformanceReversionValueTitle.Blocks.Add(projectedPerformanceReversionValueTitleBlock);
        var projectedPerformanceReversionValueValue = projectedPerformanceReversionValue.Cells.AddTableCell();
        var projectedPerformanceReversionValueValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceReversionValueValueBlock.InsertText(property.Reversion.ToString("C2"));
        projectedPerformanceReversionValueValue.Blocks.Add(projectedPerformanceReversionValueValueBlock);

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

        var projectedPerformanceNPVTitle = projectedPerformanceNPV.Cells.AddTableCell();
        var projectedPerformanceNPVTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceNPVTitleBlock.InsertText("NPV");
        projectedPerformanceNPVTitle.Blocks.Add(projectedPerformanceNPVTitleBlock);
        var projectedPerformanceNPVValue = projectedPerformanceNPV.Cells.AddTableCell();
        var projectedPerformanceNPVValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        projectedPerformanceNPVValueBlock.InsertText(property.NetPresentValue.ToString("C2"));
        projectedPerformanceNPVValue.Blocks.Add(projectedPerformanceNPVValueBlock);

        var projectedPerformanceRCoCTitle = projectedPerformanceRCoC.Cells.AddTableCell();
        var projectedPerformanceRCoCTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        projectedPerformanceRCoCTitleBlock.InsertText("CoC Return");
        projectedPerformanceRCoCTitle.Blocks.Add(projectedPerformanceRCoCTitleBlock);
        var projectedPerformanceRCoCValue = projectedPerformanceRCoC.Cells.AddTableCell();
        var projectedPerformanceRCoCValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        var gdsr = new DealSummaryReport(property);
        projectedPerformanceRCoCValueBlock.InsertText(gdsr.OurEquityPartnerCoC.ToString("P2"));
        projectedPerformanceRCoCValue.Blocks.Add(projectedPerformanceRCoCValueBlock);

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

        editor.Position.Translate(pageSize.Width * 3 / 4, 100);
        editor.DrawTable(projectedPerformanceTable);

        // Cash Flow
        var cashFlowTable = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(cellPadding) },
            LayoutType = TableLayoutType.AutoFit,
        };
        cashFlowTable.Borders = new TableBorders(blackBorder);

        var cashFlowHeaderOne = cashFlowTable.Rows.AddTableRow();
        var cashFlowHeaderTwo = cashFlowTable.Rows.AddTableRow();
        var rent = cashFlowTable.Rows.AddTableRow();
        var vacancy = cashFlowTable.Rows.AddTableRow();
        var otherLosses = cashFlowTable.Rows.AddTableRow();
        var utilitiesIncome = cashFlowTable.Rows.AddTableRow();
        var otherIncome = cashFlowTable.Rows.AddTableRow();
        var totalEffectiveIncome = cashFlowTable.Rows.AddTableRow();
        var operatingExpenses = cashFlowTable.Rows.AddTableRow();
        var NOI = cashFlowTable.Rows.AddTableRow();
        var captialReserves = cashFlowTable.Rows.AddTableRow();
        var cFBeforeDebtService = cashFlowTable.Rows.AddTableRow();
        var annualDebtService = cashFlowTable.Rows.AddTableRow();
        var cashFlowBeforeTaxes = cashFlowTable.Rows.AddTableRow();


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
        cashFlowHeader.Blocks.Add(cashFlowHeaderBlock);

        var cashFlowHeaderFour = cashFlowHeaderTwo.Cells.AddTableCell();
        var cashFlowHeaderFourBlock = new Block();
        cashFlowHeader.Blocks.Add(cashFlowHeaderBlock);
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
        cashFlowHeaderSevenTitle.Background = new RgbColor(255, 240, 245);
        var cashFlowHeaderSevenTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowHeaderSevenTitleBlock.InsertText("Year 1");
        cashFlowHeaderSevenTitle.Blocks.Add(cashFlowHeaderSevenTitleBlock);
        var cashFlowHeaderEightTitle = cashFlowHeaderTwo.Cells.AddTableCell();
        cashFlowHeaderEightTitle.Background = new RgbColor(255, 240, 245);
        var cashFlowHeaderEightTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowHeaderEightTitleBlock.InsertText("Per Unit");
        cashFlowHeaderEightTitle.Blocks.Add(cashFlowHeaderEightTitleBlock);
        var cashFlowHeaderNineTitle = cashFlowHeaderTwo.Cells.AddTableCell();
        cashFlowHeaderNineTitle.Background = new RgbColor(255, 240, 245);
        var cashFlowHeaderNineTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowHeaderNineTitleBlock.InsertText("% of ASR");
        cashFlowHeaderNineTitle.Blocks.Add(cashFlowHeaderNineTitleBlock);

        var rentTitle = rent.Cells.AddTableCell();
        var rentTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Left
        };
        rentTitleBlock.InsertText("Actual Scheduled Rents (ASR) @ 100%");
        rentTitle.Blocks.Add(rentTitleBlock);
        var rentStatedInPlace = rent.Cells.AddTableCell();
        var rentStatedInPlaceBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentStatedInPlaceBlock.InsertText("Some Value");
        rentStatedInPlace.Blocks.Add(rentStatedInPlaceBlock);
        var rentPerUnit = rent.Cells.AddTableCell();
        var rentPerUnitBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentPerUnitBlock.InsertText("Some Value");
        rentPerUnit.Blocks.Add(rentPerUnitBlock);
        var rentPercentage = rent.Cells.AddTableCell();
        var rentPercentageBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentPercentageBlock.InsertText("Some Value");
        rentPercentage.Blocks.Add(rentPercentageBlock);
        var rentYear1 = rent.Cells.AddTableCell();
        var rentYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentYear1Block.InsertText("Some Value");
        rentYear1.Blocks.Add(rentYear1Block);
        var rentPerUnitYear1 = rent.Cells.AddTableCell();
        var rentPerUnitYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentPerUnitYear1Block.InsertText("Some Value");
        rentPerUnitYear1.Blocks.Add(rentPerUnitYear1Block);
        var rentPercentageYear1 = rent.Cells.AddTableCell();
        var rentPercentageYear1Block = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rentPercentageYear1Block.InsertText("Some Value");
        rentPercentageYear1.Blocks.Add(rentPercentageYear1Block);

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
        vacancyStatedInPlaceBlock.InsertText("Some Value");
        vacancyStatedInPlace.Blocks.Add(vacancyStatedInPlaceBlock);
        var vacancyPerUnit = vacancy.Cells.AddTableCell();
        var vacancyPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyPerUnitBlock.InsertText("Some Value");
        vacancyPerUnit.Blocks.Add(vacancyPerUnitBlock);
        var vacancyPercentage = vacancy.Cells.AddTableCell();
        var vacancyPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyPercentageBlock.InsertText("Some Value");
        vacancyPercentage.Blocks.Add(vacancyPercentageBlock);
        var vacancyYear1 = vacancy.Cells.AddTableCell();
        var vacancyYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyYear1Block.InsertText("Some Value");
        vacancyYear1.Blocks.Add(vacancyYear1Block);
        var vacancyPerUnitYear1 = vacancy.Cells.AddTableCell();
        var vacancyPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyPerUnitYear1Block.InsertText("Some Value");
        vacancyPerUnitYear1.Blocks.Add(vacancyPerUnitYear1Block);
        var vacancyPercentageYear1 = vacancy.Cells.AddTableCell();
        var vacancyPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        vacancyPercentageYear1Block.InsertText("Some Value");
        vacancyPercentageYear1.Blocks.Add(vacancyPercentageYear1Block);

        var otherLossesTitle = otherLosses.Cells.AddTableCell();
        var otherLossesTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesTitleBlock.InsertText("less Other Losses");
        otherLossesTitle.Blocks.Add(otherLossesTitleBlock);
        var otherLossesStatedInPlace = otherLosses.Cells.AddTableCell();
        var otherLossesStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesStatedInPlaceBlock.InsertText("Some Value");
        otherLossesStatedInPlace.Blocks.Add(otherLossesStatedInPlaceBlock);
        var otherLossesPerUnit = otherLosses.Cells.AddTableCell();
        var otherLossesPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesPerUnitBlock.InsertText("Some Value");
        otherLossesPerUnit.Blocks.Add(otherLossesPerUnitBlock);
        var otherLossesPercentage = otherLosses.Cells.AddTableCell();
        var otherLossesPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesPercentageBlock.InsertText("Some Value");
        otherLossesPercentage.Blocks.Add(otherLossesPercentageBlock);
        var otherLossesYear1 = otherLosses.Cells.AddTableCell();
        var otherLossesYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesYear1Block.InsertText("Some Value");
        otherLossesYear1.Blocks.Add(otherLossesYear1Block);
        var otherLossesPerUnitYear1 = otherLosses.Cells.AddTableCell();
        var otherLossesPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesPerUnitYear1Block.InsertText("Some Value");
        otherLossesPerUnitYear1.Blocks.Add(otherLossesPerUnitYear1Block);
        var otherLossesPercentageYear1 = otherLosses.Cells.AddTableCell();
        var otherLossesPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherLossesPercentageYear1Block.InsertText("Some Value");
        otherLossesPercentageYear1.Blocks.Add(otherLossesPercentageYear1Block);


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
        utilitiesIncomeStatedInPlaceBlock.InsertText("Some Value");
        utilitiesIncomeStatedInPlace.Blocks.Add(utilitiesIncomeStatedInPlaceBlock);
        var utilitiesIncomePerUnit = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomePerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomePerUnitBlock.InsertText("Some Value");
        utilitiesIncomePerUnit.Blocks.Add(utilitiesIncomePerUnitBlock);
        var utilitiesIncomePercentage = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomePercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomePercentageBlock.InsertText("Some Value");
        utilitiesIncomePercentage.Blocks.Add(utilitiesIncomePercentageBlock);
        var utilitiesIncomeYear1 = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomeYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomeYear1Block.InsertText("Some Value");
        utilitiesIncomeYear1.Blocks.Add(utilitiesIncomeYear1Block);
        var utilitiesIncomePerUnitYear1 = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomePerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomePerUnitYear1Block.InsertText("Some Value");
        utilitiesIncomePerUnitYear1.Blocks.Add(utilitiesIncomePerUnitYear1Block);
        var utilitiesIncomePercentageYear1 = utilitiesIncome.Cells.AddTableCell();
        var utilitiesIncomePercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        utilitiesIncomePercentageYear1Block.InsertText("Some Value");
        utilitiesIncomePercentageYear1.Blocks.Add(utilitiesIncomePercentageYear1Block);

        var otherIncomeTitle = otherIncome.Cells.AddTableCell();
        var otherIncomeTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomeTitleBlock.InsertText("plus Other Income");
        otherIncomeTitle.Blocks.Add(otherIncomeTitleBlock);
        var otherIncomeStatedInPlace = otherIncome.Cells.AddTableCell();
        var otherIncomeStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomeStatedInPlaceBlock.InsertText("Some Value");
        otherIncomeStatedInPlace.Blocks.Add(otherIncomeStatedInPlaceBlock);
        var otherIncomePerUnit = otherIncome.Cells.AddTableCell();
        var otherIncomePerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomePerUnitBlock.InsertText("Some Value");
        otherIncomePerUnit.Blocks.Add(otherIncomePerUnitBlock);
        var otherIncomePercentage = otherIncome.Cells.AddTableCell();
        var otherIncomePercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomePercentageBlock.InsertText("Some Value");
        otherIncomePercentage.Blocks.Add(otherIncomePercentageBlock);
        var otherIncomeYear1 = otherIncome.Cells.AddTableCell();
        var otherIncomeYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomeYear1Block.InsertText("Some Value");
        otherIncomeYear1.Blocks.Add(otherIncomeYear1Block);
        var otherIncomePerUnitYear1 = otherIncome.Cells.AddTableCell();
        var otherIncomePerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomePerUnitYear1Block.InsertText("Some Value");
        otherIncomePerUnitYear1.Blocks.Add(otherIncomePerUnitYear1Block);
        var otherIncomePercentageYear1 = otherIncome.Cells.AddTableCell();
        var otherIncomePercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        otherIncomePercentageYear1Block.InsertText("Some Value");
        otherIncomePercentageYear1.Blocks.Add(otherIncomePercentageYear1Block);

        var totalEffectiveIncomeTitle = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomeTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomeTitleBlock.InsertText("Total Effective Income");
        totalEffectiveIncomeTitle.Blocks.Add(totalEffectiveIncomeTitleBlock);
        var totalEffectiveIncomeStatedInPlace = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomeStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomeStatedInPlaceBlock.InsertText("Some Value");
        totalEffectiveIncomeStatedInPlace.Blocks.Add(totalEffectiveIncomeStatedInPlaceBlock);
        var totalEffectiveIncomePerUnit = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomePerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomePerUnitBlock.InsertText("Some Value");
        totalEffectiveIncomePerUnit.Blocks.Add(totalEffectiveIncomePerUnitBlock);
        var totalEffectiveIncomePercentage = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomePercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomePercentageBlock.InsertText("Some Value");
        totalEffectiveIncomePercentage.Blocks.Add(totalEffectiveIncomePercentageBlock);
        var totalEffectiveIncomeYear1 = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomeYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomeYear1Block.InsertText("Some Value");
        totalEffectiveIncomeYear1.Blocks.Add(totalEffectiveIncomeYear1Block);
        var totalEffectiveIncomePerUnitYear1 = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomePerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomePerUnitYear1Block.InsertText("Some Value");
        totalEffectiveIncomePerUnitYear1.Blocks.Add(totalEffectiveIncomePerUnitYear1Block);
        var totalEffectiveIncomePercentageYear1 = totalEffectiveIncome.Cells.AddTableCell();
        var totalEffectiveIncomePercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        totalEffectiveIncomePercentageYear1Block.InsertText("Some Value");
        totalEffectiveIncomePercentageYear1.Blocks.Add(totalEffectiveIncomePercentageYear1Block);

        var operatingExpensesTitle = operatingExpenses.Cells.AddTableCell();
        var operatingExpensesTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesTitleBlock.InsertText("less Operating Expenses");
        operatingExpensesTitle.Blocks.Add(operatingExpensesTitleBlock);
        var operatingExpensesStatedInPlace = operatingExpenses.Cells.AddTableCell();
        var operatingExpensesStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesStatedInPlaceBlock.InsertText("Some Value");
        operatingExpensesStatedInPlace.Blocks.Add(operatingExpensesStatedInPlaceBlock);
        var operatingExpensesPerUnit = operatingExpenses.Cells.AddTableCell();
        var operatingExpensesPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesPerUnitBlock.InsertText("Some Value");
        operatingExpensesPerUnit.Blocks.Add(operatingExpensesPerUnitBlock);
        var operatingExpensesPercentage = operatingExpenses.Cells.AddTableCell();
        var operatingExpensesPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesPercentageBlock.InsertText("Some Value");
        operatingExpensesPercentage.Blocks.Add(operatingExpensesPercentageBlock);
        var operatingExpensesYear1 = operatingExpenses.Cells.AddTableCell();
        var operatingExpensesYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesYear1Block.InsertText("Some Value");
        operatingExpensesYear1.Blocks.Add(operatingExpensesYear1Block);
        var operatingExpensesPerUnitYear1 = operatingExpenses.Cells.AddTableCell();
        var operatingExpensesPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesPerUnitYear1Block.InsertText("Some Value");
        operatingExpensesPerUnitYear1.Blocks.Add(operatingExpensesPerUnitYear1Block);
        var operatingExpensesPercentageYear1 = operatingExpenses.Cells.AddTableCell();
        var operatingExpensesPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        operatingExpensesPercentageYear1Block.InsertText("Some Value");
        operatingExpensesPercentageYear1.Blocks.Add(operatingExpensesPercentageYear1Block);

        var noiTitle = NOI.Cells.AddTableCell();
        var noiTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiTitleBlock.InsertText("Net Operating Income");
        noiTitle.Blocks.Add(noiTitleBlock);
        var noiStatedInPlace = NOI.Cells.AddTableCell();
        var noiStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiStatedInPlaceBlock.InsertText("Some Value");
        noiStatedInPlace.Blocks.Add(noiStatedInPlaceBlock);
        var noiPerUnit = NOI.Cells.AddTableCell();
        var noiPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiPerUnitBlock.InsertText("Some Value");
        noiPerUnit.Blocks.Add(noiPerUnitBlock);
        var noiPercentage = NOI.Cells.AddTableCell();
        var noiPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiPercentageBlock.InsertText("Some Value");
        noiPercentage.Blocks.Add(noiPercentageBlock);
        var noiYear1 = NOI.Cells.AddTableCell();
        var noiYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiYear1Block.InsertText("Some Value");
        noiYear1.Blocks.Add(noiYear1Block);
        var noiPerUnitYear1 = NOI.Cells.AddTableCell();
        var noiPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiPerUnitYear1Block.InsertText("Some Value");
        noiPerUnitYear1.Blocks.Add(noiPerUnitYear1Block);
        var noiPercentageYear1 = NOI.Cells.AddTableCell();
        var noiPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        noiPercentageYear1Block.InsertText("Some Value");
        noiPercentageYear1.Blocks.Add(noiPercentageYear1Block);

        var captialReservesTitle = captialReserves.Cells.AddTableCell();
        var captialReservesTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesTitleBlock.InsertText("less Capital Reserves");
        captialReservesTitle.Blocks.Add(captialReservesTitleBlock);
        var captialReservesStatedInPlace = captialReserves.Cells.AddTableCell();
        var captialReservesStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesStatedInPlaceBlock.InsertText("Some Value");
        captialReservesStatedInPlace.Blocks.Add(captialReservesStatedInPlaceBlock);
        var captialReservesPerUnit = captialReserves.Cells.AddTableCell();
        var captialReservesPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesPerUnitBlock.InsertText("Some Value");
        captialReservesPerUnit.Blocks.Add(captialReservesPerUnitBlock);
        var captialReservesPercentage = captialReserves.Cells.AddTableCell();
        var captialReservesPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesPercentageBlock.InsertText("Some Value");
        captialReservesPercentage.Blocks.Add(captialReservesPercentageBlock);
        var captialReservesYear1 = captialReserves.Cells.AddTableCell();
        var captialReservesYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesYear1Block.InsertText("Some Value");
        captialReservesYear1.Blocks.Add(captialReservesYear1Block);
        var captialReservesPerUnitYear1 = captialReserves.Cells.AddTableCell();
        var captialReservesPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesPerUnitYear1Block.InsertText("Some Value");
        captialReservesPerUnitYear1.Blocks.Add(captialReservesPerUnitYear1Block);
        var captialReservesPercentageYear1 = captialReserves.Cells.AddTableCell();
        var captialReservesPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        captialReservesPercentageYear1Block.InsertText("Some Value");
        captialReservesPercentageYear1.Blocks.Add(captialReservesPercentageYear1Block);

        var cFBeforeDebtServiceTitle = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServiceTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServiceTitleBlock.InsertText("Cash Flow Before Debt Service");
        cFBeforeDebtServiceTitle.Blocks.Add(cFBeforeDebtServiceTitleBlock);
        var cFBeforeDebtServiceStatedInPlace = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServiceStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServiceStatedInPlaceBlock.InsertText("Some Value");
        cFBeforeDebtServiceStatedInPlace.Blocks.Add(cFBeforeDebtServiceStatedInPlaceBlock);
        var cFBeforeDebtServicePerUnit = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServicePerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServicePerUnitBlock.InsertText("Some Value");
        cFBeforeDebtServicePerUnit.Blocks.Add(cFBeforeDebtServicePerUnitBlock);
        var cFBeforeDebtServicePercentage = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServicePercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServicePercentageBlock.InsertText("Some Value");
        cFBeforeDebtServicePercentage.Blocks.Add(cFBeforeDebtServicePercentageBlock);
        var cFBeforeDebtServiceYear1 = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServiceYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServiceYear1Block.InsertText("Some Value");
        cFBeforeDebtServiceYear1.Blocks.Add(cFBeforeDebtServiceYear1Block);
        var cFBeforeDebtServicePerUnitYear1 = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServicePerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServicePerUnitYear1Block.InsertText("Some Value");
        cFBeforeDebtServicePerUnitYear1.Blocks.Add(cFBeforeDebtServicePerUnitYear1Block);
        var cFBeforeDebtServicePercentageYear1 = cFBeforeDebtService.Cells.AddTableCell();
        var cFBeforeDebtServicePercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cFBeforeDebtServicePercentageYear1Block.InsertText("Some Value");
        cFBeforeDebtServicePercentageYear1.Blocks.Add(cFBeforeDebtServicePercentageYear1Block);

        var annualDebtServiceTitle = annualDebtService.Cells.AddTableCell();
        var annualDebtServiceTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServiceTitleBlock.InsertText("Annual Debt Service");
        annualDebtServiceTitle.Blocks.Add(annualDebtServiceTitleBlock);
        var annualDebtServiceStatedInPlace = annualDebtService.Cells.AddTableCell();
        var annualDebtServiceStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServiceStatedInPlaceBlock.InsertText("Some Value");
        annualDebtServiceStatedInPlace.Blocks.Add(annualDebtServiceStatedInPlaceBlock);
        var annualDebtServicePerUnit = annualDebtService.Cells.AddTableCell();
        var annualDebtServicePerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServicePerUnitBlock.InsertText("Some Value");
        annualDebtServicePerUnit.Blocks.Add(annualDebtServicePerUnitBlock);
        var annualDebtServicePercentage = annualDebtService.Cells.AddTableCell();
        var annualDebtServicePercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServicePercentageBlock.InsertText("Some Value");
        annualDebtServicePercentage.Blocks.Add(annualDebtServicePercentageBlock);
        var annualDebtServiceYear1 = annualDebtService.Cells.AddTableCell();
        var annualDebtServiceYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServiceYear1Block.InsertText("Some Value");
        annualDebtServiceYear1.Blocks.Add(annualDebtServiceYear1Block);
        var annualDebtServicePerUnitYear1 = annualDebtService.Cells.AddTableCell();
        var annualDebtServicePerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServicePerUnitYear1Block.InsertText("Some Value");
        annualDebtServicePerUnitYear1.Blocks.Add(annualDebtServicePerUnitYear1Block);
        var annualDebtServicePercentageYear1 = annualDebtService.Cells.AddTableCell();
        var annualDebtServicePercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        annualDebtServicePercentageYear1Block.InsertText("Some Value");
        annualDebtServicePercentageYear1.Blocks.Add(annualDebtServicePercentageYear1Block);

        var cashFlowBeforeTaxesTitle = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesTitleBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesTitleBlock.InsertText("Cash Flow Before Taxes");
        cashFlowBeforeTaxesTitle.Blocks.Add(cashFlowBeforeTaxesTitleBlock);
        var cashFlowBeforeTaxesStatedInPlace = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesStatedInPlaceBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesStatedInPlaceBlock.InsertText("Some Value");
        cashFlowBeforeTaxesStatedInPlace.Blocks.Add(cashFlowBeforeTaxesStatedInPlaceBlock);
        var cashFlowBeforeTaxesPerUnit = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesPerUnitBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesPerUnitBlock.InsertText("Some Value");
        cashFlowBeforeTaxesPerUnit.Blocks.Add(cashFlowBeforeTaxesPerUnitBlock);
        var cashFlowBeforeTaxesPercentage = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesPercentageBlock = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesPercentageBlock.InsertText("Some Value");
        cashFlowBeforeTaxesPercentage.Blocks.Add(cashFlowBeforeTaxesPercentageBlock);
        var cashFlowBeforeTaxesYear1 = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesYear1Block.InsertText("Some Value");
        cashFlowBeforeTaxesYear1.Blocks.Add(cashFlowBeforeTaxesYear1Block);
        var cashFlowBeforeTaxesPerUnitYear1 = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesPerUnitYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesPerUnitYear1Block.InsertText("Some Value");
        cashFlowBeforeTaxesPerUnitYear1.Blocks.Add(cashFlowBeforeTaxesPerUnitYear1Block);
        var cashFlowBeforeTaxesPercentageYear1 = cashFlowBeforeTaxes.Cells.AddTableCell();
        var cashFlowBeforeTaxesPercentageYear1Block = new Block
        {
            HorizontalAlignment = HorizontalAlignment.Right
        };
        cashFlowBeforeTaxesPercentageYear1Block.InsertText("Some Value");
        cashFlowBeforeTaxesPercentageYear1.Blocks.Add(cashFlowBeforeTaxesPercentageYear1Block);


        editor.Position.Translate(50, basicAssumptionsTable.Measure().Height + 150);
        editor.DrawTable(cashFlowTable, new Size(pageSize.Width * 1 / 2 + 300, double.PositiveInfinity));

        // conclusion
        var dateBox = page.Content.AddTextFragment();
        dateBox.Text = $"{property.Name} - {DateTime.Now:MM/dd/yyyy}";
        dateBox.Position.Translate(page.Size.Width - 250, page.Size.Height - 10);
    }

}
