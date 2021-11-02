using System.ComponentModel.DataAnnotations;
using MultiFamilyPortal.Data.ComponentModel;

namespace MultiFamilyPortal.Data.Models
{
    public enum UnderwritingCategory
    {
        // Income
        [UnderwritingType(UnderwritingType.Income)]
        [Display(Name = "Gross Scheduled Rent")]
        GrossScheduledRent,

        [UnderwritingType(UnderwritingType.Income)]
        [Display(Name = "Physical Vacancy")]
        PhysicalVacancy,

        [UnderwritingType(UnderwritingType.Income)]
        [Display(Name = "Concessions / Non-Payment")]
        ConsessionsNonPayment,

        [UnderwritingType(UnderwritingType.Income)]
        [Display(Name = "Utility Reimbursement")]
        UtilityReimbursement,

        [UnderwritingType(UnderwritingType.Income)]
        [Display(Name = "Other Income")]
        OtherIncome,

        // Expenses
        [UnderwritingType(UnderwritingType.Expense)]
        [Display(Name = "Taxes")]
        Taxes,

        [UnderwritingType(UnderwritingType.Expense)]
        [Display(Name = "Insurance")]
        Insurance,

        [UnderwritingType(UnderwritingType.Expense)]
        [Display(Name = "Repairs / Maintenance")]
        RepairsMaintenance,

        [UnderwritingType(UnderwritingType.Expense)]
        [Display(Name = "General / Admin")]
        GeneralAdmin,

        [UnderwritingType(UnderwritingType.Expense)]
        [Display(Name = "Management")]
        Management,

        [UnderwritingType(UnderwritingType.Expense)]
        [Display(Name = "Marketing")]
        Marketing,

        [UnderwritingType(UnderwritingType.Expense)]
        [Display(Name = "Utility")]
        Utility,

        [UnderwritingType(UnderwritingType.Expense)]
        [Display(Name = "Contract Services")]
        ContractServices,

        [UnderwritingType(UnderwritingType.Expense)]
        [Display(Name = "Payroll")]
        Payroll
    }
}
