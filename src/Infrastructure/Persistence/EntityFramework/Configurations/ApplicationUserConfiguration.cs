using HR.Domain.Entities;
using HR.Infrastructure.Security.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("AspNetUsers");

        builder.Property(user => user.CustomerId)
            .HasMaxLength(256);

        builder.HasIndex(user => user.EmployeeId)
            .IsUnique()
            .HasFilter("[EmployeeId] IS NOT NULL");

        // One-to-one between Employee and ApplicationUser via EmployeeId FK on user
        builder.HasOne<Employee>()
            .WithOne()
            .HasForeignKey<ApplicationUser>(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

