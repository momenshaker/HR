using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.ToTable("LeaveTypes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Code).IsRequired().HasMaxLength(20);
        builder.HasIndex(x => x.Code).IsUnique();

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.RequiresApproval).IsRequired();
        builder.Property(x => x.AnnualAllowanceDays).HasColumnType("decimal(5,2)");
        builder.Property(x => x.CarryOverDays).HasColumnType("decimal(5,2)");
    }
}

