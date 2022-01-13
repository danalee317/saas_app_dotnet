using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Dtos;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;

namespace MultiFamilyPortal.Services;

public class ReportGenerator : IReport
{
    private ILogger<ReportGenerator> _logger { get; }
    private IUnderwritingService _underwritingService { get; }

    private IReport _report { get; }

    public ReportGenerator(ILogger<ReportGenerator> logger, IUnderwritingService underwritingService)
    {
        _logger = logger;
        _underwritingService = underwritingService;
    }

    public async Task<ReportResponse> ManagersReturns(Guid propertyId)
    {
        var property = await _underwritingService.GetUnderwritingAnalysis(propertyId);
        var name = $"Managers_Returns_Report.pdf";

        RadFixedDocument document = new RadFixedDocument();
        RadFixedPage page = document.Pages.AddPage();

        try
        {
            var mmr = new ManagersReturnsReport(property);
            page.Size = new Size((570 + 135 * mmr.HoldYears), 793);
            FixedContentEditor editor = new FixedContentEditor(page);

            var textFragment = page.Content.AddTextFragment();
            textFragment.Text = "Manager Return";
            textFragment.Position.Translate(page.Size.Width / 2 - 100, 100);
            textFragment.FontSize = 25;

            var table = new Table();
            table.DefaultCellProperties.Padding = new Thickness(23);
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
            var pecentagecell = r0.Cells.AddTableCell();
            pecentagecell.Blocks.AddBlock().InsertText("%");
            var empty2cell = r0.Cells.AddTableCell();
            empty2cell.Blocks.AddBlock();
            foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            {
                var cell = r0.Cells.AddTableCell();
                cell.Blocks.AddBlock().InsertText($"Year {year}");
            }
            var totalhcell = r0.Cells.AddTableCell();
            totalhcell.Blocks.AddBlock().InsertText("Total ($)");

            // row 1
            var afcell = r1.Cells.AddTableCell();
            afcell.Blocks.AddBlock().InsertText("Acquisation Fee");
            var emptyr1cell = r1.Cells.AddTableCell();
            emptyr1cell.Blocks.AddBlock();
            var afValuecell = r1.Cells.AddTableCell();
            afValuecell.Blocks.AddBlock().InsertText(mmr.AcquisitionFee.ToString("N2"));
            foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            {
                var cell = r1.Cells.AddTableCell();
                cell.Blocks.AddBlock();
            }
            var totalvcell = r1.Cells.AddTableCell();
            totalvcell.Blocks.AddBlock().InsertText(mmr.AcquisitionFee.ToString("N2"));

            // row 2
            var mecell = r2.Cells.AddTableCell();
            mecell.Blocks.AddBlock().InsertText("Manager Equity");
            var emptyr2cell = r2.Cells.AddTableCell();
            emptyr2cell.Blocks.AddBlock();
            var meValuecell = r2.Cells.AddTableCell();
            meValuecell.Blocks.AddBlock().InsertText(mmr.ManagerEquity.ToString("N2"));
            foreach (var year in Enumerable.Range(1, mmr.HoldYears + 1))
            {
                var cell = r2.Cells.AddTableCell();
                cell.Blocks.AddBlock();
            }

            // row 3
            var cfcell = r3.Cells.AddTableCell();
            cfcell.Blocks.AddBlock().InsertText($"Cash Flow ({mmr.CashFlowPercentage.ToString("P2")})");
            var cfValuecell = r3.Cells.AddTableCell();
            cfValuecell.Blocks.AddBlock().InsertText((mmr.CashFlowPercentage * 100).ToString("N2"));
            var emptyr3cell = r3.Cells.AddTableCell();
            emptyr3cell.Blocks.AddBlock();
            foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            {
                var cell = r3.Cells.AddTableCell();
                cell.Blocks.AddBlock().InsertText((mmr.ManagerEquity * mmr.CashFlowPercentage).ToString("N2"));
            }
            var totalcfcell = r3.Cells.AddTableCell();
            totalcfcell.Blocks.AddBlock().InsertText((mmr.ManagerEquity * mmr.CashFlowPercentage * mmr.HoldYears).ToString("N2"));

            // row 4
            var epcell = r4.Cells.AddTableCell();
            epcell.Blocks.AddBlock().InsertText("Equity On Sale of Property");
            foreach (var year in Enumerable.Range(1, mmr.HoldYears + 1))
            {
                var cell = r4.Cells.AddTableCell();
                cell.Blocks.AddBlock();
            }
            var totallstcell = r4.Cells.AddTableCell();
            totallstcell.Blocks.AddBlock().InsertText(mmr.EqualityOnSaleOfProperty.ToString("N2"));
            var totalepcell = r4.Cells.AddTableCell();
            totalepcell.Blocks.AddBlock().InsertText(mmr.EqualityOnSaleOfProperty.ToString("N2"));

            // row 5
            var totalcell = r5.Cells.AddTableCell();
            totalcell.Blocks.AddBlock().InsertText("Total ($)");
            var emptyr5cell = r5.Cells.AddTableCell();
            emptyr5cell.Blocks.AddBlock();
            var totalValuecell = r5.Cells.AddTableCell();
            totalValuecell.Blocks.AddBlock().InsertText(mmr.AcquisitionFee.ToString("N2"));
            foreach (var year in Enumerable.Range(1, mmr.HoldYears - 1))
            {
                var cell = r5.Cells.AddTableCell();
                cell.Blocks.AddBlock().InsertText((mmr.ManagerEquity * mmr.CashFlowPercentage).ToString("N2"));
            }
            var totallastcell = r5.Cells.AddTableCell();
            totallastcell.Blocks.AddBlock().InsertText((mmr.EqualityOnSaleOfProperty + (mmr.ManagerEquity * mmr.CashFlowPercentage)).ToString("N2"));
            var totaltotalcell = r5.Cells.AddTableCell();
            totaltotalcell.Blocks.AddBlock().InsertText(((mmr.ManagerEquity * mmr.CashFlowPercentage * mmr.HoldYears) + mmr.EqualityOnSaleOfProperty).ToString("N2"));

            var dateBox = page.Content.AddTextFragment();
            dateBox.Text = $"{property.Name} - {DateTime.Now.ToString("MM/dd/yyyy")}";
            dateBox.Position.Translate(page.Size.Width - 250, page.Size.Height - 10);

            editor.Position.Translate(page.Size.Width/2 - table.Measure().Width/2, page.Size.Height/2 - table.Measure().Height/2);
            editor.DrawTable(table);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }

        return new ReportResponse()
        {
            FileName = name,
            Data = ExportToPDF(document),
            MimeType = FileTypeLookup.GetFileTypeInfo(name).MimeType
        };
    }

    private byte[] ExportToPDF(RadFixedDocument document)
    {
        try
        {
            PdfFormatProvider provider = new PdfFormatProvider();
            PdfExportSettings settings = new PdfExportSettings();
            settings.ImageQuality = ImageQuality.High;
            settings.ComplianceLevel = PdfComplianceLevel.PdfA2B;
            provider.ExportSettings = settings;
            return provider.Export(document);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return null;
        }
    }

    public ReportResponse OverallProjections() => throw new NotImplementedException();
    public ReportResponse CashFlow() => throw new NotImplementedException();
    public ReportResponse ThreeTier() => throw new NotImplementedException();
    public ReportResponse CulmativeInvestment() => throw new NotImplementedException();
    public ReportResponse AOneandAtwo() => throw new NotImplementedException();
    public ReportResponse ThousandInvestmentProjects() => throw new NotImplementedException();
    public ReportResponse NetPresentValue() => throw new NotImplementedException();
    public ReportResponse LeveragedRateOfReturns() => throw new NotImplementedException();
}