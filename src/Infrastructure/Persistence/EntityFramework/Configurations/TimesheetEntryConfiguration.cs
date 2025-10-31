using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class TimesheetEntryConfiguration : IEntityTypeConfiguration<TimesheetEntry>
{
    public void Configure(EntityTypeBuilder<TimesheetEntry> builder)
    {
        builder.ToTable("TimesheetEntries");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DateUtc).HasColumnType("date").IsRequired();
        builder.Property(x => x.ProjectCode).HasMaxLength(50);
        builder.Property(x => x.TaskCode).HasMaxLength(50);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Hours).HasColumnType("decimal(5,2)").IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();

        builder.HasOne(x => x.Department)
            .WithMany()
            .HasForeignKey(x => x.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        // Common analytics filters: date range and department
        builder.HasIndex(x => x.DateUtc);
        builder.HasIndex(x => x.DepartmentId);
    }
}
