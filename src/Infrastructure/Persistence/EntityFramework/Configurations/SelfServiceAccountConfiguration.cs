using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class SelfServiceAccountConfiguration : IEntityTypeConfiguration<SelfServiceAccount>
{
    public void Configure(EntityTypeBuilder<SelfServiceAccount> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("SelfServiceAccounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.Email)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(account => account.OAuthProvider)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(account => account.ExternalIdentifier)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(account => account.IsMfaEnabled)
            .HasDefaultValue(false);

        builder.Property(account => account.IsLocked)
            .HasDefaultValue(false);

        var converter = new ValueConverter<List<string>, string>(
            value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            value => string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>());

        var comparer = new ValueComparer<List<string>>(
            (left, right) => left.SequenceEqual(right, StringComparer.OrdinalIgnoreCase),
            value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, StringComparer.OrdinalIgnoreCase.GetHashCode(item))),
            value => value.ToList());

        builder.Property(account => account.FeatureAccess)
            .HasConversion(converter)
            .Metadata.SetValueComparer(comparer);

        builder.HasIndex(account => account.EmployeeId)
            .IsUnique();

        builder.HasIndex(account => account.Email)
            .IsUnique();
    }
}
