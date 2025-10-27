using HR.Application.Abstractions.Repositories;
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
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _sut = new EmployeeService(_repositoryMock.Object, _departmentRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsEmployeesFromRepository()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                DepartmentId = Guid.NewGuid(),
                EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                JobTitle = "HR Specialist"
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
    public async Task CreateAsync_MapsRequestAndPersistsEmployee()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            DepartmentId = Guid.NewGuid(),
            EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            JobTitle = "Software Engineer"
        };

        Employee? persistedEmployee = null;

        _repositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()))
            .Callback<Employee, CancellationToken>((employee, _) => persistedEmployee = employee)
            .ReturnsAsync(() => persistedEmployee!);

        // Act
        var result = await _sut.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(persistedEmployee);
        Assert.Equal(request.FirstName, persistedEmployee!.FirstName);
        Assert.Equal(request.Email, result.Email);
        _repositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Employee>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmployeeExists_UpdatesAndReturnsDto()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var existingEmployee = new Employee
        {
            Id = employeeId,
            FirstName = "Existing",
            LastName = "Employee",
            Email = "existing.employee@example.com",
            DepartmentId = Guid.NewGuid(),
            EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            JobTitle = "Analyst"
        };

        var request = new UpdateEmployeeRequest
        {
            FirstName = "Updated",
            LastName = "Employee",
            Email = "updated.employee@example.com",
            DepartmentId = existingEmployee.DepartmentId,
            EmploymentStartDate = existingEmployee.EmploymentStartDate,
            JobTitle = "Senior Analyst",
            EmploymentEndDate = null,
            DateOfBirth = existingEmployee.DateOfBirth
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
            DepartmentId = Guid.NewGuid(),
            EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            JobTitle = "Senior Analyst"
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
                DepartmentId = departmentId,
                JobTitle = "HR Manager",
                EmploymentStartDate = today.AddDays(-400),
                EmploymentEndDate = null
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Bob",
                LastName = "Smith",
                Email = "bob.smith@example.com",
                DepartmentId = departmentId,
                JobTitle = "HR Associate",
                EmploymentStartDate = today.AddDays(-200),
                EmploymentEndDate = null
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Charlie",
                LastName = "Adams",
                Email = "charlie.adams@example.com",
                DepartmentId = Guid.NewGuid(),
                JobTitle = "Finance Analyst",
                EmploymentStartDate = today.AddDays(-100),
                EmploymentEndDate = today.AddDays(-10)
            }
        };

        _repositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(employees);

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
                DepartmentId = departmentA,
                JobTitle = "Consultant",
                EmploymentStartDate = today.AddDays(-15)
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Active",
                LastName = "Two",
                Email = "active.two@example.com",
                DepartmentId = departmentA,
                JobTitle = "Consultant",
                EmploymentStartDate = today.AddDays(-200),
                EmploymentEndDate = today.AddDays(10)
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Active",
                LastName = "Three",
                Email = "active.three@example.com",
                DepartmentId = departmentB,
                JobTitle = "Lead",
                EmploymentStartDate = today.AddDays(-800)
            },
            new()
            {
                Id = Guid.NewGuid(),
                FirstName = "Former",
                LastName = "Employee",
                Email = "former.employee@example.com",
                DepartmentId = departmentB,
                JobTitle = "Lead",
                EmploymentStartDate = today.AddDays(-500),
                EmploymentEndDate = today.AddDays(-5)
            }
        };

        var departments = new List<Department>
        {
            new() { Id = departmentA, Name = "People Ops" },
            new() { Id = departmentB, Name = "Finance" }
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
        Assert.Equal(2, peopleOps.TotalEmployees);
    }
}
