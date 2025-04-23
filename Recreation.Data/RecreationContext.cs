using Microsoft.EntityFrameworkCore;
using Recreation.Data.Entities;

namespace Recreation.Data
{
    public class RecreationContext(DbContextOptions<RecreationContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("recr");
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.Entity<RecreationItem>()
                .HasDiscriminator(q => q.Type)
                .HasValue<Park>(Enums.RecreationType.Park)
                .HasValue<BikeLane>(Enums.RecreationType.BikeLane)
                .HasValue<RecreationArea>(Enums.RecreationType.RecreationArea);

            modelBuilder.Entity<ItemType>()
                .HasMany<RecreationItem>()
                .WithOne(q => q.ItemType)
                .HasForeignKey(q => q.ItemTypeId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
