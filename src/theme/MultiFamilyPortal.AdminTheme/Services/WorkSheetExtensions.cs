﻿using System.Reflection;
using System.Text.RegularExpressions;
using Humanizer;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwrting;
using MultiFamilyPortal.Extensions;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace MultiFamilyPortal.AdminTheme.Services
{
    internal static class WorkSheetExtensions
    {
        private static Assembly Assembly = typeof(WorkSheetExtensions).Assembly;
        private const string CoachingForm = "Coaching Form";
        private const string UnderwritingNotes = "Underwriting Notes";

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
            if (value is null)
                return sheet;

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

        public static void UpdateCoachingForm(this Workbook workbook, UnderwritingAnalysis analysis)
        {
            var sheet = workbook.GetWorksheet(CoachingForm);

            sheet.SetValue("A4", analysis.BucketList.Summary)
                .SetValue("A6", analysis.BucketList.ValuePlays)
                .SetValue("C7", analysis.Vintage)
                .SetValue("A9", analysis.BucketList.ConstructionType)
                .SetValue("G10", analysis.BucketList.UtilityNotes)
                .SetValue("D11", analysis.DeferredMaintenance)
                .SetValue("D12", analysis.PropertyClass.Humanize(LetterCasing.Title))
                .SetValue("D13", analysis.NeighborhoodClass.Humanize(LetterCasing.Title))
                .SetValue("D24", analysis.PhysicalVacancy)
                .SetValue("D25", analysis.PricePerUnit)
                .SetValue("D26", analysis.CapRate)
                .SetValue("D27", analysis.CashOnCash)
                .SetValue("D28", analysis.AskingPrice)
                .SetValue("D29", analysis.OfferPrice)
                .SetValue("D30", analysis.StrikePrice)
                .SetValue("D32", analysis.MarketVacancy)
                .SetValue("D33", analysis.BucketList.MarketPricePerUnit)
                .SetValue("D34", analysis.BucketList.MarketCapRate)
                .SetValue("A36", analysis.BucketList.CompetitionNotes)
                .SetValue("A40", analysis.BucketList.HowUnderwritingWasDetermined);
        }

        public static void AddNotes(this Workbook workbook, UnderwritingAnalysis analysis)
        {
            var sheet = workbook.GetWorksheet(UnderwritingNotes);

            var row = 2;
            foreach (var note in analysis.Notes)
            {
                if (string.IsNullOrEmpty(note.Note?.ConvertToPlainText()))
                    continue;

                sheet.Cells[new CellIndex(2, 0)].SetValue(note.Underwriter);
                sheet.Cells[new CellIndex(2, 1)].SetValue(note.Timestamp.Date);
                sheet.Cells[new CellIndex(2, 2)].SetValue(note.Note.ConvertToPlainText());
                row++;
            }
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