using HR.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HR.Infrastructure.Persistence.EntityFramework.Seeders;

internal static class PlanCatalogSeeder
{
    internal const string StarterPlanCode = "starter";
    internal const string ProfessionalPlanCode = "professional";
    internal const string EnterprisePlanCode = "enterprise";

    internal static readonly Guid StarterSeatEntitlementId = new("4cf0c924-9328-4c0a-9ec2-0a0cb0c0a63f");
    internal static readonly Guid StarterStorageEntitlementId = new("c8a4742d-0275-4c9a-a2ba-c8dfd6de45ad");
    internal static readonly Guid ProfessionalSeatEntitlementId = new("b166d8de-9a48-45d4-81a5-07fdfb5b467b");
    internal static readonly Guid ProfessionalAutomationEntitlementId = new("3ea77932-f7fa-4df7-b362-5d8b9aafef7f");
    internal static readonly Guid EnterpriseSeatEntitlementId = new("9ec8325a-463e-4c1d-9fb3-0bd83c48ebb2");
    internal static readonly Guid EnterpriseSuccessEntitlementId = new("b2b9376d-9866-4fd9-a012-39c0fd305aa3");

    private static readonly DateOnly CatalogEffectiveFrom = new(2025, 1, 1);
    private static readonly DateTimeOffset CatalogCreatedAtUtc = new(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

    internal static void Seed(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<SubscriptionEntitlement>().HasData(
            new SubscriptionEntitlement
            {
                Id = StarterSeatEntitlementId,
                PlanCode = StarterPlanCode,
                FeatureKey = "core.users",
                DisplayName = "Active employee seats",
                Description = "Maximum number of active employee records for the Starter plan.",
                MeasurementUnit = "seats",
                Quantity = 25,
                IsEnabled = true,
                EffectiveFrom = CatalogEffectiveFrom,
                CreatedAtUtc = CatalogCreatedAtUtc
            },
            new SubscriptionEntitlement
            {
                Id = StarterStorageEntitlementId,
                PlanCode = StarterPlanCode,
                FeatureKey = "storage.documents",
                DisplayName = "Document storage",
                Description = "Shared storage capacity for uploaded documents.",
                MeasurementUnit = "GB",
                Quantity = 50,
                IsEnabled = true,
                EffectiveFrom = CatalogEffectiveFrom,
                CreatedAtUtc = CatalogCreatedAtUtc
            },
            new SubscriptionEntitlement
            {
                Id = ProfessionalSeatEntitlementId,
                PlanCode = ProfessionalPlanCode,
                FeatureKey = "core.users",
                DisplayName = "Active employee seats",
                Description = "Maximum number of active employee records for the Professional plan.",
                MeasurementUnit = "seats",
                Quantity = 250,
                IsEnabled = true,
                EffectiveFrom = CatalogEffectiveFrom,
                CreatedAtUtc = CatalogCreatedAtUtc
            },
            new SubscriptionEntitlement
            {
                Id = ProfessionalAutomationEntitlementId,
                PlanCode = ProfessionalPlanCode,
                FeatureKey = "automation.workflows",
                DisplayName = "Automation workflows",
                Description = "Automated workflow executions per month.",
                MeasurementUnit = "runs",
                Quantity = 500,
                IsEnabled = true,
                EffectiveFrom = CatalogEffectiveFrom,
                CreatedAtUtc = CatalogCreatedAtUtc
            },
            new SubscriptionEntitlement
            {
                Id = EnterpriseSeatEntitlementId,
                PlanCode = EnterprisePlanCode,
                FeatureKey = "core.users",
                DisplayName = "Active employee seats",
                Description = "Maximum number of active employee records for the Enterprise plan.",
                MeasurementUnit = "seats",
                Quantity = null,
                IsEnabled = true,
                EffectiveFrom = CatalogEffectiveFrom,
                CreatedAtUtc = CatalogCreatedAtUtc
            },
            new SubscriptionEntitlement
            {
                Id = EnterpriseSuccessEntitlementId,
                PlanCode = EnterprisePlanCode,
                FeatureKey = "success.manager",
                DisplayName = "Dedicated success manager",
                Description = "Assigned enterprise customer success manager with quarterly reviews.",
                MeasurementUnit = string.Empty,
                Quantity = 1,
                IsEnabled = true,
                EffectiveFrom = CatalogEffectiveFrom,
                CreatedAtUtc = CatalogCreatedAtUtc
            }
        );
    }
}
