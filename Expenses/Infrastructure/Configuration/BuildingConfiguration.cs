using Expenses.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Expenses.Infrastructure.Configuration
{
    public class BuildingConfiguration : IEntityTypeConfiguration<Building>
    {
        public void Configure(EntityTypeBuilder<Building> builder)
        {
            builder
                .ToTable("Building")
                .OwnsOne(c => c.Address, b=> b.ToTable("Address"));

            builder
                .HasMany(b => b.Apartments)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .Metadata
                .FindNavigation(nameof(Building.Apartments))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
