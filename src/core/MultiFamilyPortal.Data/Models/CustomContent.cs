using System.ComponentModel.DataAnnotations;

namespace MultiFamilyPortal.Data.Models
{
    public class CustomContent
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string HtmlContent { get; set; }
    }
}
