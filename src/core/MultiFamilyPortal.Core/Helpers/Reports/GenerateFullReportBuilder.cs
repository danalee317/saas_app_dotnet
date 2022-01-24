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
    public static async Task GenerateFullReportAsync(UnderwritingAnalysis property, RadFixedDocument document, IBrandService brand)
    {
        var pageSize = new Size(1503, 800);
        var headerSize = 18;
        var cellPadding = 25;
        var page = document.Pages.AddPage();
        page.Size = pageSize;
        var editor = new FixedContentEditor(page);

        var textFragment = page.Content.AddTextFragment();
        textFragment.Text = $"{property.Name} Full Report";
        textFragment.Position.Translate(page.Size.Width / 2 - 200, 100);
        textFragment.FontSize = headerSize + 20;
        var blackBorder = new Border(1, new RgbColor(200, 200, 200));

        CreateDetail(editor, blackBorder, property, 200, cellPadding);
        await CreateIdentity(page, property, pageSize.Width * 3 / 4, brand);
        CreateAddress(editor, blackBorder, property, pageSize.Width * 3 / 4, cellPadding);
    }

    private static void CreateDetail(FixedContentEditor editor, Border border, UnderwritingAnalysis property, double widthStart, double padding = 22)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.AutoFit,
            Borders = new TableBorders(border)
        };

        SimpleRow(table, "Name", property.Name);
        SimpleRow(table, "Market", property.Market);
        SimpleRow(table, "Price", property.OfferPrice.ToString("C2"));
        SimpleRow(table, "Cap Rate", property.CapRate.ToString("P2"));
        SimpleRow(table, "Debt Coverage Ratio", property.DebtCoverage.ToString("F2"));
        SimpleRow(table, "Investor Cash On Cash Return", property.CashOnCash.ToString("P2"));
        SimpleRow(table, "Number of Units", property.Units.ToString());
        SimpleRow(table, "Date", DateTime.Now.ToString("MM/dd/yyyy"));

        editor.Position.Translate(widthStart, 150);
        editor.DrawTable(table);
    }


    private static async Task CreateIdentity(RadFixedPage page, UnderwritingAnalysis property, double widthStart, IBrandService brand)
    {

       // TODO : figure this out
        var imageStream = await brand.GetBrandImage("logos");

        if (imageStream.Stream != Stream.Null)
        {
            var imageSource = new ImageSource(imageStream.Stream);
            Image image = page.Content.AddImage(imageSource);
        }
        else
        {
            Console.WriteLine("Well its empty");
        }
    }

    private static void CreateAddress(FixedContentEditor editor, Border border, UnderwritingAnalysis property, double widthStart, double padding = 22)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.AutoFit,
            Borders = new TableBorders(border)
        };


        editor.Position.Translate(widthStart, 100);
        editor.DrawTable(table);
    }

    private static void SimpleRow(Table table, string Title, string Value)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rowTitleBlock.InsertText(Title);
        rowTitle.Blocks.Add(rowTitleBlock);

        var rowSpace = row.Cells.AddTableCell();
        var rowSpaceBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        rowSpaceBlock.InsertText(":");
        rowSpace.Blocks.Add(rowSpaceBlock);

        var rowValue = row.Cells.AddTableCell();
        var rowValueBlock = new Block { HorizontalAlignment = HorizontalAlignment.Left };
        rowValueBlock.InsertText(Value);
        rowValue.Blocks.Add(rowValueBlock);
    }
}
