using Microsoft.EntityFrameworkCore;
using MultiFamilyPortal.Data.Models;

namespace MultiFamilyPortal.Data.ModelConfiguration
{
    internal static class UnderwritingExtensions
    {
        public static void ConfigureUnderwriting(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnderwritingProspectProperty>(builder =>
            {
                builder.HasOne(x => x.BucketList)
                    .WithOne(x => x.Property)
                    .HasForeignKey<UnderwritingProspectPropertyBucketList>(x => x.PropertyId);
            });
            modelBuilder.HasField<UnderwritingProspectProperty, DateTimeOffset>(x => x.Timestamp);
            modelBuilder.HasField<UnderwritingNote, DateTimeOffset>(x => x.Timestamp);
        }
    }
}
