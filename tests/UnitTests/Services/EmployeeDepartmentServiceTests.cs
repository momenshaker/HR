using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using HR.Application.Abstractions.Repositories;
using HR.Application.Services;
using HR.Domain.Entities;
using Moq;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class EmployeeDepartmentServiceTests
{
    private readonly Mock<IEmployeeDepartmentRepository> _employeeDepartmentRepositoryMock = new();
    private readonly Mock<IDepartmentRepository> _departmentRepositoryMock = new();
    private readonly EmployeeDepartmentService _sut;

    public EmployeeDepartmentServiceTests()
    {
        _sut = new EmployeeDepartmentService(
            _employeeDepartmentRepositoryMock.Object,
            _departmentRepositoryMock.Object);
    }

    [Fact]
    public async Task AssignAsync_WhenDepartmentsValid_AddsMissingAssignments()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var existingDepartmentId = Guid.NewGuid();
        var newDepartmentId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();

        var departments = new Dictionary<Guid, Department>
        {
            [existingDepartmentId] = CreateDepartment(existingDepartmentId, organizationId),
            [newDepartmentId] = CreateDepartment(newDepartmentId, organizationId)
        };

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.GetDepartmentIdsByEmployeeAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { existingDepartmentId });

        _departmentRepositoryMock
            .Setup(repository => repository.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Guid> ids, CancellationToken _) => ids.Select(id => departments[id]).ToArray());

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.AssignAsync(
                employeeId,
                It.Is<IReadOnlyCollection<Guid>>(ids => ids.Single() == newDepartmentId),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _sut.AssignAsync(employeeId, new[] { existingDepartmentId, newDepartmentId }, CancellationToken.None);

        // Assert
        _employeeDepartmentRepositoryMock.Verify();
    }

    [Fact]
    public async Task AssignAsync_WhenDepartmentsAlreadyAssigned_DoesNotInvokeAssign()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.GetDepartmentIdsByEmployeeAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { departmentId });

        _departmentRepositoryMock
            .Setup(repository => repository.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Guid> ids, CancellationToken _) =>
                ids.Select(id => CreateDepartment(id, organizationId)).ToArray());

        // Act
        await _sut.AssignAsync(employeeId, new[] { departmentId }, CancellationToken.None);

        // Assert
        _employeeDepartmentRepositoryMock
            .Verify(repository => repository.AssignAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task AssignAsync_WhenDepartmentsFromDifferentOrganizations_ThrowsValidationException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var departmentIdOne = Guid.NewGuid();
        var departmentIdTwo = Guid.NewGuid();
        var firstOrganizationId = Guid.NewGuid();
        var secondOrganizationId = Guid.NewGuid();

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.GetDepartmentIdsByEmployeeAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());

        _departmentRepositoryMock
            .Setup(repository => repository.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Guid> ids, CancellationToken _) => ids
                .Select(id => id == departmentIdOne
                    ? CreateDepartment(departmentIdOne, firstOrganizationId)
                    : CreateDepartment(departmentIdTwo, secondOrganizationId))
                .ToArray());

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.AssignAsync(employeeId, new[] { departmentIdOne, departmentIdTwo }, CancellationToken.None));

        _employeeDepartmentRepositoryMock
            .Verify(repository => repository.AssignAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task AssignAsync_WhenDepartmentNotFound_ThrowsValidationException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var missingDepartmentId = Guid.NewGuid();

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.GetDepartmentIdsByEmployeeAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());

        _departmentRepositoryMock
            .Setup(repository => repository.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Department>);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.AssignAsync(employeeId, new[] { missingDepartmentId }, CancellationToken.None));
    }

    [Fact]
    public async Task AssignAsync_WhenDuplicateIdentifiersProvided_DeduplicatesBeforeAssigning()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.GetDepartmentIdsByEmployeeAsync(employeeId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());

        _departmentRepositoryMock
            .Setup(repository => repository.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Guid> ids, CancellationToken _) =>
                ids.Select(id => CreateDepartment(id, organizationId)).ToArray());

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.AssignAsync(
                employeeId,
                It.Is<IReadOnlyCollection<Guid>>(ids => ids.Single() == departmentId),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _sut.AssignAsync(employeeId, new[] { departmentId, departmentId }, CancellationToken.None);

        // Assert
        _employeeDepartmentRepositoryMock.Verify();
    }

    [Fact]
    public async Task AssignAsync_WhenEmptyIdentifierProvided_ThrowsValidationException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.AssignAsync(employeeId, new[] { Guid.Empty }, CancellationToken.None));
    }

    [Fact]
    public async Task ReplaceAsync_WhenDepartmentsValid_DelegatesToRepository()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();

        _departmentRepositoryMock
            .Setup(repository => repository.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Guid> ids, CancellationToken _) =>
                ids.Select(id => CreateDepartment(id, organizationId)).ToArray());

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.ReplaceAsync(
                employeeId,
                It.Is<IReadOnlyCollection<Guid>>(ids => ids.Single() == departmentId),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _sut.ReplaceAsync(employeeId, new[] { departmentId }, CancellationToken.None);

        // Assert
        _employeeDepartmentRepositoryMock.Verify();
    }

    [Fact]
    public async Task ReplaceAsync_WhenDepartmentsCrossOrganizations_ThrowsValidationException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var departmentIdOne = Guid.NewGuid();
        var departmentIdTwo = Guid.NewGuid();

        _departmentRepositoryMock
            .Setup(repository => repository.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                CreateDepartment(departmentIdOne, Guid.NewGuid()),
                CreateDepartment(departmentIdTwo, Guid.NewGuid())
            });

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.ReplaceAsync(employeeId, new[] { departmentIdOne, departmentIdTwo }, CancellationToken.None));

        _employeeDepartmentRepositoryMock
            .Verify(repository => repository.ReplaceAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task ReplaceAsync_WhenDepartmentsCollectionEmpty_RemovesAllAssignments()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.ReplaceAsync(
                employeeId,
                It.Is<IReadOnlyCollection<Guid>>(ids => !ids.Any()),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _sut.ReplaceAsync(employeeId, Array.Empty<Guid>(), CancellationToken.None);

        // Assert
        _employeeDepartmentRepositoryMock.Verify();
        _departmentRepositoryMock
            .Verify(repository => repository.GetByIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task ReplaceAsync_WhenEmptyIdentifierProvided_ThrowsValidationException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.ReplaceAsync(employeeId, new[] { Guid.Empty }, CancellationToken.None));
    }

    [Fact]
    public async Task UnassignAsync_WhenDepartmentsProvided_DelegatesToRepository()
    {
        // Arrange
        var employeeId = Guid.NewGuid();
        var departmentId = Guid.NewGuid();

        _employeeDepartmentRepositoryMock
            .Setup(repository => repository.UnassignAsync(
                employeeId,
                It.Is<IReadOnlyCollection<Guid>>(ids => ids.Single() == departmentId),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        await _sut.UnassignAsync(employeeId, new[] { departmentId }, CancellationToken.None);

        // Assert
        _employeeDepartmentRepositoryMock.Verify();
    }

    [Fact]
    public async Task UnassignAsync_WhenCollectionEmpty_DoesNotCallRepository()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Act
        await _sut.UnassignAsync(employeeId, Array.Empty<Guid>(), CancellationToken.None);

        // Assert
        _employeeDepartmentRepositoryMock
            .Verify(repository => repository.UnassignAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()),
                Times.Never);
    }

    [Fact]
    public async Task UnassignAsync_WhenIdentifierEmpty_ThrowsValidationException()
    {
        // Arrange
        var employeeId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.UnassignAsync(employeeId, new[] { Guid.Empty }, CancellationToken.None));
    }

    private static Department CreateDepartment(Guid id, Guid organizationId)
    {
        return new Department(
            id,
            organizationId,
            $"Department-{id}",
            $"/department/{id}",
            level: 1,
            createdAtUtc: DateTime.UtcNow,
            isActive: true);
    }
}
