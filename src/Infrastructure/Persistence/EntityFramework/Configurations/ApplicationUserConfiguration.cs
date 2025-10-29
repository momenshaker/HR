using HR.Infrastructure.Security.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(user => user.CustomerId)
            .IsRequired()
            .HasMaxLength(64)
            .HasDefaultValue("demo-tenant");

        builder.HasIndex(user => user.CustomerId);
    }
}
