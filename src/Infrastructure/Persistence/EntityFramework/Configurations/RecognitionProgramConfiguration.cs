using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.Infrastructure.Persistence.EntityFramework.Configurations;

internal sealed class RecognitionProgramConfiguration : IEntityTypeConfiguration<RecognitionProgram>
{
    public void Configure(EntityTypeBuilder<RecognitionProgram> builder)
    {
        builder.ToTable("RecognitionPrograms");

        builder.HasKey(program => program.Id);
        builder.Property(program => program.Id).ValueGeneratedNever();

        builder.Property(program => program.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(program => program.Description)
            .HasMaxLength(2000);

        builder.Property(program => program.Criteria)
            .HasMaxLength(2000);

        builder.Property(program => program.Reward)
            .HasMaxLength(1000);

        builder.Property(program => program.OwnerId)
            .IsRequired();

        builder.Property(program => program.CreatedAtUtc)
            .HasColumnType("datetime2");
    }
}
