using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class PayrollItemConfiguration : IEntityTypeConfiguration<PayrollItem>
{
    public void Configure(EntityTypeBuilder<PayrollItem> builder)
    {
        builder.ToTable("PayrollItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property(i => i.Gross).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Deductions).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Net).HasColumnType("decimal(18,2)");

        builder.Property(i => i.Currency).IsRequired().HasMaxLength(3);

        // Store breakdown as JSON text in NVARCHAR(MAX)
        builder.Property(i => i.Breakdown);

        // The breakdown value object is persisted as JSON and should not be treated as an entity.
        builder.Ignore(i => i.BreakdownDetails);

        builder.Property(i => i.RowVersion).IsRowVersion();
    }
}
