using MultiFamilyPortal.Dtos.Underwriting;
using Telerik.Documents.Primitives;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.ColorSpaces;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.Model.Editing.Flow;
using Telerik.Windows.Documents.Fixed.Model.Editing.Tables;
using Telerik.Windows.Documents.Fixed.Model.Fonts;

namespace MultiFamilyPortal.Helpers.Reports;

public static class GenerateIncomeForecastBuilder
{
    public static void GenerateIncomeForecast(UnderwritingAnalysis property, RadFixedDocument document)
    {
        var pageSize = new Size(400 + (100 * property.HoldYears + 2), 750);
        var headerSize = 18;
        var cellPadding = 22;
        var page = document.Pages.AddPage();
        page.Size = pageSize;
        var editor = new FixedContentEditor(page);

        var textFragment = page.Content.AddTextFragment();
        textFragment.Text = "Income Forecast";
        textFragment.Position.Translate(page.Size.Width / 2 - 50, 50);
        textFragment.FontSize = headerSize + 10;
        var blackBorder = new Border(1, new RgbColor(0, 0, 0));

        CreateTable(editor, blackBorder, property, pageSize, cellPadding);

        // conclusion
        var dateBox = page.Content.AddTextFragment();
        dateBox.Text = $"{property.Name} - {DateTime.Now:MM/dd/yyyy}";
        dateBox.Position.Translate(page.Size.Width - 250, page.Size.Height - 10);
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
        IncreaseType(table, property);
        PerUnitIncrease(table, property);
        UnitsAppliedTo(table, property);
        RemainingUnits(table, property);
        Vacancy(table, property);
        OtherLosses(table, property);
        UtilityIncreases(table, property);
        OtherIncome(table, property);

        editor.Position.Translate(size.Width / 2 - table.Measure().Width / 2, 100);
        editor.DrawTable(table);
    }

    #region Income Forecast Rows
    private static void Header(Table table, UnderwritingAnalysis property)
    {
        var header = table.Rows.AddTableRow();
        var title = header.Cells.AddTableCell();
        title.ColumnSpan = 2 + property.HoldYears;
        title.Background = new RgbColor(137, 207, 240);
        var titleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Center
        };
        titleBlock.InsertText("Income Forecast");
        title.Blocks.Add(titleBlock);
    }

    private static void Year(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rowTitleBlock.InsertText("Year");
        rowTitle.Blocks.Add(rowTitleBlock);

        foreach (var year in property.IncomeForecast.Select(x => x.Year))
        {
            var cell = row.Cells.AddTableCell();
            cell.Background = new RgbColor(248, 249, 250);
            var cellBlock = new Block
            {
                TextProperties = { Font = FontsRepository.HelveticaBold },
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var currentYear = year == 0 ? "Start Year* : " + property.StartDate.Year : $"{property.StartDate.Year + year}";
            cellBlock.InsertText(currentYear.ToString());
            cell.Blocks.Add(cellBlock);
        }
    }

    private static void IncreaseType(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        rowTitleBlock.InsertText("Increase Type");
        rowTitle.Blocks.Add(rowTitleBlock);

        foreach (var increase in property.IncomeForecast.Select(x => x.IncreaseType))
        {
            var cell = row.Cells.AddTableCell();
            var cellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            cellBlock.InsertText(increase.ToString());
            cell.Blocks.Add(cellBlock);
        }
    }

    private static void PerUnitIncrease(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        rowTitleBlock.InsertText("Per Unit Increase");
        rowTitle.Blocks.Add(rowTitleBlock);

        foreach (var increase in property.IncomeForecast.Select(x => x.PerUnitIncrease))
        {
            var cell = row.Cells.AddTableCell();
            cell.Background = new RgbColor(248, 249, 250);
            var cellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            cellBlock.InsertText(increase.ToString());
            cell.Blocks.Add(cellBlock);
        }
    }

    private static void UnitsAppliedTo(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        rowTitleBlock.InsertText("Units Applied To");
        rowTitle.Blocks.Add(rowTitleBlock);

        foreach (var units in property.IncomeForecast.Select(x => x.UnitsAppliedTo))
        {
            var cell = row.Cells.AddTableCell();
            var cellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            cellBlock.InsertText(units.ToString());
            cell.Blocks.Add(cellBlock);
        }
    }

    private static void RemainingUnits(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        rowTitleBlock.InsertText("Increase on Remaining Units");
        rowTitle.Blocks.Add(rowTitleBlock);

        foreach (var increase in property.IncomeForecast.Select(x => x.FixedIncreaseOnRemainingUnits))
        {
            var cell = row.Cells.AddTableCell();
            cell.Background = new RgbColor(248, 249, 250);
            var cellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            cellBlock.InsertText(increase.ToString());
            cell.Blocks.Add(cellBlock);
        }
    }

    private static void Vacancy(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        rowTitleBlock.InsertText("Vacancy");
        rowTitle.Blocks.Add(rowTitleBlock);

        foreach (var vacancy in property.IncomeForecast.Select(x => x.Vacancy))
        {
            var cell = row.Cells.AddTableCell();
            var cellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            var currentVacancy = vacancy == 0 ? property.PhysicalVacancy : vacancy;
            cellBlock.InsertText(currentVacancy.ToString("P2"));
            cell.Blocks.Add(cellBlock);
        }
    }

    private static void OtherLosses(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block
        {
            TextProperties = { Font = FontsRepository.HelveticaBold },
            HorizontalAlignment = HorizontalAlignment.Right
        };
        rowTitleBlock.InsertText("Other Losses");
        rowTitle.Blocks.Add(rowTitleBlock);

        foreach (var loss in property.IncomeForecast.Select(x => x.OtherLossesPercent))
        {
            var cell = row.Cells.AddTableCell();
            cell.Background = new RgbColor(248, 249, 250);
            var cellBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
            cellBlock.InsertText(loss.ToString("P2"));
            cell.Blocks.Add(cellBlock);
        }
    }

    private static void UtilityIncreases(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        rowTitleBlock.InsertText("Utility Increases");
        rowTitle.Blocks.Add(rowTitleBlock);

        foreach (var increase in property.IncomeForecast.Select(x => x.UtilityIncreases))
        {
            var cell = row.Cells.AddTableCell();
            var cellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            cellBlock.InsertText(increase.ToString("C2"));
            cell.Blocks.Add(cellBlock);
        }
    }

    private static void OtherIncome(Table table, UnderwritingAnalysis property)
    {
        var row = table.Rows.AddTableRow();
        var rowTitle = row.Cells.AddTableCell();
        var rowTitleBlock = new Block { HorizontalAlignment = HorizontalAlignment.Right };
        rowTitleBlock.InsertText("Other Income");
        rowTitle.Blocks.Add(rowTitleBlock);

        foreach (var income in property.IncomeForecast.Select(x => x.OtherIncomePercent))
        {
            var cell = row.Cells.AddTableCell();
            cell.Background = new RgbColor(248, 249, 250);
            var cellBlock = new Block
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            cellBlock.InsertText(income.ToString("P2"));
            cell.Blocks.Add(cellBlock);
        }
    }
    # endregion
}
