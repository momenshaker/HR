using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class EngagementCampaignConfiguration : IEntityTypeConfiguration<EngagementCampaign>
{
    public void Configure(EntityTypeBuilder<EngagementCampaign> builder)
    {
        builder.ToTable("EngagementCampaigns");

        builder.HasKey(campaign => campaign.Id);
        builder.Property(campaign => campaign.Id).ValueGeneratedNever();

        builder.Property(campaign => campaign.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(campaign => campaign.Description)
            .HasMaxLength(2000);

        builder.Property(campaign => campaign.Channels)
            .HasMaxLength(200);

        builder.Property(campaign => campaign.TargetAudience)
            .HasMaxLength(200);

        builder.Property(campaign => campaign.OwnerId)
            .IsRequired();

        builder.Property(campaign => campaign.LaunchDateUtc)
            .HasColumnType("datetime2");

        builder.Property(campaign => campaign.EndDateUtc)
            .HasColumnType("datetime2");
    }
}
