using System.ComponentModel.DataAnnotations.Schema;

namespace MultiFamilyPortal.Data.Models
{
    public class UnderwritingPropertyUnitModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }

        public string Name { get; set; }

        public int Beds { get; set; }

        public double Baths { get; set; }

        public double MarketRent { get; set; }

        public UnderwritingProspectProperty Property { get; set; }
    }
}
