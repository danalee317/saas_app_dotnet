using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Data.ModelConfiguration
{
    internal static class UnderwritingExtensions
    {
        public static void ConfigureUnderwriting(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasField<UnderwritingProspectProperty, DateTimeOffset>(x => x.Timestamp);
            modelBuilder.HasField<UnderwritingNote, DateTimeOffset>(x => x.Timestamp);
        }
    }
}
