using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
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
        GenerateDealSummary(property, document);
        GenerateAssumptions(property, document);
        document = GenerateCashFlow(property, document);
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
        GenerateDealSummary(property, document);

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

        var document = GenerateCashFlow(property, new RadFixedDocument());

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
    private void GenerateDealSummary(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }

    private void GenerateAssumptions(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }

    private RadFixedDocument GenerateCashFlow(UnderwritingAnalysis property, RadFixedDocument document)
    {
        try
        {
            var cfr = property.Projections;
            if (cfr.Select(x => x.Year).Count() != property.HoldYears + 1)
            {
                _logger.LogCritical($"Cash Flow Report: Corrupt data for property {property.Name} : {property.Id} on {DateTime.UtcNow}");
            }

            var dynamicWidth = 600 + 100 * property.HoldYears;
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
                Borders = new TableBorders(blackBorder)
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

            editor.Position.Translate(pageOne.Size.Width / 2 - expensesTable.Measure().Width / 2, incomeTable.Measure().Height + 210);
            editor.DrawTable(expensesTable);

            // Net Income
            var netTableTitle = pageOne.Content.AddTextFragment();
            netTableTitle.FontSize = headerSize;
            netTableTitle.Text = "Net";
            netTableTitle.Position.Translate(pageOne.Size.Width / 2 - 5, incomeTable.Measure().Height + expensesTable.Measure().Height + 300);

            var netTable = new Table
            {
                DefaultCellProperties = { Padding = new Thickness(cellPadding) },
                Borders = new TableBorders(blackBorder)
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
            foreach (var year in cfr.Select( x => x.Year))
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

            editor.Position.Translate(pageOne.Size.Width / 2 - netTable.Measure().Width / 2, incomeTable.Measure().Height + expensesTable.Measure().Height + 310);
            editor.DrawTable(netTable);

            // conclusion
            var dateBox = pageOne.Content.AddTextFragment();
            dateBox.Text = $"{property.Name} - {DateTime.Now:MM/dd/yyyy}";
            dateBox.Position.Translate(pageOne.Size.Width - 250, pageOne.Size.Height - 10);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        return document;
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
