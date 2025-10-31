using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
{
    public void Configure(EntityTypeBuilder<LeaveRequest> builder)
    {
        builder.ToTable("LeaveRequests");

        builder.HasKey(request => request.Id);
        builder.Property(request => request.Id).ValueGeneratedNever();

        builder.Property(request => request.EmployeeId)
            .IsRequired();

        builder.Property(request => request.LeaveType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(request => request.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(request => request.StartDate)
            .HasColumnType("date");

        builder.Property(request => request.EndDate)
            .HasColumnType("date");

        builder.Property(request => request.RequestedAtUtc)
            .HasColumnType("datetime2");

        builder.Property(request => request.DecisionAtUtc)
            .HasColumnType("datetime2");

        builder.Property(request => request.Reason)
            .HasMaxLength(1024);

        // Common analytics filters: employee and date range
        builder.HasIndex(request => request.EmployeeId);
        builder.HasIndex(request => new { request.StartDate, request.EndDate });
    }
}
