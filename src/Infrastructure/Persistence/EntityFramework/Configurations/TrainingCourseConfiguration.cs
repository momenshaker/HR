using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class TrainingCourseConfiguration : IEntityTypeConfiguration<TrainingCourse>
{
    public void Configure(EntityTypeBuilder<TrainingCourse> builder)
    {
        builder.ToTable("TrainingCourses");

        builder.HasKey(course => course.Id);
        builder.Property(course => course.Id).ValueGeneratedNever();

        builder.Property(course => course.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(course => course.Category)
            .HasMaxLength(100);

        builder.Property(course => course.Description)
            .HasMaxLength(2000);

        builder.Property(course => course.Instructor)
            .HasMaxLength(150);

        builder.Property(course => course.StartDate)
            .HasColumnType("date");

        builder.Property(course => course.EndDate)
            .HasColumnType("date");

        builder.Property(course => course.DeliveryMode)
            .HasMaxLength(100);

        builder.Property(course => course.SkillLevel)
            .HasMaxLength(20);

        builder.Property(course => course.CertificationCriteria)
            .HasMaxLength(1000);

        builder.Property(course => course.DurationHours)
            .HasDefaultValue(0);

        var converter = new ValueConverter<List<string>, string>(
            value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
            value => string.IsNullOrWhiteSpace(value)
                ? new List<string>()
                : (JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>()));

        var comparer = new ValueComparer<List<string>>(
            (left, right) => left.SequenceEqual(right, StringComparer.OrdinalIgnoreCase),
            value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode(StringComparison.OrdinalIgnoreCase))),
            value => value.ToList());

        builder.Property(course => course.CompetencyCodes)
            .HasConversion(converter)
            .Metadata.SetValueComparer(comparer);
    }
}
