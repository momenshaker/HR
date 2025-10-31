using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.ToTable("LeaveBalances");

        builder.HasKey(x => new { x.EmployeeId, x.LeaveTypeId, x.Year });

        builder.Property(x => x.Opening).HasColumnType("decimal(5,2)");
        builder.Property(x => x.Accrued).HasColumnType("decimal(5,2)");
        builder.Property(x => x.Taken).HasColumnType("decimal(5,2)");
        builder.Property(x => x.CarriedOver).HasColumnType("decimal(5,2)");

        builder.Property(x => x.RowVersion)
            .IsRowVersion()
            .IsConcurrencyToken();
    }
}

