using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Data.Models;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;


namespace MultiFamilyPortal.Helpers.Reports;

public static class GenerateIncomeForecastBuilder
{
    public static void GenerateIncomeForecast(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var cellPadding = property.HoldYears > 5 ? 12 : 23.5;
        var page = document.Pages.AddPage();
        page.Size = ReportBuilder.LetterSizeHorizontal;
        var editor = new FixedContentEditor(page);

        ReportBuilder.Header(page, "Income Forecast");
        var blackBorder = new Border(1, ReportBuilder.DarkColor);

        CreateTable(editor, blackBorder, property, page.Size, cellPadding);
        ReportBuilder.Footer(page, property.Name);
    }

    private static void CreateTable(FixedContentEditor editor, Border border, UnderwritingAnalysis property, Size size, double padding = 22)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
            Borders = new TableBorders(border)
        };

        Header(table, property);
        Year(table, property);
        PerUnitIncrease(table, property);
        UnitsAppliedTo(table, property);
        RemainingUnits(table, property);
        Vacancy(table, property);
        OtherLosses(table, property);
        UtilityIncreases(table, property);
        OtherIncome(table, property);

        editor.Position.Translate(size.Width / 2 - table.Measure().Width / 2, 150);
        editor.DrawTable(table);
    }

    #region Income Forecast Rows
    private static void Header(Table table, UnderwritingAnalysis property)
    {
        ReportBuilder.BasicCell(table.Rows.AddTableRow(), "Income Forecast", ReportBuilder.HeaderColor, true, HorizontalAlignment.Center, 2 + property.HoldYears);
    }

    private static void Year(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        ReportBuilder.BasicCell(row, "Year", ReportBuilder.WhiteColor, true);
        foreach (var year in property.IncomeForecast.Select(x => x.Year))
        {
            var currentYear = year == 0 ? "Start Year* : " + property.StartDate.Year : $"{property.StartDate.Year + year}";
            ReportBuilder.BasicCell(row, currentYear.ToString(), ReportBuilder.WhiteColor, true);
        }
    }

    private static void PerUnitIncrease(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        ReportBuilder.BasicCell(row, "Per Unit Increase", ReportBuilder.WhiteColor);
        int i = 0;
        foreach (var increase in property.IncomeForecast.Select(x => x.PerUnitIncrease))
        {
            var format = property.IncomeForecast.ToList()[i].IncreaseType == IncomeForecastIncreaseType.Percent ? "P2" : "C2";
            ReportBuilder.BasicCell(row, increase.ToString(format), ReportBuilder.WhiteColor);
            i++;
        }
    }

    private static void UnitsAppliedTo(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        ReportBuilder.BasicCell(row, "Units Applied To", ReportBuilder.WhiteColor);
        foreach (var units in property.IncomeForecast.Select(x => x.UnitsAppliedTo))
            ReportBuilder.BasicCell(row, units.ToString(), ReportBuilder.PrimaryColor);
    }

    private static void RemainingUnits(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        ReportBuilder.BasicCell(row, "Increase on Remaining Units",ReportBuilder.WhiteColor);
        foreach (var increase in property.IncomeForecast.Select(x => x.FixedIncreaseOnRemainingUnits))
            ReportBuilder.BasicCell(row, increase.ToString("C2"), ReportBuilder.WhiteColor);
    }

    private static void Vacancy(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        ReportBuilder.BasicCell(row, "Vacancy", ReportBuilder.WhiteColor);
        foreach (var vacancy in property.IncomeForecast.Select(x => x.Vacancy))
            ReportBuilder.BasicCell(row, vacancy.ToString("P2"), ReportBuilder.PrimaryColor);
    }

    private static void OtherLosses(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        ReportBuilder.BasicCell(row, "Other Losses", ReportBuilder.WhiteColor);
        foreach (var loss in property.IncomeForecast.Select(x => x.OtherLossesPercent))
            ReportBuilder.BasicCell(row, loss.ToString("P2"), ReportBuilder.WhiteColor);
    }

    private static void UtilityIncreases(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        ReportBuilder.BasicCell(row, "Utility Increases", ReportBuilder.WhiteColor);
        foreach (var increase in property.IncomeForecast.Select(x => x.UtilityIncreases))
            ReportBuilder.BasicCell(row, increase.ToString("C2"), ReportBuilder.PrimaryColor);
    }

    private static void OtherIncome(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        ReportBuilder.BasicCell(row, "Other Income", ReportBuilder.WhiteColor);
        foreach (var income in property.IncomeForecast.Select(x => x.OtherIncomePercent))
            ReportBuilder.BasicCell(row, income.ToString("P2"), ReportBuilder.WhiteColor);
    }
    # endregion
}
