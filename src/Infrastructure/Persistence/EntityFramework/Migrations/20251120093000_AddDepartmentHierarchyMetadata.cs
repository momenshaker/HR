using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.Infrastructure.Persistence.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentHierarchyMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Organizations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Organizations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Employees",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Departments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Departments",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Departments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.DropIndex(
                name: "IX_Departments_OrganizationId_Code",
                table: "Departments");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_OrganizationId_ParentDepartmentId_Name",
                table: "Departments",
                columns: new[] { "OrganizationId", "ParentDepartmentId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Path",
                table: "Departments",
                column: "Path");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_OrganizationId_Code",
                table: "Departments",
                columns: new[] { "OrganizationId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL AND [Code] <> ''");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Name",
                table: "Organizations",
                column: "Name",
                unique: true);

            migrationBuilder.InsertData(
                table: "Organizations",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "Description", "IsActive", "Name", "UpdatedAtUtc" },
                values: new object[]
                {
                    new Guid("8d741596-7f48-4a44-9a9b-5d4d78f3dc9a"),
                    "ACME",
                    new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    "Acme Corp demo organization",
                    true,
                    "Acme Corp",
                    null
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[]
                {
                    "Id", "OrganizationId", "ParentDepartmentId", "Name", "Code", "Path", "Level",
                    "Branch", "Location", "Description", "IsActive", "CreatedAtUtc", "UpdatedAtUtc"
                },
                values: new object[,]
                {
                    {
                        new Guid("5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90"),
                        new Guid("8d741596-7f48-4a44-9a9b-5d4d78f3dc9a"),
                        null,
                        "Head Office",
                        "HQ",
                        "/org/8d741596-7f48-4a44-9a9b-5d4d78f3dc9a/dept/5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90",
                        0,
                        "Global",
                        "New York",
                        "Corporate headquarters",
                        true,
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        null
                    },
                    {
                        new Guid("2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8"),
                        new Guid("8d741596-7f48-4a44-9a9b-5d4d78f3dc9a"),
                        new Guid("5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90"),
                        "Engineering",
                        "ENG",
                        "/org/8d741596-7f48-4a44-9a9b-5d4d78f3dc9a/dept/5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90/2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8",
                        1,
                        "Product",
                        "Seattle",
                        "Platform and application engineering",
                        true,
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        null
                    },
                    {
                        new Guid("3c7d4f9a-0f8b-4f33-9ed9-6bff8e75b5f2"),
                        new Guid("8d741596-7f48-4a44-9a9b-5d4d78f3dc9a"),
                        new Guid("2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8"),
                        "Platform",
                        "PLAT",
                        "/org/8d741596-7f48-4a44-9a9b-5d4d78f3dc9a/dept/5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90/2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8/3c7d4f9a-0f8b-4f33-9ed9-6bff8e75b5f2",
                        2,
                        "Technology",
                        "Seattle",
                        "Platform services and infrastructure",
                        true,
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        null
                    },
                    {
                        new Guid("13d7a3fd-91c0-44d3-9d63-7f53820f9bde"),
                        new Guid("8d741596-7f48-4a44-9a9b-5d4d78f3dc9a"),
                        new Guid("2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8"),
                        "Applications",
                        "APPS",
                        "/org/8d741596-7f48-4a44-9a9b-5d4d78f3dc9a/dept/5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90/2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8/13d7a3fd-91c0-44d3-9d63-7f53820f9bde",
                        2,
                        "Product",
                        "Austin",
                        "Customer-facing application development",
                        true,
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        null
                    },
                    {
                        new Guid("dc05d230-9a87-4c5c-a8cf-1e1422ecf7b2"),
                        new Guid("8d741596-7f48-4a44-9a9b-5d4d78f3dc9a"),
                        new Guid("5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90"),
                        "HR",
                        "HR",
                        "/org/8d741596-7f48-4a44-9a9b-5d4d78f3dc9a/dept/5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90/dc05d230-9a87-4c5c-a8cf-1e1422ecf7b2",
                        1,
                        "Corporate",
                        "New York",
                        "People operations",
                        true,
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        null
                    }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[]
                {
                    "Id", "Email", "EmploymentEndDate", "EmploymentStartDate", "FirstName", "JobArchitectureCareerTrack",
                    "JobArchitectureJobCode", "JobArchitectureJobFamily", "JobArchitectureJobFunction", "JobArchitectureJobLevel",
                    "JobTitle", "LastName", "CreatedAtUtc", "DateOfBirth", "IsActive"
                },
                values: new object[,]
                {
                    {
                        new Guid("df15d0d5-7b31-4a2c-924f-4da55b1fb677"),
                        "alice.johnson@acme.test",
                        null,
                        new DateTime(2020, 1, 6),
                        "Alice",
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        "Director of Engineering",
                        "Johnson",
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        true
                    },
                    {
                        new Guid("dca9c1f7-5fbc-4c6c-8fd0-9be0384fe760"),
                        "bob.smith@acme.test",
                        null,
                        new DateTime(2020, 4, 6),
                        "Bob",
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        "Senior Platform Engineer",
                        "Smith",
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        true
                    },
                    {
                        new Guid("c0d7683e-8e3d-4f6d-bf16-b0cb02f1455e"),
                        "carol.lee@acme.test",
                        null,
                        new DateTime(2020, 7, 6),
                        "Carol",
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        "Applications Lead",
                        "Lee",
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        true
                    },
                    {
                        new Guid("0b90b2f9-2378-4e8f-9b77-4d56e0dcd8ec"),
                        "david.patel@acme.test",
                        null,
                        new DateTime(2020, 3, 6),
                        "David",
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        "HR Business Partner",
                        "Patel",
                        new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        null,
                        true
                    }
                });

            migrationBuilder.InsertData(
                table: "EmployeeDepartments",
                columns: new[] { "EmployeeId", "DepartmentId", "IsPrimary" },
                values: new object[,]
                {
                    { new Guid("df15d0d5-7b31-4a2c-924f-4da55b1fb677"), new Guid("2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8"), true },
                    { new Guid("df15d0d5-7b31-4a2c-924f-4da55b1fb677"), new Guid("3c7d4f9a-0f8b-4f33-9ed9-6bff8e75b5f2"), false },
                    { new Guid("dca9c1f7-5fbc-4c6c-8fd0-9be0384fe760"), new Guid("3c7d4f9a-0f8b-4f33-9ed9-6bff8e75b5f2"), true },
                    { new Guid("dca9c1f7-5fbc-4c6c-8fd0-9be0384fe760"), new Guid("13d7a3fd-91c0-44d3-9d63-7f53820f9bde"), false },
                    { new Guid("c0d7683e-8e3d-4f6d-bf16-b0cb02f1455e"), new Guid("13d7a3fd-91c0-44d3-9d63-7f53820f9bde"), true },
                    { new Guid("c0d7683e-8e3d-4f6d-bf16-b0cb02f1455e"), new Guid("2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8"), false },
                    { new Guid("0b90b2f9-2378-4e8f-9b77-4d56e0dcd8ec"), new Guid("dc05d230-9a87-4c5c-a8cf-1e1422ecf7b2"), true },
                    { new Guid("0b90b2f9-2378-4e8f-9b77-4d56e0dcd8ec"), new Guid("5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90"), false }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmployeeDepartments",
                keyColumns: new[] { "EmployeeId", "DepartmentId" },
                keyValues: new object[] { new Guid("0b90b2f9-2378-4e8f-9b77-4d56e0dcd8ec"), new Guid("5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90") });

            migrationBuilder.DeleteData(
                table: "EmployeeDepartments",
                keyColumns: new[] { "EmployeeId", "DepartmentId" },
                keyValues: new object[] { new Guid("0b90b2f9-2378-4e8f-9b77-4d56e0dcd8ec"), new Guid("dc05d230-9a87-4c5c-a8cf-1e1422ecf7b2") });

            migrationBuilder.DeleteData(
                table: "EmployeeDepartments",
                keyColumns: new[] { "EmployeeId", "DepartmentId" },
                keyValues: new object[] { new Guid("c0d7683e-8e3d-4f6d-bf16-b0cb02f1455e"), new Guid("2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8") });

            migrationBuilder.DeleteData(
                table: "EmployeeDepartments",
                keyColumns: new[] { "EmployeeId", "DepartmentId" },
                keyValues: new object[] { new Guid("c0d7683e-8e3d-4f6d-bf16-b0cb02f1455e"), new Guid("13d7a3fd-91c0-44d3-9d63-7f53820f9bde") });

            migrationBuilder.DeleteData(
                table: "EmployeeDepartments",
                keyColumns: new[] { "EmployeeId", "DepartmentId" },
                keyValues: new object[] { new Guid("dca9c1f7-5fbc-4c6c-8fd0-9be0384fe760"), new Guid("13d7a3fd-91c0-44d3-9d63-7f53820f9bde") });

            migrationBuilder.DeleteData(
                table: "EmployeeDepartments",
                keyColumns: new[] { "EmployeeId", "DepartmentId" },
                keyValues: new object[] { new Guid("dca9c1f7-5fbc-4c6c-8fd0-9be0384fe760"), new Guid("3c7d4f9a-0f8b-4f33-9ed9-6bff8e75b5f2") });

            migrationBuilder.DeleteData(
                table: "EmployeeDepartments",
                keyColumns: new[] { "EmployeeId", "DepartmentId" },
                keyValues: new object[] { new Guid("df15d0d5-7b31-4a2c-924f-4da55b1fb677"), new Guid("3c7d4f9a-0f8b-4f33-9ed9-6bff8e75b5f2") });

            migrationBuilder.DeleteData(
                table: "EmployeeDepartments",
                keyColumns: new[] { "EmployeeId", "DepartmentId" },
                keyValues: new object[] { new Guid("df15d0d5-7b31-4a2c-924f-4da55b1fb677"), new Guid("2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8") });

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("0b90b2f9-2378-4e8f-9b77-4d56e0dcd8ec"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("c0d7683e-8e3d-4f6d-bf16-b0cb02f1455e"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("dca9c1f7-5fbc-4c6c-8fd0-9be0384fe760"));

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("df15d0d5-7b31-4a2c-924f-4da55b1fb677"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("13d7a3fd-91c0-44d3-9d63-7f53820f9bde"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("2ff4b6f1-6f4e-4de0-85ed-2f1b83f6f0d8"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("3c7d4f9a-0f8b-4f33-9ed9-6bff8e75b5f2"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("5b0d38f7-8b37-4f33-9b77-39f8e8e5ef90"));

            migrationBuilder.DeleteData(
                table: "Departments",
                keyColumn: "Id",
                keyValue: new Guid("dc05d230-9a87-4c5c-a8cf-1e1422ecf7b2"));

            migrationBuilder.DeleteData(
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("8d741596-7f48-4a44-9a9b-5d4d78f3dc9a"));

            migrationBuilder.DropIndex(
                name: "IX_Departments_OrganizationId_ParentDepartmentId_Name",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_Path",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_OrganizationId_Code",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Organizations_Name",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Departments");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_OrganizationId_Code",
                table: "Departments",
                columns: new[] { "OrganizationId", "Code" },
                unique: true);
        }
    }
}
