using System;
using System.Linq;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _repositoryMock = new();
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock = new();
    private readonly Mock<ISelfServiceAccountService> _selfServiceAccountServiceMock = new();
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _sut = new EmployeeService(
            _repositoryMock.Object,
            _departmentRepositoryMock.Object,
            _selfServiceAccountServiceMock.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsEmployeesFromRepository()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var employees = new List<Employee>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                JobTitle = "HR Specialist",
                Departments = new List<EmployeeDepartment>
                {
                    new() { DepartmentId = departmentId, IsPrimary = true }
                }
            }
        };

        _repositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        // Act
        var result = await _sut.GetAsync(CancellationToken.None);

        // Assert
        Assert.Single(result);
        var dto = result.Single();
        Assert.Equal(employees[0].Id, dto.Id);
        Assert.Equal(employees[0].FirstName, dto.FirstName);
        Assert.Equal(departmentId, dto.PrimaryDepartmentId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEmployeeNotFound_ReturnsNull()
    {
        // Arrange
        _repositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByDepartmentAsync_WhenDepartmentBelongsToOrganization_ReturnsEmployees()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "Dept",
            LastName = "Employee",
            Email = "dept.employee@example.com",
            EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            Departments = new List<EmployeeDepartment>
            {
                new() { DepartmentId = departmentId, IsPrimary = true }
            }
        };

        _departmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Department { Id = departmentId, OrganizationId = organizationId, Name = "Dept", Code = "D1" });

        _repositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Employee> { employee });

        // Act
        var result = await _sut.GetByDepartmentAsync(organizationId, departmentId, CancellationToken.None);

        // Assert
        var dto = Assert.Single(result);
        Assert.Equal(employee.Id, dto.Id);
        Assert.Equal(departmentId, dto.PrimaryDepartmentId);
    }

    [Fact]
    public async Task GetByDepartmentAsync_WhenDepartmentMissingOrOutsideOrganization_ReturnsEmpty()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();

        _departmentRepositoryMock
            .Setup(repo => repo.GetByIdAsync(departmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Department { Id = departmentId, OrganizationId = Guid.NewGuid(), Name = "Other", Code = "OTH" });

        // Act
        var result = await _sut.GetByDepartmentAsync(organizationId, departmentId, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_MapsRequestAndPersistsEmployee()
    {
        // Arrange
        var departmentId = Guid.NewGuid();
        var secondaryDepartmentId = Guid.NewGuid();
        var profileDocumentId = Guid.NewGuid();
        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            JobTitle = "Software Engineer",
            PhoneNumber = " +1 555 0123 ",
            EmploymentType = "FullTime",
            DepartmentAssignment = new EmployeeDepartmentAssignmentRequest
            {
                PrimaryDepartmentId = departmentId,
                SecondaryDepartmentIds = new[] { secondaryDepartmentId }
            },
            JobArchitecture = new EmployeeJobArchitectureRequest
            {
                JobFamily = "Engineering",
                JobFunction = "Software",
                JobLevel = "L3",
                JobCode = "ENG-III",
                CareerTrack = "Individual Contributor"
            },
            Contracts = new[]
            {
                new EmploymentContractRequest
                {
                    ContractType = "Permanent",
                    ContractNumber = "CN-001",
                    Status = "Active",
                    EffectiveFrom = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30)),
                    WorkLocation = "London",
                    CompensationCurrency = "GBP",
                    AnnualCompensation = 85000m,
                    Notes = "Initial hire",
                }
            },
            ComplianceDocuments = new[]
            {
                new EmployeeComplianceDocumentRequest
                {
                    DocumentType = "Passport",
                    ReferenceNumber = "123456789",
                    Status = "Verified",
                    IssuedOn = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-5)),
                    ExpiresOn = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(5)),
                    StoragePath = "/docs/passport.pdf"
                }
            }
            ,
            ProfileDocuments = new[]
            {
                new EmployeeProfileDocumentRequest
                {
                    Id = profileDocumentId,
                    FileName = "profile.pdf",
                    StoragePath = "/files/profile.pdf",
                    Description = "Profile brief",
                    ContentType = "application/pdf",
                    UploadedAtUtc = DateTimeOffset.UtcNow
                }
            }
        };

        Employee? persistedEmployee = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((employee, _) => persistedEmployee = employee)
            .ReturnsAsync(() => persistedEmployee!);

        CreateSelfServiceAccountRequest? capturedAccountRequest = null;
        _selfServiceAccountServiceMock
            .Setup(service => service.CreateAsync(It.IsAny<CreateSelfServiceAccountRequest>(), It.IsAny<CancellationToken>()))
            .Callback<CreateSelfServiceAccountRequest, CancellationToken>((accountRequest, _) =>
            {
                capturedAccountRequest = accountRequest;
            })
            .ReturnsAsync(new SelfServiceAccountDto(
                Guid.NewGuid(),
                Guid.Empty,
                request.Email,
                "Local",
                request.Email,
                false,
                false,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow,
                null,
                Array.Empty<string>()));

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persistedEmployee);
        Assert.Equal(request.JobArchitecture!.JobFamily, persistedEmployee!.JobArchitecture.JobFamily);
        Assert.Single(persistedEmployee.Contracts);
        Assert.Single(persistedEmployee.ComplianceDocuments);
        Assert.Equal(departmentId, persistedEmployee.PrimaryDepartmentId);
        Assert.Contains(secondaryDepartmentId, persistedEmployee.DepartmentIds);

        Assert.Equal(request.JobArchitecture.JobFamily, result.JobArchitecture.JobFamily);
        Assert.Single(result.Contracts);
        Assert.Single(result.ComplianceDocuments);
        Assert.Equal(departmentId, result.PrimaryDepartmentId);
        Assert.Equal("+1 555 0123", persistedEmployee!.PhoneNumber);
        Assert.Equal("FullTime", persistedEmployee.EmploymentType);
        Assert.Single(persistedEmployee.ProfileDocuments);
        var profileDocument = persistedEmployee.ProfileDocuments.Single();
        Assert.Equal(profileDocumentId, profileDocument.Id);
        Assert.Equal("profile.pdf", profileDocument.FileName);
        Assert.Equal("application/pdf", profileDocument.ContentType);
        Assert.NotNull(capturedAccountRequest);
        Assert.Equal(persistedEmployee.Id, capturedAccountRequest!.EmployeeId);
        Assert.Equal("john.smith@example.com", capturedAccountRequest.Email);
        Assert.Equal("Local", capturedAccountRequest.OAuthProvider);
        Assert.Equal("john.smith@example.com", capturedAccountRequest.ExternalIdentifier);
        Assert.Equal(new[] { "Attendance", "Leave", "Payslips" }, capturedAccountRequest.FeatureAccess);
        _selfServiceAccountServiceMock.Verify(
            service => service.CreateAsync(It.IsAny<CreateSelfServiceAccountRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
        Assert.Contains(secondaryDepartmentId, result.DepartmentIds);

        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeExists_UpdatesAndReturnsDto()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var existingEmployee = new Employee
        {
            Id = employeeId,
            FirstName = "Existing",
            LastName = "Employee",
            Email = "existing.employee@example.com",
            EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            JobTitle = "Analyst",
            Departments = new List<EmployeeDepartment>
            {
                new() { DepartmentId = departmentId, IsPrimary = true }
            }
        };

        var request = new UpdateEmployeeRequest
        {
            FirstName = "Updated",
            LastName = "Employee",
            Email = "updated.employee@example.com",
            EmploymentStartDate = existingEmployee.EmploymentStartDate,
            JobTitle = "Senior Analyst",
            EmploymentEndDate = null,
            DateOfBirth = existingEmployee.DateOfBirth,
            DepartmentAssignment = new EmployeeDepartmentAssignmentRequest
            {
                PrimaryDepartmentId = departmentId,
                SecondaryDepartmentIds = Array.Empty<Guid>()
            },
            JobArchitecture = new EmployeeJobArchitectureRequest
            {
                JobFamily = "Ops",
                JobFunction = "Planning",
                JobLevel = "L4"
            },
            Contracts = new[]
            {
                new EmploymentContractRequest
                {
                    Id = Guid.NewGuid(),
                    ContractType = "Permanent",
                    Status = "Active",
                    EffectiveFrom = existingEmployee.EmploymentStartDate,
                    WorkLocation = "Remote"
                }
            },
            ComplianceDocuments = new[]
            {
                new EmployeeComplianceDocumentRequest
                {
                    Id = Guid.NewGuid(),
                    DocumentType = "Work Permit",
                    ReferenceNumber = "WP-123",
                    Status = "Pending",
                    IssuedOn = DateOnly.FromDateTime(DateTime.UtcNow)
                }
            }
        };

        Employee? updatedEntity = null;

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingEmployee);

        _repositoryMock
            .Setup(repo => repo.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((employee, _) => updatedEntity = employee)
            .ReturnsAsync(() => updatedEntity);

        // Act
        var result = await _sut.UpdateAsync(employeeId, request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.FirstName, result!.FirstName);
        Assert.NotNull(updatedEntity);
        Assert.Equal(employeeId, updatedEntity!.Id);
        Assert.Equal("Ops", result.JobArchitecture.JobFamily);
        Assert.Equal(departmentId, result.PrimaryDepartmentId);
        Assert.Single(result.Contracts);
        Assert.Single(result.ComplianceDocuments);

        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeDoesNotExist_ReturnsNull()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var request = new UpdateEmployeeRequest
        {
            FirstName = "Updated",
            LastName = "Employee",
            Email = "updated.employee@example.com",
            EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            JobTitle = "Senior Analyst",
            DepartmentAssignment = new EmployeeDepartmentAssignmentRequest
            {
                PrimaryDepartmentId = Guid.NewGuid()
            }
        };

        _repositoryMock
            .Setup(repo => repo.GetByIdAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var result = await _sut.UpdateAsync(employeeId, request, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _repositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_RemovesEmployee()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        _repositoryMock
            .Setup(repo => repo.RemoveAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.DeleteAsync(employeeId, CancellationToken.None);

        // Assert
        Assert.True(result);
        _repositoryMock.Verify(repo => repo.RemoveAsync(employeeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_FiltersSortsAndPaginatesEmployees()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var departmentId = Guid.NewGuid();
        var employees = new List<Employee>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Alice",
                LastName = "Johnson",
                Email = "alice.johnson@example.com",
                JobTitle = "HR Manager",
                EmploymentStartDate = today.AddDays(-400),
                Departments = new List<EmployeeDepartment>
                {
                    new() { DepartmentId = departmentId, IsPrimary = true }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Smith",
                Email = "bob.smith@example.com",
                JobTitle = "HR Associate",
                EmploymentStartDate = today.AddDays(-200),
                Departments = new List<EmployeeDepartment>
                {
                    new() { DepartmentId = departmentId, IsPrimary = true }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Charlie",
                LastName = "Adams",
                Email = "charlie.adams@example.com",
                JobTitle = "Finance Analyst",
                EmploymentStartDate = today.AddDays(-100),
                EmploymentEndDate = today.AddDays(-10),
                Departments = new List<EmployeeDepartment>
                {
                    new() { DepartmentId = Guid.NewGuid(), IsPrimary = true }
                }
            }
        };

        _repositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

        var departments = new List<Department>
        {
            new() { Id = departmentId, Name = "Human Resources", OrganizationId = Guid.NewGuid() }
        };

        _departmentRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        var request = new EmployeeSearchRequest
        {
            Query = "HR",
            DepartmentId = departmentId,
            IsActive = true,
            SortBy = EmployeeSortField.EmploymentStartDate,
            SortDirection = SortDirection.Descending,
            PageNumber = 1,
            PageSize = 1
        };

        // Act
        var result = await _sut.SearchAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Bob", result.Items.First().FirstName);
        Assert.False(result.IsLastPage);
    }

    [Fact]
    public async Task GetWorkforceSnapshotAsync_ComputesAnalytics()
    {
        // Arrange
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var departmentA = Guid.NewGuid();
        var departmentB = Guid.NewGuid();

        var employees = new List<Employee>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Active",
                LastName = "One",
                Email = "active.one@example.com",
                JobTitle = "Consultant",
                EmploymentStartDate = today.AddDays(-15),
                Departments = new List<EmployeeDepartment>
                {
                    new() { DepartmentId = departmentA, IsPrimary = true }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Active",
                LastName = "Two",
                Email = "active.two@example.com",
                JobTitle = "Consultant",
                EmploymentStartDate = today.AddDays(-200),
                EmploymentEndDate = today.AddDays(10),
                Departments = new List<EmployeeDepartment>
                {
                    new() { DepartmentId = departmentA, IsPrimary = true }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Active",
                LastName = "Three",
                Email = "active.three@example.com",
                JobTitle = "Lead",
                EmploymentStartDate = today.AddDays(-800),
                Departments = new List<EmployeeDepartment>
                {
                    new() { DepartmentId = departmentB, IsPrimary = true },
                    new() { DepartmentId = departmentA, IsPrimary = false }
                }
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Former",
                LastName = "Employee",
                Email = "former.employee@example.com",
                JobTitle = "Lead",
                EmploymentStartDate = today.AddDays(-500),
                EmploymentEndDate = today.AddDays(-5),
                Departments = new List<EmployeeDepartment>
                {
                    new() { DepartmentId = departmentB, IsPrimary = true }
                }
            }
        };

        var departments = new List<Department>
        {
            new() { Id = departmentA, Name = "People Ops", OrganizationId = Guid.NewGuid() },
            new() { Id = departmentB, Name = "Finance", OrganizationId = Guid.NewGuid() }
        };

        _repositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);
        _departmentRepositoryMock
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(departments);

        // Act
        var snapshot = await _sut.GetWorkforceSnapshotAsync(CancellationToken.None);

        // Assert
        Assert.Equal(4, snapshot.TotalEmployees);
        Assert.Equal(3, snapshot.ActiveEmployees);
        Assert.Equal(1, snapshot.InactiveEmployees);
        Assert.Equal(1, snapshot.DeparturesLast30Days);
        Assert.Equal(1, snapshot.UpcomingDeparturesNext30Days);
        Assert.True(snapshot.AverageTenureInYears > 0);
        Assert.Equal(2, snapshot.DepartmentHeadcounts.Count);

        var peopleOps = snapshot.DepartmentHeadcounts.Single(dto => dto.DepartmentName == "People Ops");
        Assert.Equal(2, peopleOps.ActiveEmployees);
        Assert.Equal(3, peopleOps.TotalEmployees);
    }
}
