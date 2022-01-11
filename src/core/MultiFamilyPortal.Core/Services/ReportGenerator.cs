using Microsoft.Extensions.Logging;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.Pdf;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.Utilities;

namespace MultiFamilyPortal.Services;

public class ReportGenerator : IReport
{
    private ILogger<ReportGenerator> _logger { get; }

    public ReportGenerator(ILogger<ReportGenerator> logger) => _logger = logger;

    public Workbook OverallProjections()
    {
        throw new NotImplementedException();
    }

    public Workbook CashFlow()
    {
        throw new NotImplementedException();
    }

    public Workbook ManagersReturns(ManagersReturnsReport mmr)
    {
        var workbook = new Workbook();

        try
        {
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

        return workbook;
    }

    public Workbook ThreeTier()
    {
        throw new NotImplementedException();
    }

    public Workbook CulmativeInvestment()
    {
        throw new NotImplementedException();
    }

    public Workbook AOneandAtwo()
    {
        throw new NotImplementedException();
    }

    public Workbook ThousandInvestmentProjects()
    {
        throw new NotImplementedException();
    }

    public Workbook NetPresentValue()
    {
        throw new NotImplementedException();
    }

    public Workbook LeveragedRateOfReturns()
    {
        throw new NotImplementedException();
    }

    public bool ExportToPDF(Workbook workbook, string name, string folder)
    {
        var path = Path.Combine(folder, $"{name}.pdf");

        // TODO : Do export settings here
        var pdfFormatProvider = new PdfFormatProvider();

        try
        {
            using Stream output = File.OpenWrite(path);
            pdfFormatProvider.Export(workbook, output);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.ToString());
            return false;
        }

        return true;
    }

    public bool ExportToSpreadsheet(Workbook workbook, string name, string folder)
    {
        var path = Path.Combine(folder, $"{name}.xlsx");
        IWorkbookFormatProvider formatProvider = new XlsxFormatProvider();

        try
        {
            using Stream output = new FileStream(path, FileMode.Create);
            formatProvider.Export(workbook, output);
        }
        catch (Exception e)
        {
            _logger.LogWarning(e.ToString());
            return false;
        }

        return true;
    }


    private static char ToAlphabet(int number)
    {
        if (number is < 0 or > 26)
            throw new ArgumentOutOfRangeException(nameof(number));

        return number switch
        {
            0 => 'A',
            1 => 'B',
            2 => 'C',
            3 => 'D',
            4 => 'E',
            5 => 'F',
            6 => 'G',
            7 => 'H',
            8 => 'I',
            9 => 'J',
            10 => 'K',
            11 => 'L',
            12 => 'M',
            13 => 'N',
            14 => 'O',
            15 => 'P',
            16 => 'Q',
            17 => 'R',
            18 => 'S',
            19 => 'T',
            20 => 'U',
            21 => 'V',
            22 => 'W',
            23 => 'X',
            24 => 'Y',
            _ => 'Z'
        };
    }
}