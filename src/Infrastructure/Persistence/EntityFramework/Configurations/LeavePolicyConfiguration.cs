using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class LeavePolicyConfiguration : IEntityTypeConfiguration<LeavePolicy>
{
    public void Configure(EntityTypeBuilder<LeavePolicy> builder)
    {
        builder.ToTable("LeavePolicies");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.OrganizationId).IsRequired();
        builder.Property(x => x.LeaveTypeId).IsRequired();
        builder.Property(x => x.AccrualMethod).HasConversion<int>();
        builder.Property(x => x.DaysPerYear).HasColumnType("decimal(5,2)");
        builder.Property(x => x.CarryForwardAllowed).IsRequired();
        builder.Property(x => x.MaxCarryForwardDays).HasColumnType("decimal(5,2)");
        builder.Property(x => x.IsNegativeBalanceAllowed).IsRequired();

        builder.HasIndex(x => new { x.OrganizationId, x.LeaveTypeId }).IsUnique();
    }
}
