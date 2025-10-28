using System;
using HR.Infrastructure.Persistence.EntityFramework.Seeders;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Infrastructure.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class Add_Billing_Subscriptions_Audit_20251028 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BillingEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    BillingPhone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    AddressLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AddressLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    State = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    TrialEndsOn = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Actor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ActorEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Info"),
                    CorrelationId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Changes = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Metadata = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OccurredAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    BillingInterval = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    RenewalDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CancelledOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaidDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    Notes = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoices_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionEntitlements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlanCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FeatureKey = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    MeasurementUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    EffectiveFrom = table.Column<DateOnly>(type: "date", nullable: true),
                    EffectiveTo = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionEntitlements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionEntitlements_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsageCounters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MetricKey = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MeasurementUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrentValue = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Limit = table.Column<decimal>(type: "decimal(18,4)", nullable: true),
                    PeriodStart = table.Column<DateOnly>(type: "date", nullable: false),
                    PeriodEnd = table.Column<DateOnly>(type: "date", nullable: false),
                    LastResetAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsageCounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsageCounters_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CustomerId",
                table: "AuditLogs",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OccurredAtUtc",
                table: "AuditLogs",
                column: "OccurredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_BillingEmail",
                table: "Customers",
                column: "BillingEmail");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_SubscriptionId",
                table: "Invoices",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionEntitlements_SubscriptionId",
                table: "SubscriptionEntitlements",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionEntitlements_PlanCode_FeatureKey_SubscriptionId",
                table: "SubscriptionEntitlements",
                columns: new[] { "PlanCode", "FeatureKey", "SubscriptionId" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_CustomerId_PlanCode",
                table: "Subscriptions",
                columns: new[] { "CustomerId", "PlanCode" });

            migrationBuilder.CreateIndex(
                name: "IX_UsageCounters_SubscriptionId_MetricKey_PeriodStart_PeriodEnd",
                table: "UsageCounters",
                columns: new[] { "SubscriptionId", "MetricKey", "PeriodStart", "PeriodEnd" },
                unique: true);

            migrationBuilder.InsertData(
                table: "SubscriptionEntitlements",
                columns: new[]
                {
                    "Id",
                    "SubscriptionId",
                    "PlanCode",
                    "FeatureKey",
                    "DisplayName",
                    "Description",
                    "MeasurementUnit",
                    "Quantity",
                    "IsEnabled",
                    "EffectiveFrom",
                    "EffectiveTo",
                    "CreatedAtUtc"
                },
                values: new object[,]
                {
                    {
                        PlanCatalogSeeder.StarterSeatEntitlementId,
                        null,
                        PlanCatalogSeeder.StarterPlanCode,
                        "core.users",
                        "Active employee seats",
                        "Maximum number of active employee records for the Starter plan.",
                        "seats",
                        25,
                        true,
                        new DateOnly(2025, 1, 1),
                        null,
                        new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero)
                    },
                    {
                        PlanCatalogSeeder.StarterStorageEntitlementId,
                        null,
                        PlanCatalogSeeder.StarterPlanCode,
                        "storage.documents",
                        "Document storage",
                        "Shared storage capacity for uploaded documents.",
                        "GB",
                        50,
                        true,
                        new DateOnly(2025, 1, 1),
                        null,
                        new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero)
                    },
                    {
                        PlanCatalogSeeder.ProfessionalSeatEntitlementId,
                        null,
                        PlanCatalogSeeder.ProfessionalPlanCode,
                        "core.users",
                        "Active employee seats",
                        "Maximum number of active employee records for the Professional plan.",
                        "seats",
                        250,
                        true,
                        new DateOnly(2025, 1, 1),
                        null,
                        new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero)
                    },
                    {
                        PlanCatalogSeeder.ProfessionalAutomationEntitlementId,
                        null,
                        PlanCatalogSeeder.ProfessionalPlanCode,
                        "automation.workflows",
                        "Automation workflows",
                        "Automated workflow executions per month.",
                        "runs",
                        500,
                        true,
                        new DateOnly(2025, 1, 1),
                        null,
                        new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero)
                    },
                    {
                        PlanCatalogSeeder.EnterpriseSeatEntitlementId,
                        null,
                        PlanCatalogSeeder.EnterprisePlanCode,
                        "core.users",
                        "Active employee seats",
                        "Maximum number of active employee records for the Enterprise plan.",
                        "seats",
                        null,
                        true,
                        new DateOnly(2025, 1, 1),
                        null,
                        new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero)
                    },
                    {
                        PlanCatalogSeeder.EnterpriseSuccessEntitlementId,
                        null,
                        PlanCatalogSeeder.EnterprisePlanCode,
                        "success.manager",
                        "Dedicated success manager",
                        "Assigned enterprise customer success manager with quarterly reviews.",
                        string.Empty,
                        1,
                        true,
                        new DateOnly(2025, 1, 1),
                        null,
                        new DateTimeOffset(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified), TimeSpan.Zero)
                    }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "SubscriptionEntitlements");

            migrationBuilder.DropTable(
                name: "UsageCounters");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
