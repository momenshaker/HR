using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class DelegatedAuthorityConfiguration : IEntityTypeConfiguration<DelegatedAuthority>
{
    public void Configure(EntityTypeBuilder<DelegatedAuthority> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("DelegatedAuthorities");

        builder.HasKey(authority => authority.Id);

        builder.Property(authority => authority.AuthorityScope)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(authority => authority.Notes)
            .HasMaxLength(1000);

        builder.Property(authority => authority.ApprovalLimit)
            .HasColumnType("decimal(18,2)");

        builder.Property(authority => authority.IsRevoked)
            .HasDefaultValue(false);

        builder.HasIndex(authority => authority.GrantorEmployeeId);
        builder.HasIndex(authority => authority.DelegateEmployeeId);
    }
}
