using System;
using System.Linq;
using System.Threading.Tasks;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence.EntityFramework;
using HR.Infrastructure.Persistence.EntityFramework.Seeders;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HR.UnitTests.Infrastructure.Persistence.EntityFramework;

public sealed class BillingEntityConfigurationTests
{
    [Fact]
    public void CustomerConfiguration_ShouldMapToCustomersTableWithDefaults()
    {
        using var context = CreateInMemoryContext();

        var entityType = context.Model.FindEntityType(typeof(Customer));
        Assert.NotNull(entityType);
        Assert.Equal("Customers", entityType!.GetTableName());

        var statusProperty = entityType.FindProperty(nameof(Customer.Status));
        Assert.NotNull(statusProperty);
        Assert.Equal(50, statusProperty!.GetMaxLength());
        Assert.Equal("Active", statusProperty.GetDefaultValue());

        var createdProperty = entityType.FindProperty(nameof(Customer.CreatedAtUtc));
        Assert.NotNull(createdProperty);
        Assert.Equal("CURRENT_TIMESTAMP", createdProperty!.GetDefaultValueSql());
    }

    [Fact]
    public void SubscriptionConfiguration_ShouldConfigureCascadeToCustomer()
    {
        using var context = CreateInMemoryContext();

        var entityType = context.Model.FindEntityType(typeof(Subscription));
        Assert.NotNull(entityType);

        var customerForeignKey = Assert.Single(entityType!.GetForeignKeys(), fk => fk.PrincipalEntityType.ClrType == typeof(Customer));
        Assert.Equal(DeleteBehavior.Cascade, customerForeignKey.DeleteBehavior);

        var autoRenewProperty = entityType.FindProperty(nameof(Subscription.AutoRenew));
        Assert.NotNull(autoRenewProperty);
        Assert.Equal(true, autoRenewProperty!.GetDefaultValue());
    }

    [Fact]
    public void UsageCounterConfiguration_ShouldCreateUniqueMetricIndex()
    {
        using var context = CreateInMemoryContext();

        var entityType = context.Model.FindEntityType(typeof(UsageCounter));
        Assert.NotNull(entityType);

        var uniqueIndex = entityType!.GetIndexes()
            .Single(index => index.IsUnique && index.Properties.Select(property => property.Name)
                .SequenceEqual(new[]
                {
                    nameof(UsageCounter.SubscriptionId),
                    nameof(UsageCounter.MetricKey),
                    nameof(UsageCounter.PeriodStart),
                    nameof(UsageCounter.PeriodEnd)
                }));

        Assert.True(uniqueIndex.IsUnique);
    }

    [Fact]
    public async Task PlanCatalogSeeder_ShouldSeedStarterProfessionalAndEnterprisePlans()
    {
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<HrDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var context = new HrDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
        }

        await using var verificationContext = new HrDbContext(options);

        var planCounts = await verificationContext.SubscriptionEntitlements
            .Where(entitlement => entitlement.SubscriptionId == null)
            .GroupBy(entitlement => entitlement.PlanCode)
            .Select(group => new { PlanCode = group.Key, Count = group.Count() })
            .ToListAsync();

        Assert.Equal(3, planCounts.Count);
        Assert.Contains(planCounts, plan => plan.PlanCode == PlanCatalogSeeder.StarterPlanCode);
        Assert.Contains(planCounts, plan => plan.PlanCode == PlanCatalogSeeder.ProfessionalPlanCode);
        Assert.Contains(planCounts, plan => plan.PlanCode == PlanCatalogSeeder.EnterprisePlanCode);
    }

    private static HrDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<HrDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new HrDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
