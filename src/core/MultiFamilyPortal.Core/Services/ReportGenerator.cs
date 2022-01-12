using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf.Export;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Data;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;

namespace MultiFamilyPortal.Services;

public class ReportGenerator : IReport
{
    private ILogger<ReportGenerator> _logger { get; }

    public ReportGenerator(ILogger<ReportGenerator> logger) => _logger = logger;

    public RadFixedDocument OverallProjections()
    {
        throw new NotImplementedException();
    }

    public RadFixedDocument CashFlow()
    {
        throw new NotImplementedException();
    }

    public RadFixedDocument ManagersReturns(ManagersReturnsReport mmr)
    {
        RadFixedDocument document = new RadFixedDocument();
        RadFixedPage page = document.Pages.AddPage();
        page.Size = new Size(1823, 793);

        FixedContentEditor editor = new FixedContentEditor(page);

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
            //cell.SetHorizontalAlignment(RadHorizontalAlignment.Center);
        }
        var totalhcell = r0.Cells.AddTableCell();
        totalhcell.Blocks.AddBlock().InsertText("Total");
        
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
        foreach (var year in Enumerable.Range(1, mmr.HoldYears+1))
        {
            var cell = r2.Cells.AddTableCell();
            cell.Blocks.AddBlock();
        }
        
        // row 3
        var cfcell = r3.Cells.AddTableCell();
        cfcell.Blocks.AddBlock().InsertText($"Cash Flow ({mmr.CashFlowPercentage.ToString("P2")})");
        var cfValuecell = r3.Cells.AddTableCell();
        cfValuecell.Blocks.AddBlock().InsertText((mmr.CashFlowPercentage*100).ToString("N2"));
        var emptyr3cell = r3.Cells.AddTableCell();
        emptyr3cell.Blocks.AddBlock();
        foreach (var year in Enumerable.Range(1, mmr.HoldYears))
        {
            var cell = r3.Cells.AddTableCell();
            cell.Blocks.AddBlock().InsertText((mmr.ManagerEquity*mmr.CashFlowPercentage).ToString("N2"));
        }
        var totalcfcell = r3.Cells.AddTableCell();
        totalcfcell.Blocks.AddBlock().InsertText((mmr.ManagerEquity*mmr.CashFlowPercentage*mmr.HoldYears).ToString("N2"));
       
        // row 4
        var epcell = r4.Cells.AddTableCell();
        epcell.Blocks.AddBlock().InsertText("Equity On Sale of Property");
        foreach (var year in Enumerable.Range(1, mmr.HoldYears+1))
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
        totalcell.Blocks.AddBlock().InsertText("Total");
        var emptyr5cell = r5.Cells.AddTableCell();
        emptyr5cell.Blocks.AddBlock();
        var totalValuecell = r5.Cells.AddTableCell();
        totalValuecell.Blocks.AddBlock().InsertText(mmr.AcquisitionFee.ToString("N2"));
        foreach (var year in Enumerable.Range(1, mmr.HoldYears-1))
        {
            var cell = r5.Cells.AddTableCell();
            cell.Blocks.AddBlock().InsertText((mmr.ManagerEquity*mmr.CashFlowPercentage).ToString("N2"));
        }
        var totallastcell = r5.Cells.AddTableCell();
        totallastcell.Blocks.AddBlock().InsertText((mmr.EqualityOnSaleOfProperty + (mmr.ManagerEquity*mmr.CashFlowPercentage)).ToString("N2"));
        var totaltotalcell = r5.Cells.AddTableCell();
        totaltotalcell.Blocks.AddBlock().InsertText(((mmr.ManagerEquity*mmr.CashFlowPercentage*mmr.HoldYears)+mmr.EqualityOnSaleOfProperty).ToString("N2"));



        editor.Position.Translate(10, 100);
        editor.DrawTable(table);



