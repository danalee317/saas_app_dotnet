using System.Reflection;
using System.Text.RegularExpressions;
using MultiFamilyPortal.Data.Models;
using MultiFamilyPortal.Dtos.Underwrting;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;

namespace MultiFamilyPortal.AdminTheme.Services
{
    public class UnderwritingService
    {

        public static byte[] GenerateUnderwritingSpreadsheet(UnderwritingAnalysis analysis)
        {
            var formatProvider = new XlsxFormatProvider();
            var workbook = formatProvider.LoadWorkbook("underwriting.xlsx");
            var aquisition = workbook.GetWorksheet("Acqusition-NF");
            var projections = workbook.GetWorksheet("Year 1 Projection - NF");
            UpdateAcquisition(aquisition, analysis);
            UpdateFirstYear(projections, analysis);

            workbook.ActiveSheet = aquisition;
            return formatProvider.ExportAsByteArray(workbook);
        }

        private static void UpdateFirstYear(Worksheet sheet, UnderwritingAnalysis analysis)
        {

        }

        private static void UpdateAcquisition(Worksheet sheet, UnderwritingAnalysis analysis)
        {
            sheet.SetValue("F4", analysis.Timestamp)
                .SetValue("C5", analysis.Name)
                .SetValue("D6", analysis.Units)
                .SetValue("D7", analysis.PurchasePrice)
                .SetValue("D9", analysis.RentableSqFt)
                .SetValue("H8", analysis.OfferPrice)
                .SetValue("H9", analysis.StrikePrice)
                .SetValue("H10", analysis.AskingPrice)
                .SetValue("M6", analysis.GrossPotentialRent);

            var vacancy = 0.05;
            if (analysis.PhysicalVacancy >= 0.05)
                vacancy = analysis.PhysicalVacancy;
            else if (analysis.MarketVacancy > 0.05)
                vacancy = analysis.MarketVacancy;

            sheet.SetValue("D13", analysis.Ours.Sum(UnderwritingCategory.GrossScheduledRent))
                .SetValue("C14", vacancy)
                .SetValue("D15", analysis.Ours.Sum(UnderwritingCategory.ConsessionsNonPayment))
                .SetValue("D17", analysis.Ours.Sum(UnderwritingCategory.UtilityReimbursement))
                .SetValue("D18", analysis.Ours.Sum(UnderwritingCategory.OtherIncome));

            sheet.SetValue("F13", analysis.Sellers.Sum(UnderwritingCategory.GrossScheduledRent))
                .SetValue("F14", analysis.Sellers.Sum(UnderwritingCategory.PhysicalVacancy))
                .SetValue("F15", analysis.Sellers.Sum(UnderwritingCategory.ConsessionsNonPayment))
                .SetValue("F17", analysis.Sellers.Sum(UnderwritingCategory.UtilityReimbursement))
                .SetValue("F18", analysis.Sellers.Sum(UnderwritingCategory.OtherIncome));

            sheet.SetValue("D22", analysis.Ours.Sum(UnderwritingCategory.Taxes))
                .SetValue("D23", analysis.Ours.Sum(UnderwritingCategory.Insurance))
                .SetValue("D24", analysis.Ours.Sum(UnderwritingCategory.RepairsMaintenance))
                .SetValue("D25", analysis.Ours.Sum(UnderwritingCategory.GeneralAdmin))
                .SetValue("C26", analysis.Management)
                .SetValue("D27", analysis.Ours.Sum(UnderwritingCategory.Marketing))
                .SetValue("D28", analysis.Ours.Sum(UnderwritingCategory.Utility))
                .SetValue("D29", analysis.Ours.Sum(UnderwritingCategory.ContractServices))
                .SetValue("D30", analysis.Ours.Sum(UnderwritingCategory.Payroll));

            sheet.SetValue("F22", analysis.Sellers.Sum(UnderwritingCategory.Taxes))
                .SetValue("F23", analysis.Sellers.Sum(UnderwritingCategory.Insurance))
                .SetValue("F24", analysis.Sellers.Sum(UnderwritingCategory.RepairsMaintenance))
                .SetValue("F25", analysis.Sellers.Sum(UnderwritingCategory.GeneralAdmin))
                .SetValue("F26", analysis.Sellers.Sum(UnderwritingCategory.Management))
                .SetValue("F27", analysis.Sellers.Sum(UnderwritingCategory.Marketing))
                .SetValue("F28", analysis.Sellers.Sum(UnderwritingCategory.Utility))
                .SetValue("F29", analysis.Sellers.Sum(UnderwritingCategory.ContractServices))
                .SetValue("F30", analysis.Sellers.Sum(UnderwritingCategory.Payroll));

            sheet.SetValue("E34", analysis.CapXTotal / analysis.Units)
                .SetValue("E42", analysis.OurEquityOfCF);

            sheet.SetValue("L22", analysis.ClosingCostPercent)
                .SetValue("L25", analysis.AquisitionFeePercent)
                .SetValue("M26", analysis.ClosingCostOther);

            var mortgage = analysis.Mortgages.First();
            sheet.SetValue("M23", mortgage.Points);
            sheet.SetValue("M14", 1 - (mortgage.LoanAmount / analysis.PurchasePrice));
            sheet.SetValue("M17", mortgage.InterestRate);
            sheet.SetValue("M18", mortgage.TermInYears);

            var secondMortgage = analysis.Mortgages.Skip(1).FirstOrDefault() ?? new UnderwritingAnalysisMortgage();
            sheet.SetValue("M32", secondMortgage.LoanAmount)
                .SetValue("M33", secondMortgage.InterestRate)
                .SetValue("M34", secondMortgage.TermInYears);
        }
    }

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
