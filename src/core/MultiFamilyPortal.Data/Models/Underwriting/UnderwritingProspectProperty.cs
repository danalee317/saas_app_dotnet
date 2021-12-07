﻿using System.ComponentModel.DataAnnotations.Schema;

namespace MultiFamilyPortal.Data.Models
{
    public class UnderwritingProspectProperty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid BucketListId { get; set; }

        private DateTimeOffset _timestamp = DateTimeOffset.UtcNow;
        public DateTimeOffset Timestamp => _timestamp;

        public DateTimeOffset StartDate { get; set; }

        public string Name { get; set; }

        public string UnderwriterId { get; set; }

        public string Address { get; set; }

        public string Market { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public int Units { get; set; }

        public int Vintage { get; set; }

        public double CapRate { get; set; }

        public double CashOnCash { get; set; }

        public double DebtCoverage { get; set; }

        public double DesiredYield { get; set; }

        public int HoldYears { get; set; }

        public double NOI { get; set; }

        public double AskingPrice { get; set; }

        public double StrikePrice { get; set; }

        public double OfferPrice { get; set; }

        public double PurchasePrice { get; set; }

        public double Downpayment { get; set; }

        public int RentableSqFt { get; set; }

        public double GrossPotentialRent { get; set; }

        public double PhysicalVacancy { get; set; }

        public double MarketVacancy { get; set; }

        public double Management { get; set; }

        public double CapX { get; set; }

        public CostType CapXType { get; set; }

        public double OurEquityOfCF { get; set; } = 0.25;

        public double AquisitionFeePercent { get; set; } = 0.05;

        public double ClosingCostPercent { get; set; } = 0.03;

        public double DeferredMaintenance { get; set; }

        public double SECAttorney { get; set; }

        public double ClosingCostMiscellaneous { get; set; }

        public UnderwritingStatus Status { get; set; }

        public UnderwritingLoanType LoanType { get; set; }

        public double LTV { get; set; } = 0.8;

        public PropertyClass PropertyClass { get; set; } = PropertyClass.ClassB;

        public PropertyClass NeighborhoodClass { get; set; } = PropertyClass.ClassB;

        public SiteUser Underwriter { get; set; }

        public UnderwritingProspectPropertyBucketList BucketList { get; set; }

        public virtual ICollection<UnderwritingProspectPropertyCapitalImprovements> CapitalImprovements { get; set; }

        public virtual ICollection<UnderwritingLineItem> LineItems { get; set; }

        public virtual ICollection<UnderwritingNote> Notes { get; set; }

        public virtual ICollection<UnderwritingMortgage> Mortgages { get; set; }

        public virtual ICollection<UnderwritingPropertyUnitModel> Models { get; set; }

        public virtual ICollection<UnderwritingProspectFile> Files { get; set; }
    }
}