        /*
                try
                {
                    /*
                    var worksheet = workbook.Worksheets.Add();
                    worksheet.Name = "Manager Reports";

                    // Header
                    var b2 = new CellIndex(1, 1);
                    var bn = new CellIndex(1, mmr.HoldYears + 4);

                    worksheet.Cells[b2, bn].Merge();
                    var title = worksheet.Cells[1, 6];
                    title.SetValue("Manager Returns Report");
                    title.SetFontSize(20);
                    title.SetIsBold(true);


                    var c3 = worksheet.Cells[2, 2];
                    c3.SetValueAsText("%");
                    c3.SetHorizontalAlignment(RadHorizontalAlignment.Center);

                    foreach (var year in Enumerable.Range(1, mmr.HoldYears))
                    {
                        var cell = worksheet.Cells[2, 3 + year];
                        cell.SetValueAsText($"Year {year}");
                        cell.SetHorizontalAlignment(RadHorizontalAlignment.Center);
                    }

                    var n3 = worksheet.Cells[2, 14];
                    n3.SetValueAsText("Total");

                    // Side Names
                    var a4 = worksheet.Cells[3, 1];
                    a4.SetValueAsText("Acquisition Fee");
                    var a5 = worksheet.Cells[4, 1];
                    a5.SetValueAsText("Manager Equality");
                    var a6 = worksheet.Cells[5, 1];
                    a6.SetValueAsText($"Cash Flow ({mmr.CashFlowPercentage:P2})");
                    var a7 = worksheet.Cells[6, 1];
                    a7.SetValueAsText("Equality on Sale of Property");
                    var a8 = worksheet.Cells[7, 1];
                    a8.SetValueAsText("Total");
                    a8.SetIsBold(true);

                    // Numbers
                    var c6 = worksheet.Cells[5, 2];
                    c6.SetValue(mmr.CashFlowPercentage);
                    // c6.SetFormat(new CellValueFormat("P0"));

                    var d4 = worksheet.Cells[3, 3];
                    d4.SetValue(mmr.AcquisitionFee);

                    var o4 = worksheet.Cells[3, 14];
                    o4.SetValueAsFormula("=D4");

                    var d5 = worksheet.Cells[4, 3];
                    d5.SetValue(mmr.ManagerEquity);

                    foreach (var year in Enumerable.Range(1, mmr.HoldYears))
                    {
                        var cell = worksheet.Cells[5, 3 + year];
                        cell.SetValueAsFormula("=(C6*D5)");
                        // cell.SetFormat(new CellValueFormat("N2"));
                        var col = worksheet.Columns[3 + year];
                        col.SetWidth(new ColumnWidth(UnitHelper.ExcelColumnWidthToPixelWidth(workbook, 15.0), true));
                    }

                    var o6 = worksheet.Cells[5, 14];
                    o6.SetValueAsFormula("=SUM(E6:N6)");

                    var n7 = worksheet.Cells[6, 13];
                    n7.SetValue(mmr.EqualityOnSaleOfProperty);

                    var o7 = worksheet.Cells[6, 14];
                    o7.SetValueAsFormula("=SUM(E7:N7)");

                    foreach (var year in Enumerable.Range(1, mmr.HoldYears + 1))
                    {
                        var cell = worksheet.Cells[7, 3 + year];
                        var letter = ToAlphabet(3 + year);
                        cell.SetValueAsFormula($"=SUM({letter}4:{letter}7)");
                        // cell.SetFormat(new CellValueFormat("N2"));
                    }

                    var d8 = worksheet.Cells[7, 3];
                    d8.SetValueAsFormula("=D4");
                    //d8.SetFormat(new CellValueFormat("N2"));

                    // Additional styles
                    var b1 = worksheet.Columns[1];
                    b1.SetWidth(new ColumnWidth(UnitHelper.ExcelColumnWidthToPixelWidth(workbook, 24.50), true));

                    foreach (var number in Enumerable.Range(1, 6))
                    {
                        var row = worksheet.Rows[2 + number];
                        row.SetHeight(new RowHeight(UnitHelper.PointToDip(30.50), true));
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                }
                */

        return document;
    }

    public RadFixedDocument ThreeTier()
    {
        throw new NotImplementedException();
    }

    public RadFixedDocument CulmativeInvestment()
    {
        throw new NotImplementedException();
    }

    public RadFixedDocument AOneandAtwo()
    {
        throw new NotImplementedException();
    }

    public RadFixedDocument ThousandInvestmentProjects()
    {
        throw new NotImplementedException();
    }

    public RadFixedDocument NetPresentValue()
    {
        throw new NotImplementedException();
    }

    public RadFixedDocument LeveragedRateOfReturns()
    {
        throw new NotImplementedException();
    }

    public byte[] ExportToPDF(RadFixedDocument document)
    {
        PdfFormatProvider provider = new PdfFormatProvider();
        PdfExportSettings settings = new PdfExportSettings();
        settings.ImageQuality = ImageQuality.High;
        settings.ComplianceLevel = PdfComplianceLevel.PdfA2B;
        provider.ExportSettings = settings;

        return provider.Export(document);
    }
}