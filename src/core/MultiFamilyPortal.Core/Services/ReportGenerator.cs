using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
using MultiFamilyPortal.Helpers.Reports;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;

namespace MultiFamilyPortal.Services;

public class ReportGenerator : IReportGenerator
{
    private ILogger<ReportGenerator> _logger { get; }
    private IUnderwritingService _underwritingService { get; }

    public ReportGenerator(ILogger<ReportGenerator> logger, IUnderwritingService underwritingService)
    {
        _logger = logger;
        _underwritingService = underwritingService;
    }

    #region Underwriting Reports
    public async Task<ReportResponse> FullReport(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();
        GenearateDealSummaryBuilder.GenerateDealSummary(property, document);
        GenerateAssumptions(property, document);
        GenerateCashFlow(property, document);
        GenerateIncomeForecast(property, document);
        GenerateCapitalExpenses(property, document);

        var name = $"Overall_Projections.pdf";
        return new ReportResponse
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    public async Task<ReportResponse> DealSummary(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();
        GenearateDealSummaryBuilder.GenerateDealSummary(property, document);

        var name = $"Deal_Summary.pdf";
        return new ReportResponse
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    public async Task<ReportResponse> Assumptions(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();
        GenerateAssumptions(property, document);

        var name = $"Assumptions.pdf";
        return new ReportResponse
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    public async Task<ReportResponse> CashFlow(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();
        GenerateCashFlow(property, document);

        var name = $"Cash_Flow.pdf";
        return new ReportResponse
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    public async Task<ReportResponse> IncomeForecast(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();
        GenerateIncomeForecast(property, document);

        var name = $"Income_Forecast.pdf";
        return new ReportResponse
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    public async Task<ReportResponse> CapitalExpenses(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();
        GenerateCapitalExpenses(property, document);

        var name = $"Capital_Expenses.pdf";
        return new ReportResponse
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    public async Task<ReportResponse> RentRoll(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();
        GenerateRentRoll(property, document);

        var name = $"Rent_Roll.pdf";
        return new ReportResponse
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    public async Task<ReportResponse> LeaseExposure(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();
        GenerateLeaseExposure(property, document);

        var name = $"Lease_Exposure.pdf";
        return new ReportResponse
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    #endregion Underwriting Reports

    #region Investor Reports
    public async Task<ReportResponse> ManagersReturns(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();
        var page = document.Pages.AddPage();

        try
        {
            var mmr = new ManagersReturnsReport(property);
            page.Size = new Size((570 + 135 * mmr.HoldYears), 793);
            FixedContentEditor editor = new(page);


            var textFragment = page.Content.AddTextFragment();
            textFragment.Text = "Manager Return";
            textFragment.Position.Translate(page.Size.Width / 2 - 100, 100);
            textFragment.FontSize = 25;

            var table = new Table { DefaultCellProperties = { Padding = new Thickness(28) } };
            var blackBorder = new Border(1, new RgbColor(0, 0, 0));
            table.Borders = new TableBorders(blackBorder);

            var r0 = table.Rows.AddTableRow();
            var r1 = table.Rows.AddTableRow();
            var r2 = table.Rows.AddTableRow();
            var r3 = table.Rows.AddTableRow();
            var r4 = table.Rows.AddTableRow();
            var r5 = table.Rows.AddTableRow();
            var r6 = table.Rows.AddTableRow();

            // row 0
            var emptycell = r0.Cells.AddTableCell();
            emptycell.Blocks.AddBlock();
            var empty2cell = r0.Cells.AddTableCell();
            empty2cell.Blocks.AddBlock();
            foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            {
                var cell = r0.Cells.AddTableCell();
                var block = new Block { HorizontalAlignment = HorizontalAlignment.Right };
                block.InsertText($"Year {year}");
                cell.Blocks.Add(block);
            }
            var totalhcell = r0.Cells.AddTableCell();
            totalhcell.Blocks.AddBlock().InsertText("Total");

            // row 1
            var afcell = r1.Cells.AddTableCell();
            afcell.Blocks.AddBlock().InsertText("Acquisation Fee");
            var afValuecell = r1.Cells.AddTableCell();
            var afblock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            afblock.InsertText(mmr.AcquisitionFee.ToString("C2"));
            afValuecell.Blocks.Add(afblock);
            foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            {
                var cell = r1.Cells.AddTableCell();
                cell.Blocks.AddBlock();
            }
            var totalvcell = r1.Cells.AddTableCell();
            var totalvblock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalvblock.InsertText(mmr.AcquisitionFee.ToString("C2"));
            totalvcell.Blocks.Add(totalvblock);

            // row 2
            var mecell = r2.Cells.AddTableCell();
            mecell.Blocks.AddBlock().InsertText("Manager Equity");
            var meValuecell = r2.Cells.AddTableCell();
            var meValueblock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            meValueblock.InsertText(mmr.ManagerEquity.ToString("C2"));
            meValuecell.Blocks.Add(meValueblock);
            foreach (var year in Enumerable.Range(1, mmr.HoldYears + 1))
            {
                var cell = r2.Cells.AddTableCell();
                cell.Blocks.AddBlock();
            }

            // row 3
            var cfcell = r3.Cells.AddTableCell();
            cfcell.Blocks.AddBlock().InsertText($"Total Cash Flow");
            foreach (var yearlycashFlow in mmr.CashFlow)
            {
                var cell = r3.Cells.AddTableCell();
                var block = new Block { HorizontalAlignment = HorizontalAlignment.Right };
                block.InsertText(yearlycashFlow.ToString("C2"));
                cell.Blocks.Add(block);
            }

            // row 4
            var cfpcell = r4.Cells.AddTableCell();
            cfpcell.Blocks.AddBlock().InsertText($"Cash Flow ({mmr.CashFlowPercentage.ToString("P2")})");
            var totalMCF = 0d;
            foreach (var yearlycashFlow in mmr.CashFlow)
            {
                var cell = r4.Cells.AddTableCell();
                var block = new Block { HorizontalAlignment = HorizontalAlignment.Right };
                block.InsertText((yearlycashFlow * mmr.CashFlowPercentage).ToString("C2"));
                cell.Blocks.Add(block);
                totalMCF += (yearlycashFlow * mmr.CashFlowPercentage);
            }
            var totalcfpcell = r4.Cells.AddTableCell();
            var totalcfpblock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalcfpblock.InsertText(totalMCF.ToString("C2"));
            totalcfpcell.Blocks.Add(totalcfpblock);

            // row 5
            var epcell = r5.Cells.AddTableCell();
            epcell.Blocks.AddBlock().InsertText("Equity On Sale of Property");
            foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            {
                var cell = r5.Cells.AddTableCell();
                cell.Blocks.AddBlock();
            }
            var totallstcell = r5.Cells.AddTableCell();
            var totallstblock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totallstblock.InsertText(mmr.EqualityOnSaleOfProperty.ToString("C2"));
            totallstcell.Blocks.Add(totallstblock);
            var totalepcell = r5.Cells.AddTableCell();
            var totalepblock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalepblock.InsertText(mmr.EqualityOnSaleOfProperty.ToString("C2"));
            totalepcell.Blocks.Add(totalepblock);

            // row 6
            var totalcell = r6.Cells.AddTableCell();
            totalcell.Blocks.AddBlock().InsertText("Total");
            var totalValuecell = r6.Cells.AddTableCell();
            var totalValueblock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalValueblock.InsertText((mmr.AcquisitionFee + mmr.CashFlow.FirstOrDefault() * mmr.CashFlowPercentage).ToString("C2"));
            totalValuecell.Blocks.Add(totalValueblock);
            int i = 0;
            foreach (var year in mmr.CashFlow)
            {
                var block = new Block { HorizontalAlignment = HorizontalAlignment.Right };
                if (i != 0 && i != mmr.HoldYears)
                {
                    var cell = r6.Cells.AddTableCell();
                    block.InsertText((year * mmr.CashFlowPercentage).ToString("C2"));
                    cell.Blocks.Add(block);
                }
                else if (i == mmr.HoldYears)
                {
                    var cell = r6.Cells.AddTableCell();
                    block.InsertText((mmr.EqualityOnSaleOfProperty + year * mmr.CashFlowPercentage).ToString("C2"));
                    cell.Blocks.Add(block);
                }
                i++;
            }
            var totaltotalcell = r6.Cells.AddTableCell();
            var totaltotalblock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totaltotalblock.InsertText((totalMCF + mmr.EqualityOnSaleOfProperty).ToString("C2"));
            totaltotalcell.Blocks.Add(totaltotalblock);

            var dateBox = page.Content.AddTextFragment();
            dateBox.Text = $"{property.Name} - {DateTime.Now:MM/dd/yyyy}";
            dateBox.Position.Translate(page.Size.Width - 250, page.Size.Height - 10);

            editor.Position.Translate(page.Size.Width / 2 - table.Measure().Width / 2, page.Size.Height / 2 - table.Measure().Height / 2);
            editor.DrawTable(table);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while generating the Manager Report");
        }

        var name = $"Managers_Returns_Report.pdf";
        return new ReportResponse
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    public async Task<ReportResponse> CulmativeInvestment(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();

        return NotFound();
    }

    public async Task<ReportResponse> OneHundredThousandInvestmentProjections(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();

        return NotFound();
    }

    public async Task<ReportResponse> NetPresentValue(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();

        return NotFound();
    }

    public async Task<ReportResponse> LeveragedRateOfReturns(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();

        return NotFound();
    }

    public async Task<ReportResponse> TieredInvestmentGroup(Guid propertyId, string groupName)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return NotFound();

        var document = new RadFixedDocument();

        return NotFound();
    }
    #endregion Investor Reports

    private byte[] ExportToPdf(RadFixedDocument document)
    {
        try
        {
            if (!document.Pages.Any())
                return Array.Empty<byte>();

            PdfFormatProvider provider = new();
            PdfExportSettings settings = new PdfExportSettings
            {
                ImageQuality = ImageQuality.High,
                ComplianceLevel = PdfComplianceLevel.PdfA2B
            };
            provider.ExportSettings = settings;
            return provider.Export(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return Array.Empty<byte>();
        }
    }

    #region Underwriting Report Generators
    private void GenerateAssumptions(UnderwritingAnalysis property, RadFixedDocument document)
    {
        try
        {
            var asr = new AssumptionsReport(property);
            var pageSize = new Size(1323, 2250);
            var headerSize = 18;
            var cellPadding = 22;
            var page = document.Pages.AddPage();
            page.Size = pageSize;
            var editor = new FixedContentEditor(page);

            var textFragment = page.Content.AddTextFragment();
            textFragment.Text = "Assumptions";
            textFragment.Position.Translate(page.Size.Width / 2 - 50, 50);
            textFragment.FontSize = headerSize + 10;

            var tableWidth = page.Size.Width / 3 - 60;

            // Investment Criteria
            var iCTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                LayoutType = TableLayoutType.FixedWidth,
            };
            var blackBorder = new Border(1, new RgbColor(0, 0, 0));
            iCTable.Borders = new TableBorders(blackBorder);

            var iCHeader = iCTable.Rows.AddTableRow();
            var iCData = iCTable.Rows.AddTableRow();

            var iCHeaderTitle = iCHeader.Cells.AddTableCell();
            iCHeaderTitle.Background = new RgbColor(137, 207, 240);
            var iCHeaderTitleBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            iCHeaderTitleBlock.InsertText("Investment Criteria");
            iCHeaderTitle.Blocks.Add(iCHeaderTitleBlock);
            var iCHeaderCell = iCHeader.Cells.AddTableCell();
            iCHeaderCell.Background = new RgbColor(137, 207, 240);
            var iCHeaderBlock = new Block();
            iCHeaderCell.Blocks.Add(iCHeaderBlock);

            var iCDataCell = iCData.Cells.AddTableCell();
            var iCDataTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right, };
            iCDataTitleBlock.InsertText("Hold Period");
            iCDataCell.Blocks.Add(iCDataTitleBlock);
            var iCDataHoldYears = iCData.Cells.AddTableCell();
            var iCDataHoldYearsBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            iCDataHoldYearsBlock.InsertText(property.HoldYears.ToString());
            iCDataHoldYears.Blocks.Add(iCDataHoldYearsBlock);

            editor.Position.Translate(10, 100);
            editor.DrawTable(iCTable, new Size(tableWidth, double.PositiveInfinity));

            // Equity Distribution
            var eDTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                LayoutType = TableLayoutType.FixedWidth,
            };
            eDTable.Borders = new TableBorders(blackBorder);

            var eDHeader = eDTable.Rows.AddTableRow();
            var eDassetEquity = eDTable.Rows.AddTableRow();
            var eDinvestorEquity = eDTable.Rows.AddTableRow();

            var eDHeaderTitle = eDHeader.Cells.AddTableCell();
            eDHeaderTitle.Background = new RgbColor(137, 207, 240);
            var eDHeaderTitleBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            eDHeaderTitleBlock.InsertText("Equity Distribution");
            eDHeaderTitle.Blocks.Add(eDHeaderTitleBlock);
            var eDHeaderCell = eDHeader.Cells.AddTableCell();
            eDHeaderCell.Background = new RgbColor(137, 207, 240);
            var eDHeaderBlock = new Block();
            eDHeaderCell.Blocks.Add(eDHeaderBlock);

            var eDassetEquityTitle = eDassetEquity.Cells.AddTableCell();
            var eDassetEquityTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            eDassetEquityTitleBlock.InsertText("Asset Mgr Equity");
            eDassetEquityTitle.Blocks.Add(eDassetEquityTitleBlock);
            var eDassetEquityPercentage = eDassetEquity.Cells.AddTableCell();
            var eDassetEquityPercentageBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            eDassetEquityPercentageBlock.InsertText(property.OurEquityOfCF.ToString("P2"));
            eDassetEquityPercentage.Blocks.Add(eDassetEquityPercentageBlock);

            var eDinvestorEquityTitle = eDinvestorEquity.Cells.AddTableCell();
            eDinvestorEquityTitle.Background = new RgbColor(248, 249, 250);
            var eDinvestorEquityTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            eDinvestorEquityTitleBlock.InsertText("Investor Equity");
            eDinvestorEquityTitle.Blocks.Add(eDinvestorEquityTitleBlock);
            var eDinvestorEquityPercentage = eDinvestorEquity.Cells.AddTableCell();
            eDinvestorEquityPercentage.Background = new RgbColor(248, 249, 250);
            var eDinvestorEquityPercentageBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            eDinvestorEquityPercentageBlock.InsertText((1 - property.OurEquityOfCF).ToString("P2"));
            eDinvestorEquityPercentage.Blocks.Add(eDinvestorEquityPercentageBlock);

            editor.Position.Translate(iCTable.Measure().Width + 240, 100);
            editor.DrawTable(eDTable, new Size(tableWidth, double.PositiveInfinity));

            // Reversion
            var reversionTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                LayoutType = TableLayoutType.FixedWidth,
            };
            reversionTable.Borders = new TableBorders(blackBorder);

            var reversionHeader = reversionTable.Rows.AddTableRow();
            var reversionCapRate = reversionTable.Rows.AddTableRow();
            var reversionCommission = reversionTable.Rows.AddTableRow();
            var reversionTransferTax = reversionTable.Rows.AddTableRow();

            var reversionHeaderTitle = reversionHeader.Cells.AddTableCell();
            reversionHeaderTitle.Background = new RgbColor(137, 207, 240);
            var reversionHeaderTitleBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            reversionHeaderTitleBlock.InsertText("Reversion");
            reversionHeaderTitle.Blocks.Add(reversionHeaderTitleBlock);
            var reversionHeaderCell = reversionHeader.Cells.AddTableCell();
            reversionHeaderCell.Background = new RgbColor(137, 207, 240);
            var reversionHeaderBlock = new Block();
            reversionHeaderCell.Blocks.Add(reversionHeaderBlock);

            var reversionCapRateTitle = reversionCapRate.Cells.AddTableCell();
            var reversionCapRateTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            reversionCapRateTitleBlock.InsertText("Reversion Cap Rate");
            reversionCapRateTitle.Blocks.Add(reversionCapRateTitleBlock);
            var reversionCapRatePercentage = reversionCapRate.Cells.AddTableCell();
            var reversionCapRatePercentageBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            reversionCapRatePercentageBlock.InsertText(property.ReversionCapRate.ToString("P2"));
            reversionCapRatePercentage.Blocks.Add(reversionCapRatePercentageBlock);

            var reversionCommissionTitle = reversionCommission.Cells.AddTableCell();
            reversionCommissionTitle.Background = new RgbColor(248, 249, 250);
            var reversionCommissionTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            reversionCommissionTitleBlock.InsertText("Commission");
            reversionCommissionTitle.Blocks.Add(reversionCommissionTitleBlock);
            var reversionCommissionPercentage = reversionCommission.Cells.AddTableCell();
            reversionCommissionPercentage.Background = new RgbColor(248, 249, 250);
            var reversionCommissionPercentageBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            reversionCommissionPercentageBlock.InsertText(" - ");
            reversionCommissionPercentage.Blocks.Add(reversionCommissionPercentageBlock);

            var reversionTransferTaxTitle = reversionTransferTax.Cells.AddTableCell();
            var reversionTransferTaxTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            reversionTransferTaxTitleBlock.InsertText("Transfer Tax");
            reversionTransferTaxTitle.Blocks.Add(reversionTransferTaxTitleBlock);
            var reversionTransferTaxPercentage = reversionTransferTax.Cells.AddTableCell();
            var reversionTransferTaxPercentageBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            reversionTransferTaxPercentageBlock.InsertText(" - ");
            reversionTransferTaxPercentage.Blocks.Add(reversionTransferTaxPercentageBlock);

            editor.Position.Translate(iCTable.Measure().Width + eDTable.Measure().Width + 435, 100);
            editor.DrawTable(reversionTable, new Size(tableWidth, double.PositiveInfinity));

            // Equity - Acqusition costs
            var equityAcqusitionCostsTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                LayoutType = TableLayoutType.FixedWidth,
            };
            equityAcqusitionCostsTable.Borders = new TableBorders(blackBorder);

            var equityAcqusitionCostsHeader = equityAcqusitionCostsTable.Rows.AddTableRow();
            var closingCosts = equityAcqusitionCostsTable.Rows.AddTableRow();
            var loanPoints = equityAcqusitionCostsTable.Rows.AddTableRow();
            var acquisitionFee = equityAcqusitionCostsTable.Rows.AddTableRow();
            var totalClosingCosts = equityAcqusitionCostsTable.Rows.AddTableRow();
            var downPaymentPercentage = equityAcqusitionCostsTable.Rows.AddTableRow();
            var downPayment = equityAcqusitionCostsTable.Rows.AddTableRow();
            var insurancePremium = equityAcqusitionCostsTable.Rows.AddTableRow();
            var intialCapitalImprovements = equityAcqusitionCostsTable.Rows.AddTableRow();
            var totalEquityAcqusitionCosts = equityAcqusitionCostsTable.Rows.AddTableRow();

            var equityAcqusitionCostsHeaderTitle = equityAcqusitionCostsHeader.Cells.AddTableCell();
            equityAcqusitionCostsHeaderTitle.Background = new RgbColor(137, 207, 240);
            var equityAcqusitionCostsHeaderTitleBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            equityAcqusitionCostsHeaderTitleBlock.InsertText("Equity (Acqusition costs)");
            equityAcqusitionCostsHeaderTitle.Blocks.Add(equityAcqusitionCostsHeaderTitleBlock);
            var equityAcqusitionCostsHeaderCell = equityAcqusitionCostsHeader.Cells.AddTableCell();
            equityAcqusitionCostsHeaderCell.Background = new RgbColor(137, 207, 240);
            var equityAcqusitionCostsHeaderBlock = new Block();

            var closingCostsTitle = closingCosts.Cells.AddTableCell();
            var closingCostsTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            closingCostsTitleBlock.InsertText("Closing Costs");
            closingCostsTitle.Blocks.Add(closingCostsTitleBlock);
            var closingCostsValue = closingCosts.Cells.AddTableCell();
            var closingCostsValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            closingCostsValueBlock.InsertText(asr.ClosingCosts.ToString("C2"));
            closingCostsValue.Blocks.Add(closingCostsValueBlock);

            var loanPointsTitle = loanPoints.Cells.AddTableCell();
            loanPointsTitle.Background = new RgbColor(248, 249, 250);
            var loanPointsTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanPointsTitleBlock.InsertText("Loan Points");
            loanPointsTitle.Blocks.Add(loanPointsTitleBlock);
            var loanPointsValue = loanPoints.Cells.AddTableCell();
            loanPointsValue.Background = new RgbColor(248, 249, 250);
            var loanPointsValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanPointsValueBlock.InsertText(asr.LoanPoints.ToString("C2"));
            loanPointsValue.Blocks.Add(loanPointsValueBlock);

            var acquisitionFeeTitle = acquisitionFee.Cells.AddTableCell();
            var acquisitionFeeTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            acquisitionFeeTitleBlock.InsertText("Acquisition Fee");
            acquisitionFeeTitle.Blocks.Add(acquisitionFeeTitleBlock);
            var acquisitionFeeValue = acquisitionFee.Cells.AddTableCell();
            var acquisitionFeeValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            acquisitionFeeValueBlock.InsertText(property.AquisitionFee.ToString("C2"));
            acquisitionFeeValue.Blocks.Add(acquisitionFeeValueBlock);

            var totalClosingCostsTitle = totalClosingCosts.Cells.AddTableCell();
            totalClosingCostsTitle.Background = new RgbColor(248, 249, 250);
            var totalClosingCostsTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalClosingCostsTitleBlock.InsertText("Total Closing Costs");
            totalClosingCostsTitle.Blocks.Add(totalClosingCostsTitleBlock);
            var totalClosingCostsValue = totalClosingCosts.Cells.AddTableCell();
            totalClosingCostsValue.Background = new RgbColor(248, 249, 250);
            var totalClosingCostsValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalClosingCostsValueBlock.InsertText((property.AquisitionFee + asr.ClosingCosts + asr.LoanPoints).ToString("C2"));
            totalClosingCostsValue.Blocks.Add(totalClosingCostsValueBlock);

            var downPaymentPercentageTitle = downPaymentPercentage.Cells.AddTableCell();
            var downPaymentPercentageTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            downPaymentPercentageTitleBlock.InsertText("Down Payment");
            downPaymentPercentageTitle.Blocks.Add(downPaymentPercentageTitleBlock);
            var downPaymentPercentageValue = downPaymentPercentage.Cells.AddTableCell();
            var downPaymentPercentageValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            downPaymentPercentageValueBlock.InsertText(asr.DownpaymentPercentage.ToString("P2"));
            downPaymentPercentageValue.Blocks.Add(downPaymentPercentageValueBlock);

            var downPaymentTitle = downPayment.Cells.AddTableCell();
            downPaymentTitle.Background = new RgbColor(248, 249, 250);
            var downPaymentTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            downPaymentTitleBlock.InsertText("Down Payment");
            downPaymentTitle.Blocks.Add(downPaymentTitleBlock);
            var downPaymentValue = downPayment.Cells.AddTableCell();
            downPaymentValue.Background = new RgbColor(248, 249, 250);
            var downPaymentValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            downPaymentValueBlock.InsertText(asr.Downpayment.ToString("C2"));
            downPaymentValue.Blocks.Add(downPaymentValueBlock);

            var insurancePremiumTitle = insurancePremium.Cells.AddTableCell();
            var insurancePremiumTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            insurancePremiumTitleBlock.InsertText("Insurance Premium");
            insurancePremiumTitle.Blocks.Add(insurancePremiumTitleBlock);
            var insurancePremiumValue = insurancePremium.Cells.AddTableCell();
            var insurancePremiumValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            insurancePremiumValueBlock.InsertText(asr.InsurancePremium.ToString("C2"));
            insurancePremiumValue.Blocks.Add(insurancePremiumValueBlock);

            var intialCapitalImprovementsTitle = intialCapitalImprovements.Cells.AddTableCell();
            intialCapitalImprovementsTitle.Background = new RgbColor(248, 249, 250);
            var intialCapitalImprovementsTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            intialCapitalImprovementsTitleBlock.InsertText("Initial Capital Improvements (see breakdown below)");
            intialCapitalImprovementsTitle.Blocks.Add(intialCapitalImprovementsTitleBlock);
            var intialCapitalImprovementsValue = intialCapitalImprovements.Cells.AddTableCell();
            intialCapitalImprovementsValue.Background = new RgbColor(248, 249, 250);
            var intialCapitalImprovementsValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            intialCapitalImprovementsValueBlock.InsertText(asr.CapitalImprovementsBreakDown.Sum(x => x.Value).ToString("C2"));
            intialCapitalImprovementsValue.Blocks.Add(intialCapitalImprovementsValueBlock);

            var totalEquityAcqusitionCostsTitle = totalEquityAcqusitionCosts.Cells.AddTableCell();
            var totalEquityAcqusitionCostsTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalEquityAcqusitionCostsTitleBlock.InsertText("Total Acqusition Costs");
            totalEquityAcqusitionCostsTitle.Blocks.Add(totalEquityAcqusitionCostsTitleBlock);
            var totalEquityAcqusitionCostsValue = totalEquityAcqusitionCosts.Cells.AddTableCell();
            var totalEquityAcqusitionCostsValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalEquityAcqusitionCostsValueBlock.InsertText(asr.TotalEquity.ToString("C2"));
            totalEquityAcqusitionCostsValue.Blocks.Add(totalEquityAcqusitionCostsValueBlock);

            editor.Position.Translate(20, reversionTable.Measure().Height + 150);
            editor.DrawTable(equityAcqusitionCostsTable, new Size(page.Size.Width / 2 - 50, double.PositiveInfinity));

            // Cash Flow
            var cashFlowTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                LayoutType = TableLayoutType.FixedWidth,
            };
            cashFlowTable.Borders = new TableBorders(blackBorder);

            var cashFlowTitle = cashFlowTable.Rows.AddTableRow();
            var rentAbatement = cashFlowTable.Rows.AddTableRow();
            var reimbursementIncome = cashFlowTable.Rows.AddTableRow();
            var generalMinimumVacany = cashFlowTable.Rows.AddTableRow();
            var taxAdustments = cashFlowTable.Rows.AddTableRow();
            var expenseAdjustments = cashFlowTable.Rows.AddTableRow();
            var capitalReserveAdustment = cashFlowTable.Rows.AddTableRow();

            var cashFlowHeader = cashFlowTitle.Cells.AddTableCell();
            cashFlowHeader.Background = new RgbColor(137, 207, 240);
            var cashFlowTitleBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            cashFlowTitleBlock.InsertText("Cash Flow");
            cashFlowHeader.Blocks.Add(cashFlowTitleBlock);
            var cashFlowHeaderCell = cashFlowTitle.Cells.AddTableCell();
            cashFlowHeaderCell.Background = new RgbColor(137, 207, 240);
            var cashFlowHeaderCellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            cashFlowHeaderCellBlock.InsertText("Per Year");
            cashFlowHeaderCell.Blocks.Add(cashFlowHeaderCellBlock);

            var rentAbatementTitle = rentAbatement.Cells.AddTableCell();
            var rentAbatementTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            rentAbatementTitleBlock.InsertText("Rent Abatement");
            rentAbatementTitle.Blocks.Add(rentAbatementTitleBlock);
            var rentAbatementValue = rentAbatement.Cells.AddTableCell();
            var rentAbatementValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            rentAbatementValueBlock.InsertText(" - ");
            rentAbatementValue.Blocks.Add(rentAbatementValueBlock);

            var reimbursementIncomeTitle = reimbursementIncome.Cells.AddTableCell();
            reimbursementIncomeTitle.Background = new RgbColor(248, 249, 250);
            var reimbursementIncomeTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            reimbursementIncomeTitleBlock.InsertText("Annual Adjustment to Expense Reimbursement Income");
            reimbursementIncomeTitle.Blocks.Add(reimbursementIncomeTitleBlock);
            var reimbursementIncomeValue = reimbursementIncome.Cells.AddTableCell();
            reimbursementIncomeValue.Background = new RgbColor(248, 249, 250);
            var reimbursementIncomeValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            reimbursementIncomeValueBlock.InsertText(" - ");
            reimbursementIncomeValue.Blocks.Add(reimbursementIncomeValueBlock);

            var generalMinimumVacanyTitle = generalMinimumVacany.Cells.AddTableCell();
            var generalMinimumVacanyTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            generalMinimumVacanyTitleBlock.InsertText("General Minimum Vacancy");
            generalMinimumVacanyTitle.Blocks.Add(generalMinimumVacanyTitleBlock);
            var generalMinimumVacanyValue = generalMinimumVacany.Cells.AddTableCell();
            var generalMinimumVacanyValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            generalMinimumVacanyValueBlock.InsertText(" - ");
            generalMinimumVacanyValue.Blocks.Add(generalMinimumVacanyValueBlock);

            var taxAdustmentsTitle = taxAdustments.Cells.AddTableCell();
            taxAdustmentsTitle.Background = new RgbColor(248, 249, 250);
            var taxAdustmentsTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            taxAdustmentsTitleBlock.InsertText("Annual Tax Adustment");
            taxAdustmentsTitle.Blocks.Add(taxAdustmentsTitleBlock);
            var taxAdustmentsValue = taxAdustments.Cells.AddTableCell();
            taxAdustmentsValue.Background = new RgbColor(248, 249, 250);
            var taxAdustmentsValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            taxAdustmentsValueBlock.InsertText(" - ");
            taxAdustmentsValue.Blocks.Add(taxAdustmentsValueBlock);

            var expenseAdjustmentsTitle = expenseAdjustments.Cells.AddTableCell();
            var expenseAdjustmentsTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            expenseAdjustmentsTitleBlock.InsertText("Annual Expense Adjustment");
            expenseAdjustmentsTitle.Blocks.Add(expenseAdjustmentsTitleBlock);
            var expenseAdjustmentsValue = expenseAdjustments.Cells.AddTableCell();
            var expenseAdjustmentsValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            expenseAdjustmentsValueBlock.InsertText(" - ");
            expenseAdjustmentsValue.Blocks.Add(expenseAdjustmentsValueBlock);

            var capitalReserveAdustmentTitle = capitalReserveAdustment.Cells.AddTableCell();
            capitalReserveAdustmentTitle.Background = new RgbColor(248, 249, 250);
            var capitalReserveAdustmentTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            capitalReserveAdustmentTitleBlock.InsertText("Annual Additional Capital Reserve Adjustment");
            capitalReserveAdustmentTitle.Blocks.Add(capitalReserveAdustmentTitleBlock);
            var capitalReserveAdustmentValue = capitalReserveAdustment.Cells.AddTableCell();
            capitalReserveAdustmentValue.Background = new RgbColor(248, 249, 250);
            var capitalReserveAdustmentValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            capitalReserveAdustmentValueBlock.InsertText(" - ");
            capitalReserveAdustmentValue.Blocks.Add(capitalReserveAdustmentValueBlock);

            editor.Position.Translate(equityAcqusitionCostsTable.Measure().Width + 200, reversionTable.Measure().Height + 150);
            editor.DrawTable(cashFlowTable, new Size(page.Size.Width / 2 - 50, double.PositiveInfinity));

            // Primary Debt - Aqusition costs
            var primaryDebtAcqusitionCostsTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                LayoutType = TableLayoutType.FixedWidth,
            };
            primaryDebtAcqusitionCostsTable.Borders = new TableBorders(blackBorder);

            var primaryDebtAcqusitionCostsTitle = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var typeOfLoan = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var financedCapitalImprovements = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var loanAmount = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var totalLoanAmount = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var loanToValue = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var interestOnlyPeriod = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var interestRate = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var amortization = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var term = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var debtServiceMonth = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();
            var debtServiceYear = primaryDebtAcqusitionCostsTable.Rows.AddTableRow();

            var primaryDebtAcqusitionCostsTitleHeader = primaryDebtAcqusitionCostsTitle.Cells.AddTableCell();
            primaryDebtAcqusitionCostsTitleHeader.Background = new RgbColor(137, 207, 240);
            var primaryDebtAcqusitionCostsTitleHeaderBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            primaryDebtAcqusitionCostsTitleHeaderBlock.InsertText("Primary Debt (Aqusition)");
            primaryDebtAcqusitionCostsTitleHeader.Blocks.Add(primaryDebtAcqusitionCostsTitleHeaderBlock);
            var primaryDebtAcqusitionCostsTitleHeaderCell = primaryDebtAcqusitionCostsTitle.Cells.AddTableCell();
            primaryDebtAcqusitionCostsTitleHeaderCell.Background = new RgbColor(137, 207, 240);
            var primaryDebtAcqusitionCostsTitleHeaderCellBlock = new Block();
            primaryDebtAcqusitionCostsTitleHeaderCell.Blocks.Add(primaryDebtAcqusitionCostsTitleHeaderCellBlock);

            var typeOfLoanTitle = typeOfLoan.Cells.AddTableCell();
            var typeOfLoanTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            typeOfLoanTitleBlock.InsertText("Type of Loan");
            typeOfLoanTitle.Blocks.Add(typeOfLoanTitleBlock);
            var typeOfLoanValue = typeOfLoan.Cells.AddTableCell();
            var typeOfLoanValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            typeOfLoanValueBlock.InsertText(property.LoanType.ToString());
            typeOfLoanValue.Blocks.Add(typeOfLoanValueBlock);

            var financedCapitalImprovementsTitle = financedCapitalImprovements.Cells.AddTableCell();
            financedCapitalImprovementsTitle.Background = new RgbColor(248, 249, 250);
            var financedCapitalImprovementsTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            financedCapitalImprovementsTitleBlock.InsertText("Intial Capital Improvements (Financed)");
            financedCapitalImprovementsTitle.Blocks.Add(financedCapitalImprovementsTitleBlock);
            var financedCapitalImprovementsValue = financedCapitalImprovements.Cells.AddTableCell();
            financedCapitalImprovementsValue.Background = new RgbColor(248, 249, 250);
            var financedCapitalImprovementsValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            financedCapitalImprovementsValueBlock.InsertText(" - ");
            financedCapitalImprovementsValue.Blocks.Add(financedCapitalImprovementsValueBlock);

            var loanAmountTitle = loanAmount.Cells.AddTableCell();
            var loanAmountTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanAmountTitleBlock.InsertText("Loan Amount");
            loanAmountTitle.Blocks.Add(loanAmountTitleBlock);
            var loanAmountValue = loanAmount.Cells.AddTableCell();
            var loanAmountValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanAmountValueBlock.InsertText(property.Mortgages.Sum(x => x.LoanAmount).ToString("C2"));
            loanAmountValue.Blocks.Add(loanAmountValueBlock);

            var totalLoanAmountTitle = totalLoanAmount.Cells.AddTableCell();
            totalLoanAmountTitle.Background = new RgbColor(248, 249, 250);
            var totalLoanAmountTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalLoanAmountTitleBlock.InsertText("Total Loan Amount");
            totalLoanAmountTitle.Blocks.Add(totalLoanAmountTitleBlock);
            var totalLoanAmountValue = totalLoanAmount.Cells.AddTableCell();
            totalLoanAmountValue.Background = new RgbColor(248, 249, 250);
            var totalLoanAmountValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalLoanAmountValueBlock.InsertText(property.Mortgages.Sum(x => x.LoanAmount).ToString("C2"));
            totalLoanAmountValue.Blocks.Add(totalLoanAmountValueBlock);

            var loanToValueTitle = loanToValue.Cells.AddTableCell();
            var loanToValueTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanToValueTitleBlock.InsertText("Loan to Value");
            loanToValueTitle.Blocks.Add(loanToValueTitleBlock);
            var loanToValueValue = loanToValue.Cells.AddTableCell();
            var loanToValueValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanToValueValueBlock.InsertText(property.LTV.ToString("P2"));
            loanToValueValue.Blocks.Add(loanToValueValueBlock);

            var interestOnlyPeriodTitle = interestOnlyPeriod.Cells.AddTableCell();
            interestOnlyPeriodTitle.Background = new RgbColor(248, 249, 250);
            var interestOnlyPeriodTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestOnlyPeriodTitleBlock.InsertText("Interest Only Period");
            interestOnlyPeriodTitle.Blocks.Add(interestOnlyPeriodTitleBlock);
            var interestOnlyPeriodValue = interestOnlyPeriod.Cells.AddTableCell();
            interestOnlyPeriodValue.Background = new RgbColor(248, 249, 250);
            var interestOnlyPeriodValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestOnlyPeriodValueBlock.InsertText(" - years");
            interestOnlyPeriodValue.Blocks.Add(interestOnlyPeriodValueBlock);

            var interestRateTitle = interestRate.Cells.AddTableCell();
            var interestRateTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestRateTitleBlock.InsertText("Interest Rate");
            interestRateTitle.Blocks.Add(interestRateTitleBlock);
            var interestRateValue = interestRate.Cells.AddTableCell();
            var interestRateValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestRateValueBlock.InsertText(property.Mortgages.Sum(x => x.InterestRate).ToString("P2"));
            interestRateValue.Blocks.Add(interestRateValueBlock);

            var amortizationTitle = amortization.Cells.AddTableCell();
            amortizationTitle.Background = new RgbColor(248, 249, 250);
            var amortizationTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            amortizationTitleBlock.InsertText("Amortization");
            amortizationTitle.Blocks.Add(amortizationTitleBlock);
            var amortizationValue = amortization.Cells.AddTableCell();
            amortizationValue.Background = new RgbColor(248, 249, 250);
            var amortizationValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            amortizationValueBlock.InsertText(" - years");
            amortizationValue.Blocks.Add(amortizationValueBlock);

            var termTitle = term.Cells.AddTableCell();
            var termTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            termTitleBlock.InsertText("Term");
            termTitle.Blocks.Add(termTitleBlock);
            var termValue = term.Cells.AddTableCell();
            var termValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            termValueBlock.InsertText(property.Mortgages.Sum(x => x.TermInYears).ToString() + " years");
            termValue.Blocks.Add(termValueBlock);

            var debtServiceMonthTitle = debtServiceMonth.Cells.AddTableCell();
            debtServiceMonthTitle.Background = new RgbColor(248, 249, 250);
            var debtServiceMonthTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceMonthTitleBlock.InsertText("Debt Service / Month");
            debtServiceMonthTitle.Blocks.Add(debtServiceMonthTitleBlock);
            var debtServiceMonthValue = debtServiceMonth.Cells.AddTableCell();
            debtServiceMonthValue.Background = new RgbColor(248, 249, 250);
            var debtServiceMonthValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceMonthValueBlock.InsertText((property.Mortgages.Sum(x => x.AnnualDebtService) / 12).ToString("C2"));
            debtServiceMonthValue.Blocks.Add(debtServiceMonthValueBlock);

            var debtServiceYearTitle = debtServiceYear.Cells.AddTableCell();
            var debtServiceYearTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceYearTitleBlock.InsertText("Debt Service / Year");
            debtServiceYearTitle.Blocks.Add(debtServiceYearTitleBlock);
            var debtServiceYearValue = debtServiceYear.Cells.AddTableCell();
            var debtServiceYearValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceYearValueBlock.InsertText(property.Mortgages.Sum(x => x.AnnualDebtService).ToString("C2"));
            debtServiceYearValue.Blocks.Add(debtServiceYearValueBlock);

            editor.Position.Translate(20, equityAcqusitionCostsTable.Measure().Height + reversionTable.Measure().Height + 200);
            editor.DrawTable(primaryDebtAcqusitionCostsTable, new Size(tableWidth + 30, double.PositiveInfinity));

            // Supplemnental debt - Aqusition costs
            var supplementalDebtAcqusitionCostsTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                LayoutType = TableLayoutType.FixedWidth,
            };
            supplementalDebtAcqusitionCostsTable.Borders = new TableBorders(blackBorder);

            var supplementalDebtAcqusitionCostsTitle = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var loanCostSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var adddedCapitalImprovementsSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var loanAmountSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var totalLoanAmountSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var loanToValueSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var interestOnlyPeriodSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var interestRateSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var amortizationSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var termSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var debtServiceMonthSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();
            var debtServiceYearSupplemental = supplementalDebtAcqusitionCostsTable.Rows.AddTableRow();

            var supplementalDebtAcqusitionCostsTitleHeader = supplementalDebtAcqusitionCostsTitle.Cells.AddTableCell();
            supplementalDebtAcqusitionCostsTitleHeader.Background = new RgbColor(137, 207, 240);
            var supplementalDebtAcqusitionCostsTitleHeaderBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            supplementalDebtAcqusitionCostsTitleHeaderBlock.InsertText("Supplemental Debt (Aqusition)");
            supplementalDebtAcqusitionCostsTitleHeader.Blocks.Add(supplementalDebtAcqusitionCostsTitleHeaderBlock);
            var supplementalDebtAcqusitionCostsTitleHeaderCell = supplementalDebtAcqusitionCostsTitle.Cells.AddTableCell();
            supplementalDebtAcqusitionCostsTitleHeaderCell.Background = new RgbColor(137, 207, 240);
            var supplementalDebtAcqusitionCostsTitleHeaderCellBlock = new Block();
            supplementalDebtAcqusitionCostsTitleHeaderCell.Blocks.Add(supplementalDebtAcqusitionCostsTitleHeaderCellBlock);

            var loanCostSupplementalTitle = loanCostSupplemental.Cells.AddTableCell();
            var loanCostSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanCostSupplementalTitleBlock.InsertText("Loan Cost (Financed)");
            loanCostSupplementalTitle.Blocks.Add(loanCostSupplementalTitleBlock);
            var loanCostSupplementalValue = loanCostSupplemental.Cells.AddTableCell();
            var loanCostSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanCostSupplementalValueBlock.InsertText(" - ");
            loanCostSupplementalValue.Blocks.Add(loanCostSupplementalValueBlock);

            var adddedCapitalImprovementsSupplementalTitle = adddedCapitalImprovementsSupplemental.Cells.AddTableCell();
            adddedCapitalImprovementsSupplementalTitle.Background = new RgbColor(248, 249, 250);
            var adddedCapitalImprovementsSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            adddedCapitalImprovementsSupplementalTitleBlock.InsertText("Added Capital Improvements (Financed)");
            adddedCapitalImprovementsSupplementalTitle.Blocks.Add(adddedCapitalImprovementsSupplementalTitleBlock);
            var adddedCapitalImprovementsSupplementalValue = adddedCapitalImprovementsSupplemental.Cells.AddTableCell();
            adddedCapitalImprovementsSupplementalValue.Background = new RgbColor(248, 249, 250);
            var adddedCapitalImprovementsSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            adddedCapitalImprovementsSupplementalValueBlock.InsertText(" - ");
            adddedCapitalImprovementsSupplementalValue.Blocks.Add(adddedCapitalImprovementsSupplementalValueBlock);

            var loanAmountSupplementalTitle = loanAmountSupplemental.Cells.AddTableCell();
            var loanAmountSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanAmountSupplementalTitleBlock.InsertText("Loan Amount");
            loanAmountSupplementalTitle.Blocks.Add(loanAmountSupplementalTitleBlock);
            var loanAmountSupplementalValue = loanAmountSupplemental.Cells.AddTableCell();
            var loanAmountSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanAmountSupplementalValueBlock.InsertText(" - ");
            loanAmountSupplementalValue.Blocks.Add(loanAmountSupplementalValueBlock);

            var totalLoanAmountSupplementalTitle = totalLoanAmountSupplemental.Cells.AddTableCell();
            totalLoanAmountSupplementalTitle.Background = new RgbColor(248, 249, 250);
            var totalLoanAmountSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalLoanAmountSupplementalTitleBlock.InsertText("Total Loan Amount");
            totalLoanAmountSupplementalTitle.Blocks.Add(totalLoanAmountSupplementalTitleBlock);
            var totalLoanAmountSupplementalValue = totalLoanAmountSupplemental.Cells.AddTableCell();
            totalLoanAmountSupplementalValue.Background = new RgbColor(248, 249, 250);
            var totalLoanAmountSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalLoanAmountSupplementalValueBlock.InsertText(" - ");
            totalLoanAmountSupplementalValue.Blocks.Add(totalLoanAmountSupplementalValueBlock);

            var loanToValueSupplementalTitle = loanToValueSupplemental.Cells.AddTableCell();
            var loanToValueSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanToValueSupplementalTitleBlock.InsertText("Loan to Value");
            loanToValueSupplementalTitle.Blocks.Add(loanToValueSupplementalTitleBlock);
            var loanToValueSupplementalValue = loanToValueSupplemental.Cells.AddTableCell();
            var loanToValueSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanToValueSupplementalValueBlock.InsertText(" - ");
            loanToValueSupplementalValue.Blocks.Add(loanToValueSupplementalValueBlock);

            var interestOnlyPeriodSupplementalTitle = interestOnlyPeriodSupplemental.Cells.AddTableCell();
            interestOnlyPeriodSupplementalTitle.Background = new RgbColor(248, 249, 250);
            var interestOnlyPeriodSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestOnlyPeriodSupplementalTitleBlock.InsertText("Interest Only Period");
            interestOnlyPeriodSupplementalTitle.Blocks.Add(interestOnlyPeriodSupplementalTitleBlock);
            var interestOnlyPeriodSupplementalValue = interestOnlyPeriodSupplemental.Cells.AddTableCell();
            interestOnlyPeriodSupplementalValue.Background = new RgbColor(248, 249, 250);
            var interestOnlyPeriodSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestOnlyPeriodSupplementalValueBlock.InsertText(" - ");
            interestOnlyPeriodSupplementalValue.Blocks.Add(interestOnlyPeriodSupplementalValueBlock);

            var interestRateSupplementalTitle = interestRateSupplemental.Cells.AddTableCell();
            var interestRateSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestRateSupplementalTitleBlock.InsertText("Interest Rate");
            interestRateSupplementalTitle.Blocks.Add(interestRateSupplementalTitleBlock);
            var interestRateSupplementalValue = interestRateSupplemental.Cells.AddTableCell();
            var interestRateSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestRateSupplementalValueBlock.InsertText(" - ");
            interestRateSupplementalValue.Blocks.Add(interestRateSupplementalValueBlock);

            var amortizationSupplementalTitle = amortizationSupplemental.Cells.AddTableCell();
            amortizationSupplementalTitle.Background = new RgbColor(248, 249, 250);
            var amortizationSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            amortizationSupplementalTitleBlock.InsertText("Amortization");
            amortizationSupplementalTitle.Blocks.Add(amortizationSupplementalTitleBlock);
            var amortizationSupplementalValue = amortizationSupplemental.Cells.AddTableCell();
            amortizationSupplementalValue.Background = new RgbColor(248, 249, 250);
            var amortizationSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            amortizationSupplementalValueBlock.InsertText(" - ");
            amortizationSupplementalValue.Blocks.Add(amortizationSupplementalValueBlock);

            var termSupplementalTitle = termSupplemental.Cells.AddTableCell();
            var termSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            termSupplementalTitleBlock.InsertText("Term");
            termSupplementalTitle.Blocks.Add(termSupplementalTitleBlock);
            var termSupplementalValue = termSupplemental.Cells.AddTableCell();
            var termSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            termSupplementalValueBlock.InsertText(" - ");
            termSupplementalValue.Blocks.Add(termSupplementalValueBlock);

            var debtServiceMonthSupplementalTitle = debtServiceMonthSupplemental.Cells.AddTableCell();
            debtServiceMonthSupplementalTitle.Background = new RgbColor(248, 249, 250);
            var debtServiceMonthSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceMonthSupplementalTitleBlock.InsertText("Debt Service / Month");
            debtServiceMonthSupplementalTitle.Blocks.Add(debtServiceMonthSupplementalTitleBlock);
            var debtServiceMonthSupplementalValue = debtServiceMonthSupplemental.Cells.AddTableCell();
            debtServiceMonthSupplementalValue.Background = new RgbColor(248, 249, 250);
            var debtServiceMonthSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceMonthSupplementalValueBlock.InsertText(" - ");
            debtServiceMonthSupplementalValue.Blocks.Add(debtServiceMonthSupplementalValueBlock);

            var debtServiceYearSupplementalTitle = debtServiceYearSupplemental.Cells.AddTableCell();
            var debtServiceYearSupplementalTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceYearSupplementalTitleBlock.InsertText("Debt Service / Year");
            debtServiceYearSupplementalTitle.Blocks.Add(debtServiceYearSupplementalTitleBlock);
            var debtServiceYearSupplementalValue = debtServiceYearSupplemental.Cells.AddTableCell();
            var debtServiceYearSupplementalValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceYearSupplementalValueBlock.InsertText(" - ");
            debtServiceYearSupplementalValue.Blocks.Add(debtServiceYearSupplementalValueBlock);

            editor.Position.Translate(primaryDebtAcqusitionCostsTable.Measure().Width + 60, equityAcqusitionCostsTable.Measure().Height + reversionTable.Measure().Height + 200);
            editor.DrawTable(supplementalDebtAcqusitionCostsTable, new Size(tableWidth, double.PositiveInfinity));

            // Refinance
            var refinanceTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                LayoutType = TableLayoutType.FixedWidth,
            };
            refinanceTable.Borders = new TableBorders(blackBorder);

            var refinanceTitle = refinanceTable.Rows.AddTableRow();
            var loanStartDate = refinanceTable.Rows.AddTableRow();
            var financedCapitalImprovementsRefinance = refinanceTable.Rows.AddTableRow();
            var loanCostsRefinance = refinanceTable.Rows.AddTableRow();
            var loanAmountRefinance = refinanceTable.Rows.AddTableRow();
            var totalLoanAmountRefinance = refinanceTable.Rows.AddTableRow();
            var loanToValueRefinance = refinanceTable.Rows.AddTableRow();
            var interestRateRefinance = refinanceTable.Rows.AddTableRow();
            var amortizationRefinance = refinanceTable.Rows.AddTableRow();
            var termRefinance = refinanceTable.Rows.AddTableRow();
            var debtServiceMonthRefinance = refinanceTable.Rows.AddTableRow();
            var debtServiceYearRefinance = refinanceTable.Rows.AddTableRow();

            var refinanceTitleHeader = refinanceTitle.Cells.AddTableCell();
            refinanceTitleHeader.Background = new RgbColor(137, 207, 240);
            var refinanceTitleHeaderBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            refinanceTitleHeaderBlock.InsertText("Refinance");
            refinanceTitleHeader.Blocks.Add(refinanceTitleHeaderBlock);
            var refinanceTitleCell = refinanceTitle.Cells.AddTableCell();
            refinanceTitleCell.Background = new RgbColor(137, 207, 240);
            var refinanceTitleCellBlock = new Block();
            refinanceTitleCell.Blocks.Add(refinanceTitleCellBlock);

            var loanStartDateTitle = loanStartDate.Cells.AddTableCell();
            var loanStartDateTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanStartDateTitleBlock.InsertText("Loan Start Date");
            loanStartDateTitle.Blocks.Add(loanStartDateTitleBlock);
            var loanStartDateValue = loanStartDate.Cells.AddTableCell();
            var loanStartDateValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanStartDateValueBlock.InsertText(" - ");
            loanStartDateValue.Blocks.Add(loanStartDateValueBlock);

            var financedCapitalImprovementsRefinanceTitle = financedCapitalImprovementsRefinance.Cells.AddTableCell();
            financedCapitalImprovementsRefinanceTitle.Background = new RgbColor(248, 249, 250);
            var financedCapitalImprovementsRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            financedCapitalImprovementsRefinanceTitleBlock.InsertText("Capital Improvements (Financed)");
            financedCapitalImprovementsRefinanceTitle.Blocks.Add(financedCapitalImprovementsRefinanceTitleBlock);
            var financedCapitalImprovementsRefinanceValue = financedCapitalImprovementsRefinance.Cells.AddTableCell();
            financedCapitalImprovementsRefinanceValue.Background = new RgbColor(248, 249, 250);
            var financedCapitalImprovementsRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            financedCapitalImprovementsRefinanceValueBlock.InsertText(" - ");
            financedCapitalImprovementsRefinanceValue.Blocks.Add(financedCapitalImprovementsRefinanceValueBlock);

            var loanCostsRefinanceTitle = loanCostsRefinance.Cells.AddTableCell();
            var loanCostsRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanCostsRefinanceTitleBlock.InsertText("Loan Costs (Financed)");
            loanCostsRefinanceTitle.Blocks.Add(loanCostsRefinanceTitleBlock);
            var loanCostsRefinanceValue = loanCostsRefinance.Cells.AddTableCell();
            var loanCostsRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanCostsRefinanceValueBlock.InsertText(" - ");
            loanCostsRefinanceValue.Blocks.Add(loanCostsRefinanceValueBlock);

            var loanAmountRefinanceTitle = loanAmountRefinance.Cells.AddTableCell();
            loanAmountRefinanceTitle.Background = new RgbColor(248, 249, 250);
            var loanAmountRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanAmountRefinanceTitleBlock.InsertText("Loan Amount");
            loanAmountRefinanceTitle.Blocks.Add(loanAmountRefinanceTitleBlock);
            var loanAmountRefinanceValue = loanAmountRefinance.Cells.AddTableCell();
            loanAmountRefinanceValue.Background = new RgbColor(248, 249, 250);
            var loanAmountRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanAmountRefinanceValueBlock.InsertText(" - ");
            loanAmountRefinanceValue.Blocks.Add(loanAmountRefinanceValueBlock);

            var totalLoanAmountRefinanceTitle = totalLoanAmountRefinance.Cells.AddTableCell();
            var totalLoanAmountRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalLoanAmountRefinanceTitleBlock.InsertText("Total Loan Amount");
            totalLoanAmountRefinanceTitle.Blocks.Add(totalLoanAmountRefinanceTitleBlock);
            var totalLoanAmountRefinanceValue = totalLoanAmountRefinance.Cells.AddTableCell();
            var totalLoanAmountRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            totalLoanAmountRefinanceValueBlock.InsertText(" - ");
            totalLoanAmountRefinanceValue.Blocks.Add(totalLoanAmountRefinanceValueBlock);

            var loanToValueRefinanceTitle = loanToValueRefinance.Cells.AddTableCell();
            loanToValueRefinanceTitle.Background = new RgbColor(248, 249, 250);
            var loanToValueRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanToValueRefinanceTitleBlock.InsertText("Loan to Value");
            loanToValueRefinanceTitle.Blocks.Add(loanToValueRefinanceTitleBlock);
            var loanToValueRefinanceValue = loanToValueRefinance.Cells.AddTableCell();
            loanToValueRefinanceValue.Background = new RgbColor(248, 249, 250);
            var loanToValueRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            loanToValueRefinanceValueBlock.InsertText(" - ");
            loanToValueRefinanceValue.Blocks.Add(loanToValueRefinanceValueBlock);

            var interestRateRefinanceTitle = interestRateRefinance.Cells.AddTableCell();
            var interestRateRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestRateRefinanceTitleBlock.InsertText("Interest Rate");
            interestRateRefinanceTitle.Blocks.Add(interestRateRefinanceTitleBlock);
            var interestRateRefinanceValue = interestRateRefinance.Cells.AddTableCell();
            var interestRateRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            interestRateRefinanceValueBlock.InsertText(" - ");
            interestRateRefinanceValue.Blocks.Add(interestRateRefinanceValueBlock);

            var amortizationRefinanceTitle = amortizationRefinance.Cells.AddTableCell();
            amortizationRefinanceTitle.Background = new RgbColor(248, 249, 250);
            var amortizationRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            amortizationRefinanceTitleBlock.InsertText("Amortization");
            amortizationRefinanceTitle.Blocks.Add(amortizationRefinanceTitleBlock);
            var amortizationRefinanceValue = amortizationRefinance.Cells.AddTableCell();
            amortizationRefinanceValue.Background = new RgbColor(248, 249, 250);
            var amortizationRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            amortizationRefinanceValueBlock.InsertText(" - ");
            amortizationRefinanceValue.Blocks.Add(amortizationRefinanceValueBlock);

            var termRefinanceTitle = termRefinance.Cells.AddTableCell();
            var termRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            termRefinanceTitleBlock.InsertText("Term");
            termRefinanceTitle.Blocks.Add(termRefinanceTitleBlock);
            var termRefinanceValue = termRefinance.Cells.AddTableCell();
            var termRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            termRefinanceValueBlock.InsertText(" -");
            termRefinanceValue.Blocks.Add(termRefinanceValueBlock);

            var debtServiceMonthlyRefinanceTitle = debtServiceMonthRefinance.Cells.AddTableCell();
            debtServiceMonthlyRefinanceTitle.Background = new RgbColor(248, 249, 250);
            var debtServiceMonthlyRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceMonthlyRefinanceTitleBlock.InsertText("Debt Service / Month");
            debtServiceMonthlyRefinanceTitle.Blocks.Add(debtServiceMonthlyRefinanceTitleBlock);
            var debtServiceMonthlyRefinanceValue = debtServiceMonthRefinance.Cells.AddTableCell();
            debtServiceMonthlyRefinanceValue.Background = new RgbColor(248, 249, 250);
            var debtServiceMonthlyRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceMonthlyRefinanceValueBlock.InsertText(" - ");
            debtServiceMonthlyRefinanceValue.Blocks.Add(debtServiceMonthlyRefinanceValueBlock);

            var debtServiceYearRefinanceTitle = debtServiceYearRefinance.Cells.AddTableCell();
            var debtServiceYearRefinanceTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceYearRefinanceTitleBlock.InsertText("Debt Service / Year");
            debtServiceYearRefinanceTitle.Blocks.Add(debtServiceYearRefinanceTitleBlock);
            var debtServiceYearRefinanceValue = debtServiceYearRefinance.Cells.AddTableCell();
            var debtServiceYearRefinanceValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            debtServiceYearRefinanceValueBlock.InsertText(" - ");
            debtServiceYearRefinanceValue.Blocks.Add(debtServiceYearRefinanceValueBlock);

            editor.Position.Translate(primaryDebtAcqusitionCostsTable.Measure().Width + 160 + supplementalDebtAcqusitionCostsTable.Measure().Width, equityAcqusitionCostsTable.Measure().Height + reversionTable.Measure().Height + 200);
            editor.DrawTable(refinanceTable, new Size(tableWidth, double.PositiveInfinity));

            // Capital Improvements Breakdown
            var capitalImprovementsBreakdownTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                LayoutType = TableLayoutType.FixedWidth,
            };
            capitalImprovementsBreakdownTable.Borders = new TableBorders(blackBorder);

            var capitalImprovementsBreakdownTitle = capitalImprovementsBreakdownTable.Rows.AddTableRow();
            var capitalImprovementsBreakdownHeaders = capitalImprovementsBreakdownTable.Rows.AddTableRow();

            var capitalImprovementsBreakdownTitleCell = capitalImprovementsBreakdownTitle.Cells.AddTableCell();
            capitalImprovementsBreakdownTitleCell.Background = new RgbColor(137, 207, 240);
            var capitalImprovementsBreakdownTitleBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            capitalImprovementsBreakdownTitleBlock.InsertText("Capital Improvements Breakdown");
            capitalImprovementsBreakdownTitleCell.Blocks.Add(capitalImprovementsBreakdownTitleBlock);
            var capitalImprovementsBreakdownHeader = capitalImprovementsBreakdownTitle.Cells.AddTableCell();
            capitalImprovementsBreakdownHeader.Background = new RgbColor(137, 207, 240);
            var capitalImprovementsBreakdownHeaderBlock = new Block();
            capitalImprovementsBreakdownHeader.Blocks.Add(capitalImprovementsBreakdownHeaderBlock);

            var capitalImprovementsBreakdownHeaderCost = capitalImprovementsBreakdownHeaders.Cells.AddTableCell();
            var capitalImprovementsBreakdownHeaderCostBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            capitalImprovementsBreakdownHeaderCostBlock.InsertText("Cost");
            capitalImprovementsBreakdownHeaderCost.Blocks.Add(capitalImprovementsBreakdownHeaderCostBlock);
            var capitalImprovementsBreakdownHeaderDiscription = capitalImprovementsBreakdownHeaders.Cells.AddTableCell();
            var capitalImprovementsBreakdownHeaderDiscriptionBlock = new Block { TextProperties = { Font = FontsRepository.HelveticaBold } };
            capitalImprovementsBreakdownHeaderDiscriptionBlock.InsertText("Description");
            capitalImprovementsBreakdownHeaderDiscription.Blocks.Add(capitalImprovementsBreakdownHeaderDiscriptionBlock);

            var targetindex = 0;
            foreach (var capitalImprovement in asr.CapitalImprovementsBreakDown)
            {
                if (capitalImprovement.Value == 0)
                    continue;

                var dynamicRow = capitalImprovementsBreakdownTable.Rows.AddTableRow();
                var dynamicCell = dynamicRow.Cells.AddTableCell();
                if (targetindex % 2 == 0)
                    dynamicCell.Background = new RgbColor(248, 249, 250);
                var dynamicBlock = new Block();
                dynamicBlock.InsertText(capitalImprovement.Value.ToString("C2"));
                dynamicCell.Blocks.Add(dynamicBlock);
                var dynamicCell2 = dynamicRow.Cells.AddTableCell();
                if (targetindex % 2 == 0)
                    dynamicCell2.Background = new RgbColor(248, 249, 250);
                var dynamicBlock2 = new Block();
                dynamicBlock2.InsertText(capitalImprovement.Key);
                dynamicCell2.Blocks.Add(dynamicBlock2);
                targetindex++;
            }

            editor.Position.Translate(20, equityAcqusitionCostsTable.Measure().Height + reversionTable.Measure().Height + refinanceTable.Measure().Height + 300);
            editor.DrawTable(capitalImprovementsBreakdownTable, new Size(page.Size.Width - 40, double.PositiveInfinity));

            // conclusion
            var dateBox = page.Content.AddTextFragment();
            dateBox.Text = $"{property.Name} - {DateTime.Now:MM/dd/yyyy}";
            dateBox.Position.Translate(page.Size.Width - 250, page.Size.Height - 10);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    private void GenerateCashFlow(UnderwritingAnalysis property, RadFixedDocument document)
    {
        try
        {
            var cfr = property.Projections;
            if (cfr.Select(x => x.Year).Count() != property.HoldYears + 1)
            {
                _logger.LogCritical($"Cash Flow Report: Corrupt data for property {property.Name} : {property.Id} on {DateTime.UtcNow}");
            }

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
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }

    private void GenerateIncomeForecast(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }

    private void GenerateCapitalExpenses(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }

    private void GenerateRentRoll(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }

    private void GenerateLeaseExposure(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }
    #endregion Underwriting Report Generators

    private static ReportResponse NotFound() => new() { Data = Array.Empty<byte>() };
}
