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
    private readonly EmployeeService _sut;

    public EmployeeServiceTests()
    {
        _sut = new EmployeeService(_repositoryMock.Object);
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
}
