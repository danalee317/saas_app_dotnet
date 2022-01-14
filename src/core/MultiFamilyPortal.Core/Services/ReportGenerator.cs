using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;

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

    public async Task<ReportResponse> ManagersReturns(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);

        if (property is null)
            return await NotFound();

        var name = $"Managers_Returns_Report.pdf";

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
            _logger.LogError(ex.ToString());
        }

        return new ReportResponse()
        {
            FileName = name,
            Data = ExportToPdf(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    private byte[] ExportToPdf(RadFixedDocument document)
    {
        try
        {
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
            return null;
        }
    }

    public Task<ReportResponse> OverallProjections(Guid propertyId) => NotFound();
    public Task<ReportResponse> CashFlow(Guid propertyId)
    {
        return NotFound();
    }

    public Task<ReportResponse> ThreeTier(Guid propertyId) => NotFound();
    public Task<ReportResponse> CulmativeInvestment(Guid propertyId) => NotFound();
    public Task<ReportResponse> AOneandAtwo(Guid propertyId) => NotFound();
    public Task<ReportResponse> ThousandInvestmentProjects(Guid propertyId) => NotFound();
    public Task<ReportResponse> NetPresentValue(Guid propertyId) => NotFound();
    public Task<ReportResponse> LeveragedRateOfReturns(Guid propertyId) => NotFound();
    private static async Task<ReportResponse> NotFound() => new() { Data = Array.Empty<byte>() };
}
