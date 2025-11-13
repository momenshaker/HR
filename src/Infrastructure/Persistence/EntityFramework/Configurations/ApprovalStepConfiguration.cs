using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class ApprovalStepConfiguration : IEntityTypeConfiguration<ApprovalStep>
{
    public void Configure(EntityTypeBuilder<ApprovalStep> builder)
    {
        builder.ToTable("LeaveApprovalSteps");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.LeaveRequestId).IsRequired();
        builder.Property(x => x.StepOrder).IsRequired();
        builder.Property(x => x.ApproverId).IsRequired();
        builder.Property(x => x.Status).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Comment).HasMaxLength(512);

        builder.HasIndex(x => new { x.LeaveRequestId, x.StepOrder }).IsUnique();
    }
}
