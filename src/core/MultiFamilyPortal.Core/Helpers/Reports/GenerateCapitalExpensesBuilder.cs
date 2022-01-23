using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;

namespace MultiFamilyPortal.Helpers.Reports;

public static class GenerateCapitalExpensesBuilder
{
    public static void GenerateCapitalExpenses(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var pageSize = new Size(900, 1600);
        var headerSize = 18;
        var cellPadding = 22;
        var page = document.Pages.AddPage();
        page.Size = pageSize;
        var editor = new FixedContentEditor(page);

        var textFragment = page.Content.AddTextFragment();
        textFragment.Text = "Capital Expenses";
        textFragment.Position.Translate(page.Size.Width / 2 - 70, 50);
        textFragment.FontSize = headerSize + 10;
        var blackBorder = new Border(1, new RgbColor(0, 0, 0));

        SellerTable(editor, blackBorder, property, pageSize, cellPadding);
        PlannedTable(editor, blackBorder, property, pageSize, cellPadding);

        // conclusion
        var dateBox = page.Content.AddTextFragment();
        dateBox.Text = $"{property.Name} - {DateTime.Now:MM/dd/yyyy}";
        dateBox.Position.Translate(page.Size.Width - 250, page.Size.Height - 10);
    }

    private static void SellerTable(FixedContentEditor editor, Border border, UnderwritingAnalysis property, Size size, double padding = 22)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
            Borders = new TableBorders(border)
        };

        Header(table, property, "Seller Capital Expenses");
        MenuOption(table, property);
        DynamicRow(table, property);

        editor.Position.Translate(size.Width / 2 - table.Measure().Width / 2, 100);
        editor.DrawTable(table);
    }

    private static void PlannedTable(FixedContentEditor editor, Border border, UnderwritingAnalysis property, Size size, double padding = 22)
    {
        var table = new Table
        {
            DefaultCellProperties = { Padding = new Thickness(padding) },
            LayoutType = TableLayoutType.FixedWidth,
            Borders = new TableBorders(border)
        };

        Header(table, property, "Planned Capital Expenses", 4);
        MenuOption(table, property, true);
        DynamicRow(table, property, true);

        editor.Position.Translate(size.Width / 2 - table.Measure().Width / 2, 850);
        editor.DrawTable(table);
    }

    private static void Header(Table table, UnderwritingAnalysis property, string name, int colSpan = 3)
    {
        var header = table.Rows.AddTableRow();
        var title = header.Cells.AddTableCell();
        title.ColumnSpan = colSpan;
        title.Background = new RgbColor(137, 207, 240);
        var titleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        titleBlock.InsertText(name);
        title.Blocks.Add(titleBlock);
    }

    private static void MenuOption(Table table, UnderwritingAnalysis property, bool isPlanned = false)
    {
        var row = table.Rows.AddTableRow();
        var costTitle = row.Cells.AddTableCell();
        var costTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
        };
        costTitleBlock.InsertText("Cost");
        costTitle.Blocks.Add(costTitleBlock);

        var statusTitle = row.Cells.AddTableCell();
        var statusTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
        };
        statusTitleBlock.InsertText("Status");
        statusTitle.Blocks.Add(statusTitleBlock);

        if (isPlanned)
        {
            var fundedTitle = row.Cells.AddTableCell();
            var fundedTitleBlock = new Block
            {
                TextProperties = { Font = FontsRepository.HelveticaBold },
            };
            fundedTitleBlock.InsertText("Funded By");
            fundedTitle.Blocks.Add(fundedTitleBlock);
        }

        var descriptionTitle = row.Cells.AddTableCell();
        var descriptionTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        descriptionTitleBlock.InsertText("Description");
        descriptionTitle.Blocks.Add(descriptionTitleBlock);
    }


    private static void DynamicRow(Table table, UnderwritingAnalysis property, bool isPlanned = false)
    {

        foreach (var increase in Enumerable.Range(1, 10))
        {
            var row = table.Rows.AddTableRow();

            var costCell = row.Cells.AddTableCell();
            var costCellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            costCellBlock.InsertText("$3455.43");
            costCell.Blocks.Add(costCellBlock);

            var statusCell = row.Cells.AddTableCell();
            var statusCellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            statusCellBlock.InsertText("Pending");
            statusCell.Blocks.Add(statusCellBlock);

            if (isPlanned)
            {
                var fundedCell = row.Cells.AddTableCell();
                var fundedCellBlock = new Block
                {
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                fundedCellBlock.InsertText("Investors");
                fundedCell.Blocks.Add(fundedCellBlock);
            }

            var descriptionCell = row.Cells.AddTableCell();
            var descriptionCellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            descriptionCellBlock.InsertText("Phasellus vitae lorem aliquet, vulputate nisl vel, varius tortor.");
            descriptionCell.Blocks.Add(descriptionCellBlock);
        }
    }
}
