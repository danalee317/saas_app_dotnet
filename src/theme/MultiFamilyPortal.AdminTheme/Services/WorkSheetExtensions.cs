using System.Reflection;
using System.Text.RegularExpressions;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwrting;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace MultiFamilyPortal.AdminTheme.Services
{
    internal static class WorkSheetExtensions
    {
        private static Assembly Assembly = typeof(WorkSheetExtensions).Assembly;

        public static Workbook LoadWorkbook(this XlsxFormatProvider formatProvider, string name)
        {
            var resourceId = Assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(name));
            return formatProvider.Import(Assembly.GetManifestResourceStream(resourceId));
        }

        public static byte[] ExportAsByteArray(this XlsxFormatProvider formatProvider, Workbook workbook)
        {
            using var output = new MemoryStream();
            formatProvider.Export(workbook, output);
            return output.ToArray();
        }

        public static double Sum(this IEnumerable<UnderwritingAnalysisLineItem> lineItems, UnderwritingCategory category)
        {
            return lineItems.Where(x => x.Category == category)
                .Sum(x => x.AnnualizedTotal);
        }

        public static Worksheet GetWorksheet(this Workbook workbook, string name)
        {
            return workbook.Sheets.OfType<Worksheet>().FirstOrDefault(x => x.Name == name);
        }

        public static Worksheet SetValue(this Worksheet sheet, string cellName, string value)
        {
            var index = GetCellIndex(cellName);
            var cell = sheet.Cells[index];
            cell.SetValueAsText(value);
            return sheet;
        }

        public static Worksheet SetValue(this Worksheet sheet, string cellName, int value)
        {
            var index = GetCellIndex(cellName);
            var cell = sheet.Cells[index];
            cell.SetValue(value);
            return sheet;
        }

        public static Worksheet SetValue(this Worksheet sheet, string cellName, double value)
        {
            var index = GetCellIndex(cellName);
            var cell = sheet.Cells[index];
            cell.SetValue(value);
            return sheet;
        }

        public static Worksheet SetValue(this Worksheet sheet, string cellName, DateTimeOffset value)
        {
            var index = GetCellIndex(cellName);
            var cell = sheet.Cells[index];
            cell.SetValue(value.Date);
            return sheet;
        }

        private static CellIndex GetCellIndex(string cell)
        {
            var column = Regex.Replace(cell, @"\d+", string.Empty).ToUpper();
            var row = Regex.Replace(cell, @"[a-zA-Z]+", string.Empty);
            var columnIndex = column.Sum(x => _alphabet.IndexOf(x)) + (column.Length - 1);
            var rowIndex = int.Parse(row) - 1;
            return new CellIndex(rowIndex, columnIndex);
        }

        private static readonly List<char> _alphabet = new()
        {
            'A',
            'B',
            'C',
            'D',
            'E',
            'F',
            'G',
            'H',
            'I',
            'J',
            'K',
            'L',
            'M',
            'N',
            'O',
            'P',
            'Q',
            'R',
            'S',
            'T',
            'U',
            'V',
            'W',
            'X',
            'Y',
            'Z'
        };
    }
}
