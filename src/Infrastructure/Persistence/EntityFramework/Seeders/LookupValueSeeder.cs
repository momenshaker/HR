using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Seeders;

/// <summary>
///     Seeds canonical lookup values used by the UI out of the box.
/// </summary>
public static class LookupValueSeeder
{
    private static readonly DateTime SeedTimestamp = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static void Seed(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<LookupValue>().HasData(
            Create("ea6e39b4-7911-43d8-aba5-5e1846f88874", "branch", "HEADQUARTERS", "Headquarters", 1),
            Create("f4aa9d9b-5925-48c6-ba70-220bb9260c6a", "branch", "FIELD", "Field", 2),
            Create("719b7a47-109f-431d-9418-9d3cdb377c7a", "branch", "REGIONAL_OFFICE", "Regional Office", 3),
            Create("771cb91a-a6ae-453f-bdc9-f4baf22fc436", "businessUnit", "CORPORATE", "Corporate", 1),
            Create("8e6db0b5-a580-46c8-9a00-c4a81b448f3a", "businessUnit", "PRODUCT", "Product", 2),
            Create("37aa74bb-f001-4ecc-bf7a-b35057c35f0a", "businessUnit", "SERVICES", "Services", 3),
            Create("2f48a4d1-3519-4b02-86c3-1e77fec77bf0", "businessUnit", "OPERATIONS", "Operations", 4),
            Create("86694dbe-a50f-4330-a794-cf9625528ac3", "operatingHours", "DAY", "Day", 1),
            Create("36d48abe-4ade-4189-af71-03e38526f06d", "operatingHours", "SWING", "Swing", 2),
            Create("972e6450-17e9-48ee-ac9a-9041d6d8fb97", "operatingHours", "NIGHT", "Night", 3),
            Create("dfce0848-757f-4ebd-990a-f6861a25b981", "operatingHours", "24_7", "24/7", 4),
            Create("02029a6d-a17d-4773-92ba-ac63955e8a17", "industry", "TECHNOLOGY", "Technology", 1),
            Create("e03d5548-0998-4b74-836f-ace82fd812f3", "industry", "RETAIL", "Retail", 2),
            Create("17f974c2-ed4d-454a-b592-86e48bf74e3f", "industry", "FINANCE", "Finance", 3),
            Create("5c1e0e99-540c-4dea-a361-b49d3e1c2ec5", "industry", "HEALTHCARE", "Healthcare", 4),
            Create("44c8cf0e-ea28-4f0f-ae7c-990f361776a5", "region", "NORTH_AMERICA", "North America", 1),
            Create("fce559ca-6ff7-4876-a770-c24126aef993", "region", "EMEA", "EMEA", 2),
            Create("c785c01d-84a4-4923-a6e7-fa59b15f63b4", "region", "APAC", "APAC", 3),
            Create("a88beff5-2332-42f8-9a96-0550c1d2364a", "region", "LATAM", "LATAM", 4),
            Create("3c2aba9e-a562-4f72-a564-a85f25689272", "timeZone", "UTC", "UTC", 1),
            Create("c8c741e5-fc1b-4895-bc98-6ae8607afdcf", "timeZone", "AMERICA_NEW_YORK", "America/New_York", 2),
            Create("fd228009-14bb-46e0-ade8-aabeb82f8abd", "timeZone", "EUROPE_LONDON", "Europe/London", 3),
            Create("383c0c64-3592-475f-8b31-cd5b5cfcc146", "timeZone", "ASIA_SINGAPORE", "Asia/Singapore", 4),
            Create("5897d8d7-8eb8-4612-a123-3f8d7f5d24c8", "leaveType", "VACATION", "Vacation", 1),
            Create("65a1f6c3-9e6a-4d5f-901f-3b4e7c1f0e3f", "leaveType", "SICK", "Sick", 2),
            Create("b6b1c9e2-3d44-4c01-9f21-a9c9475c6f4f", "leaveType", "PERSONAL", "Personal", 3)
        );
    }

    private static LookupValue Create(
        string id,
        string category,
        string code,
        string displayName,
        int sortOrder)
    {
        return new LookupValue
        {
            Id = new Guid(id),
            Category = category,
            Code = code,
            DisplayName = displayName,
            SortOrder = sortOrder,
            IsActive = true,
            CreatedAtUtc = SeedTimestamp,
            UpdatedAtUtc = SeedTimestamp
        };
    }
}
