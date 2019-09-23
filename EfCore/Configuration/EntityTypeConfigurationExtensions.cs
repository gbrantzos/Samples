using EfCore.Core;
using EfCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace EfCore.Configuration
{
    public static class EntityTypeConfigurationExtensions
    {
        private const string DefaultColumnName = "ID";

        public static PropertyBuilder<TProperty> IsEntityID<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder,
            string columnName = DefaultColumnName)
            where TProperty : BaseEntityID<int>, new()
        {
            propertyBuilder
                .HasColumnName(columnName)
                .HasValueGenerator<CustomIdGenerator>()
                .HasConversion(
                    v => v.ID,
                    v => EntityID.FromValue<TProperty>(v))
                .Metadata.BeforeSaveBehavior = Microsoft.EntityFrameworkCore.Metadata.PropertySaveBehavior.Save;
            return propertyBuilder;
        }
    }

    public class CustomIdGenerator : ValueGenerator
    {
        protected override object NextValue(EntityEntry entry)
        {
            var hilo = entry.Context.GetService<IHiLoGenerator>();
            return EntityID.FromValue<ExpenseID>(hilo.NextID(entry.Metadata.Relational().TableName));
        }

        public override bool GeneratesTemporaryValues => false;
    }
}
