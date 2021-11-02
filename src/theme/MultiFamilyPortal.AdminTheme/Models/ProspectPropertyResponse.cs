using System.ComponentModel.DataAnnotations;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class ProspectPropertyResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public int Units { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CapRate { get; set; }

        [DisplayFormat(DataFormatString = "{0:P}")]
        public double CoC { get; set; }

        public double DebtCoverage { get; set; }

        public string Underwriter { get; set; }

        public string UnderwriterEmail { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTimeOffset Created { get; set; }
    }
}
