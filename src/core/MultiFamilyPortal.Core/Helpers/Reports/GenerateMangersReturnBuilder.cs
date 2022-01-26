using MultiFamilyPortal.Dtos.Underwriting.Reports;
using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model;

namespace MultiFamilyPortal.Helpers.Reports;

public static class GenerateManagerReturnsBuilder
{
    public static void GenerateManagerReturns(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var page = document.Pages.AddPage();

        var mmr = new ManagersReturnsReport(property);
        page.Size = mmr.HoldYears > 5 ? page.Size = new Size((555 + 105 * mmr.HoldYears), ReportBuilder.LetterSizeHorizontal.Height) : ReportBuilder.LetterSizeHorizontal;
        FixedContentEditor editor = new(page);

        ReportBuilder.Header(page, "Manager Returns");

        var table = new Table { DefaultCellProperties = { Padding = new Thickness(16) } };
        var blackBorder = new Border(1, ReportBuilder.DarkColor);
        table.Borders = new TableBorders(blackBorder);

        var r0 = table.Rows.AddTableRow();
        var r1 = table.Rows.AddTableRow();
        var r2 = table.Rows.AddTableRow();
        var r3 = table.Rows.AddTableRow();
        var r4 = table.Rows.AddTableRow();
        var r5 = table.Rows.AddTableRow();
        var r6 = table.Rows.AddTableRow();

        // row 0
        ReportBuilder.BasicCell(r0, "", ReportBuilder.HeaderColor);
        ReportBuilder.BasicCell(r0, "", ReportBuilder.HeaderColor);
        foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            ReportBuilder.BasicCell(r0, $"Year {year}", ReportBuilder.HeaderColor, true);
        ReportBuilder.BasicCell(r0, $"Total", ReportBuilder.HeaderColor, true);

        // row 1
        ReportBuilder.BasicCell(r1, "Acquisation Fee", ReportBuilder.WhiteColor);
        ReportBuilder.BasicCell(r1, mmr.AcquisitionFee.ToString("C2"), ReportBuilder.PrimaryColor);
        foreach (var year in Enumerable.Range(1, mmr.HoldYears))
            ReportBuilder.BasicCell(r1, "", ReportBuilder.PrimaryColor);
        ReportBuilder.BasicCell(r1, mmr.AcquisitionFee.ToString("C2"), ReportBuilder.PrimaryColor);

        // row 2
        ReportBuilder.BasicCell(r2, "Manager Equity", ReportBuilder.WhiteColor);
        ReportBuilder.BasicCell(r2, mmr.ManagerEquity.ToString("C2"), ReportBuilder.WhiteColor);
        foreach (var year in Enumerable.Range(1, mmr.HoldYears + 1))
            ReportBuilder.BasicCell(r2, "", ReportBuilder.WhiteColor);


        // row 3
        ReportBuilder.BasicCell(r3, "Total Cash Flow", ReportBuilder.WhiteColor);
        ReportBuilder.BasicCell(r3, mmr.ManagerEquity.ToString("C2"), ReportBuilder.PrimaryColor);
        foreach (var yearlycashFlow in mmr.CashFlow)
            ReportBuilder.BasicCell(r3, yearlycashFlow.ToString("C2"), ReportBuilder.PrimaryColor);

        // row 4
        ReportBuilder.BasicCell(r4, $"Cash Flow ({mmr.CashFlowPercentage.ToString("P2")})", ReportBuilder.WhiteColor);
        var totalMCF = 0d;
        foreach (var yearlycashFlow in mmr.CashFlow)
        {
            ReportBuilder.BasicCell(r4, (yearlycashFlow * mmr.CashFlowPercentage).ToString("C2"), ReportBuilder.WhiteColor);
            totalMCF += (yearlycashFlow * mmr.CashFlowPercentage);
        }
        ReportBuilder.BasicCell(r4, totalMCF.ToString("C2"), ReportBuilder.WhiteColor);


        // row 5
        ReportBuilder.BasicCell(r5, "Equity On Sale of Property", ReportBuilder.WhiteColor);
        foreach (var year in Enumerable.Range(0, mmr.HoldYears))
            ReportBuilder.BasicCell(r5, "", ReportBuilder.PrimaryColor);
        ReportBuilder.BasicCell(r5, mmr.EqualityOnSaleOfProperty.ToString("C2"), ReportBuilder.PrimaryColor);
        ReportBuilder.BasicCell(r5, mmr.EqualityOnSaleOfProperty.ToString("C2"), ReportBuilder.PrimaryColor);

        // row 6
        ReportBuilder.BasicCell(r6, "Total", ReportBuilder.WhiteColor, true);
        ReportBuilder.BasicCell(r6, (mmr.AcquisitionFee + mmr.CashFlow.FirstOrDefault() * mmr.CashFlowPercentage).ToString("C2"), ReportBuilder.WhiteColor, true);
        int i = 0;
        foreach (var year in mmr.CashFlow)
        {
            if (i != 0 && i != mmr.HoldYears)
                ReportBuilder.BasicCell(r6, (year * mmr.CashFlowPercentage).ToString("C2"), ReportBuilder.WhiteColor, true);
            else if (i == mmr.HoldYears)
                ReportBuilder.BasicCell(r6, (mmr.EqualityOnSaleOfProperty + year * mmr.CashFlowPercentage).ToString("C2"), ReportBuilder.WhiteColor, true);

            i++;
        }
        ReportBuilder.BasicCell(r6, (totalMCF + mmr.EqualityOnSaleOfProperty).ToString("C2"), ReportBuilder.WhiteColor, true);

        editor.Position.Translate(ReportBuilder.PageMargin / 2, page.Size.Height / 2 - table.Measure().Height / 2);
        editor.DrawTable(table, new Size(page.Size.Width - ReportBuilder.PageMargin, double.PositiveInfinity));

        ReportBuilder.Footer(page, property.Name);
    }
}
