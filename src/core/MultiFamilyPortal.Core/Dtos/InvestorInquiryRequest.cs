using System.ComponentModel.DataAnnotations;

namespace MultiFamilyPortal.Dtos
{
    public class InvestorInquiryRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(25)]
        public string LastName { get; set; }

        [Required]
        [MinLength(10)]
        [MaxLength(15)]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Timezone { get; set; }

        [Required]
        public string LookingToInvest { get; set; }

        public string Comments { get; set; }
    }
}
