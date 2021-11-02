using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.AdminTheme.Models
{
    public class ActivityResponse
    {
        public Guid Id { get; set; }

        public TimeSpan Total { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTime Date { get; set; }

        [DisplayFormat(DataFormatString = "{0:hh:mm}")]
        public DateTimeOffset Timestamp { get; set; }

        public string Description { get; set; }

        public ActivityType Type { get; set; }

        public string UserName { get; set; }

        public string UserEmail { get; set; }
    }
}
