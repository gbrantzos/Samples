using DDD.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DDD.Persistence.Configuration;

public class CustomerTypeConfiguration : EntityTypeConfiguration<Customer, CustomerID>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);

        builder.HasIndex(p => p.Code).IsUnique().HasDatabaseName("UQ_Customer_Code");
        builder.Property(p => p.Code).HasMaxLength(50);
        builder.Property(p => p.FullName).HasMaxLength(100);
        builder.Property(p => p.TIN).HasMaxLength(25);

        builder.HasMany(p => p.Addresses)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_Customer_Address")
            .HasRelatedTableIndexName("IX_Address_CustomerID");
    }
}