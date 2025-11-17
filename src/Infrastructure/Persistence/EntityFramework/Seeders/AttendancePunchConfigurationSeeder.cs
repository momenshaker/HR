using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Seeders;

public static class AttendancePunchConfigurationSeeder
{
    public static readonly Guid ClockInId = new("a221292f-5ad8-4427-834a-5d9b6e4e9d2f");
    public static readonly Guid ClockOutId = new("b5ad6f5e-7d26-414c-bdc4-0f69c1e7b6f6");
    public static readonly Guid BreakStartId = new("cde8f618-5a57-4c90-9a8d-3b6a3a2f9c1d");
    public static readonly Guid BreakEndId = new("d3f8e26d-0f4b-4b28-bcbb-7a8c6f2e1c58");

    public static IReadOnlyCollection<AttendancePunchConfiguration> GetSeedData()
    {
        return new[]
        {
            new AttendancePunchConfiguration
            {
                Id = ClockInId,
                PunchType = "ClockIn",
                DisplayName = "Clock In",
                Description = "Employee clock-in punch.",
                SortOrder = 1,
                IsActive = true
            },
            new AttendancePunchConfiguration
            {
                Id = ClockOutId,
                PunchType = "ClockOut",
                DisplayName = "Clock Out",
                Description = "Employee clock-out punch.",
                SortOrder = 2,
                IsActive = true
            },
            new AttendancePunchConfiguration
            {
                Id = BreakStartId,
                PunchType = "BreakStart",
                DisplayName = "Break Start",
                Description = "Start of a break period.",
                SortOrder = 3,
                IsActive = true
            },
            new AttendancePunchConfiguration
            {
                Id = BreakEndId,
                PunchType = "BreakEnd",
                DisplayName = "Break End",
                Description = "End of a break period.",
                SortOrder = 4,
                IsActive = true
            }
        };
    }

    public static void Seed(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<AttendancePunchConfiguration>().HasData(GetSeedData());
    }
}
