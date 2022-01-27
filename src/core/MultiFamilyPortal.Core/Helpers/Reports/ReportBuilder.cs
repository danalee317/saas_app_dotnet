using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Fonts;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Media;
using Telerik.Windows.Documents.Model;

public static class ReportBuilder
{
    #region Document variables
    public static readonly double HeaderSize = 25;
    public static readonly double PageMargin = 96 * 0.5;
    public static readonly Size LetterSize = PaperTypeConverter.ToSize(PaperTypes.Letter);
    public static readonly Size LetterSizeHorizontal = new Size(LetterSize.Height, LetterSize.Width);
    public static readonly RgbColor PrimaryColor = new RgbColor(248, 249, 250);
    public static readonly RgbColor SecondaryColor = new RgbColor(239, 222, 205);
    public static readonly RgbColor TertiaryColor = new RgbColor(64, 224, 208);
    public static readonly RgbColor HeaderColor = new RgbColor(137, 207, 240);
    public static readonly RgbColor WhiteColor = new RgbColor(255, 255, 255);
    public static readonly RgbColor DarkColor = new RgbColor(0, 0, 0);

    #endregion

    #region Document creation
    public static void Header(RadFixedPage page, string title)
    {
        var header = page.Content.AddTextFragment();
        header.Text = title;
        header.Position.Translate(page.Size.Width / 2 - title.Length * 6, PageMargin * 2);
        header.FontSize = HeaderSize;
    }

    public static void Footer(RadFixedPage page, string name)
    {
        var footer = page.Content.AddTextFragment();
        footer.Text = $"{name} - {DateTime.Now:MM/dd/yyyy}";
        footer.Position.Translate(page.Size.Width - 150 - PageMargin, page.Size.Height - 60);
    }
    #endregion


    #region Table creation


    public static void ByTwoRow(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }


    public static void ByThreeRow(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }

    public static void BasicCell(TableRow row, string value, RgbColor color, bool isBold = false, HorizontalAlignment alignment = HorizontalAlignment.Right, int ColumnSpan = 1)
    {
        var cell = row.Cells.AddTableCell();
        cell.Background = color;
        cell.ColumnSpan = ColumnSpan;
        var block = new Block
        {
            TextProperties = { Font = isBold ? FontsRepository.HelveticaBold : FontsRepository.Helvetica, },
            HorizontalAlignment = alignment,
        };
        block.InsertText(value);
        cell.Blocks.Add(block);
    }

    public static void DynamicRow(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }

    public static void DynamicColumn(UnderwritingAnalysis property, RadFixedDocument document)
    {

    }
    #endregion
}