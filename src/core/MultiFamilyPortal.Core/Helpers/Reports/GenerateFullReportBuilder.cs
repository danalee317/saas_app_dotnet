using Humanizer;
using MultiFamilyPortal.Dtos.Underwriting;
using MultiFamilyPortal.Dtos.Underwriting.Reports;
using MultiFamilyPortal.Services;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;
using Telerik.Windows.Documents.Fixed.Model.Objects;
using Telerik.Windows.Documents.Fixed.Model.Resources;

namespace MultiFamilyPortal.Helpers.Reports;

public static class GenerateFullReportBuilder
{
    public static void GenerateFullReport(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var headerSize = 60;
        var cellPadding = 25;
        var page = document.Pages.AddPage();
        var pageSize = new Size(1503, 800);
        page.Size = pageSize;

        var editor = new FixedContentEditor(page);
        var blackBorder = new Border(1, new RgbColor(200, 200, 200));


        CreateHeader(editor, property, page.Size.Width, cellPadding);
        CreateAddress(page, property, pageSize.Width * 1 / 2 + 5);
        CreateDetail(editor, blackBorder, property, pageSize.Width, cellPadding);

    }

    private static void CreateHeader(FixedContentEditor editor, UnderwritingAnalysis property, double widthStart, double padding = 22)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding + 10) },
            LayoutType = TableLayoutType.AutoFit,
        };

        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        rowTitle.Background = new RgbColor(137, 207, 240);
        var rowTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold, FontSize = 50 },
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        rowTitleBlock.InsertText($"{property.Name} Full Report");
        rowTitle.Blocks.Add(rowTitleBlock);

        editor.Position.Translate(widthStart / 2 - table.Measure().Width / 2, 10);
        editor.DrawTable(table);
    }

    private static void CreateDetail(FixedContentEditor editor, Border border, UnderwritingAnalysis property, double widthStart, double padding = 22)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.AutoFit,
            Borders = new TableBorders(border)
        };

        SimpleRow(table, "Class", property.PropertyClass.Humanize(LetterCasing.Title));
        SimpleRow(table, "Price", property.OfferPrice.ToString("C2"));
        SimpleRow(table, "Cap Rate", property.CapRate.ToString("P2"));
        SimpleRow(table, "Debt Coverage Ratio", property.DebtCoverage.ToString("F2"));
        SimpleRow(table, "Investor Cash On Cash Return", property.CashOnCash.ToString("P2"));
        SimpleRow(table, "Number of Units", property.Units.ToString());
        SimpleRow(table, "Date", DateTime.Now.ToString("MM/dd/yyyy"));

        editor.Position.Translate(widthStart / 2 - table.Measure().Width / 2, 290);
        editor.DrawTable(table);
    }

    private static void CreateAddress(RadFixedPage page, UnderwritingAnalysis property, double widthStart, double fontSize = 18)
    {
        var marketFragment = page.Content.AddTextFragment();
        marketFragment.Text = $"{property.Market}";
        marketFragment.Position.Translate(widthStart - 265, 225);
        marketFragment.FontSize = fontSize + 20;

        var addressFragment = page.Content.AddTextFragment();
        addressFragment.Text = $"{property.Address}";
        addressFragment.Position.Translate(widthStart, 180);
        addressFragment.FontSize = fontSize;

        var cityFragment = page.Content.AddTextFragment();
        cityFragment.Text = $"{property.City}, {property.State}, {property.Zip}";
        cityFragment.Position.Translate(widthStart, 220);
        cityFragment.FontSize = fontSize;

        var countryFragment = page.Content.AddTextFragment();
        countryFragment.Text = "United States";
        countryFragment.Position.Translate(widthStart, 260);
        countryFragment.FontSize = fontSize;
    }

    private static void SimpleRow(Table table, string title, string value, double fontSize = 18)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold, FontSize = fontSize },
            HorizontalAlignment = HorizontalAlignment.Right,
        };
        rowTitleBlock.InsertText(title);
        rowTitle.Blocks.Add(rowTitleBlock);

        var rowSpace = row.Cells.AddTableCell();
        var rowSpaceBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold, FontSize = fontSize },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        rowSpaceBlock.InsertText(":");
        rowSpace.Blocks.Add(rowSpaceBlock);

        var rowValue = row.Cells.AddTableCell();
        var rowValueBlock = new Block { TextProperties = { FontSize = fontSize } };
        rowValueBlock.InsertText(value);
        rowValue.Blocks.Add(rowValueBlock);
    }
}
