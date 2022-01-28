using MultiFamilyPortal.Dtos.Underwriting.Reports;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;

namespace MultiFamilyPortal.Helpers.Reports;

public static class GenerateManagerReturnsBuilder
{
    public static void GenerateManagerReturns(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var page = document.Pages.AddPage();

        var mmr = new ManagersReturnsReport(property);
        page.Size = ReportBuilder.LetterSizeHorizontal;
        var padding = mmr.HoldYears > 5 ? 10 : 15.5;
        FixedContentEditor editor = new(page);

        ReportBuilder.Header(page, "Manager Returns");

        var table = new Table { DefaultCellProperties = { Padding = new Thickness(padding) } };
        var blackBorder = new Border(1, ReportBuilder.DarkColor);
        table.Borders = new TableBorders(blackBorder);

        // row 0
        var r0 = table.Rows.AddTableRow();
        r0.BasicCell("", false, ReportBuilder.HeaderColor, HorizontalAlignment.Center, 2);
        foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            r0.BasicCell($"Year {year}", true, ReportBuilder.HeaderColor);
        r0.BasicCell($"Total", true, ReportBuilder.HeaderColor);

        var r1 = table.Rows.AddTableRow();
        r1.BasicCell("Acquisation Fee");
        r1.BasicCell(mmr.AcquisitionFee.ToString("C2"), false, ReportBuilder.PrimaryColor);
        foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            r1.BasicCell("", false, ReportBuilder.PrimaryColor);
        r1.BasicCell(mmr.AcquisitionFee.ToString("C2"), false, ReportBuilder.PrimaryColor);

        var r2 = table.Rows.AddTableRow();
        r2.BasicCell("Manager Equity");
        r2.BasicCell(mmr.ManagerEquity.ToString("C2"));
        foreach (var year in Enumerable.Range(1, mmr.HoldYears + 1))
            r2.BasicCell("");

        var r3 = table.Rows.AddTableRow();
        r3.BasicCell("Total Cash Flow");
        r3.BasicCell(mmr.ManagerEquity.ToString("C2"), false, ReportBuilder.PrimaryColor);
        foreach (var yearlycashFlow in mmr.CashFlow)
            r3.BasicCell(yearlycashFlow.ToString("C2"), false, ReportBuilder.PrimaryColor);

        var r4 = table.Rows.AddTableRow();
        r4.BasicCell($"Cash Flow ({mmr.CashFlowPercentage.ToString("P2")})");
        var totalMCF = 0d;
        foreach (var yearlycashFlow in mmr.CashFlow)
        {
            r4.BasicCell((yearlycashFlow * mmr.CashFlowPercentage).ToString("C2"));
            totalMCF += (yearlycashFlow * mmr.CashFlowPercentage);
        }
        r4.BasicCell(totalMCF.ToString("C2"));

        var r5 = table.Rows.AddTableRow();
        r5.BasicCell("Equity On Sale of Property");
        foreach (var year in Enumerable.Range(0, mmr.HoldYears))
            r5.BasicCell("", false, ReportBuilder.PrimaryColor);
        r5.BasicCell(mmr.EqualityOnSaleOfProperty.ToString("C2"), false, ReportBuilder.PrimaryColor);
        r5.BasicCell(mmr.EqualityOnSaleOfProperty.ToString("C2"), false, ReportBuilder.PrimaryColor);

        var r6 = table.Rows.AddTableRow();
        r6.BasicCell("Total", true);
        r6.BasicCell((mmr.AcquisitionFee + mmr.CashFlow.FirstOrDefault() * mmr.CashFlowPercentage).ToString("C2"), true);
        int i = 0;
        foreach (var year in mmr.CashFlow)
        {
            if (i != 0 && i != mmr.HoldYears)
                r6.BasicCell((year * mmr.CashFlowPercentage).ToString("C2"), true);
            else if (i == mmr.HoldYears)
                r6.BasicCell((mmr.EqualityOnSaleOfProperty + year * mmr.CashFlowPercentage).ToString("C2"), true);

            i++;
        }
        r6.BasicCell((totalMCF + mmr.EqualityOnSaleOfProperty).ToString("C2"), true);

        editor.Position.Translate(ReportBuilder.PageMargin+2, page.Size.Height / 2 - table.Measure().Height / 2);
        editor.DrawTable(table, new Size(page.Size.Width - ReportBuilder.PageMargin * 2-4, double.PositiveInfinity));

        ReportBuilder.Footer(page, property.Name);
    }
}
