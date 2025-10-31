using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Services;
using HR.Domain.Entities;
using HR.Infrastructure.Persistence.Repositories;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class DepartmentTreeServiceTests
{
    private readonly InMemoryDepartmentRepository _repository = new();
    private readonly DepartmentTreeService _sut;

    public DepartmentTreeServiceTests()
    {
        _sut = new DepartmentTreeService(_repository);
    }

    [Fact]
    public async Task MoveDepartmentAsync_ReparentsSubtreeAndUpdatesHierarchy()
    {
        var organizationId = Guid.NewGuid();
        var rootId = Guid.NewGuid();
        var originalParentId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var newParentId = Guid.NewGuid();

        var root = CreateDepartment(rootId, organizationId, null, 0);
        var originalParent = CreateDepartment(originalParentId, organizationId, rootId, 1, root.Path);
        var child = CreateDepartment(childId, organizationId, originalParentId, 2, originalParent.Path);
        var newParent = CreateDepartment(newParentId, organizationId, null, 0);

        await SeedAsync(root, originalParent, child, newParent).ConfigureAwait(false);

        await _sut.MoveDepartmentAsync(originalParentId, newParentId, CancellationToken.None).ConfigureAwait(false);

        var updatedDepartments = await _repository.GetAllAsync(CancellationToken.None).ConfigureAwait(false);
        var movedParent = updatedDepartments.Single(department => department.Id == originalParentId);
        var movedChild = updatedDepartments.Single(department => department.Id == childId);

        Assert.Equal(newParentId, movedParent.ParentDepartmentId);
        Assert.Equal(newParent.Path + $"/{originalParentId}", movedParent.Path);
        Assert.Equal(newParent.Level + 1, movedParent.Level);

        Assert.Equal(originalParentId, movedChild.ParentDepartmentId);
        Assert.Equal(movedParent.Path + $"/{childId}", movedChild.Path);
        Assert.Equal(movedParent.Level + 1, movedChild.Level);
    }

    [Fact]
    public async Task MoveDepartmentAsync_WhenOrganizationsDiffer_Throws()
    {
        var organizationA = Guid.NewGuid();
        var organizationB = Guid.NewGuid();
        var departmentId = Guid.NewGuid();
        var otherOrgParentId = Guid.NewGuid();

        var department = CreateDepartment(departmentId, organizationA, null, 0);
        var otherOrgParent = CreateDepartment(otherOrgParentId, organizationB, null, 0);

        await SeedAsync(department, otherOrgParent).ConfigureAwait(false);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.MoveDepartmentAsync(departmentId, otherOrgParentId, CancellationToken.None));
    }

    [Fact]
    public async Task MoveDepartmentAsync_WhenTargetIsDescendant_Throws()
    {
        var organizationId = Guid.NewGuid();
        var rootId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var grandChildId = Guid.NewGuid();

        var root = CreateDepartment(rootId, organizationId, null, 0);
        var child = CreateDepartment(childId, organizationId, rootId, 1, root.Path);
        var grandChild = CreateDepartment(grandChildId, organizationId, childId, 2, child.Path);

        await SeedAsync(root, child, grandChild).ConfigureAwait(false);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.MoveDepartmentAsync(rootId, grandChildId, CancellationToken.None));
    }

    [Fact]
    public async Task GetSubtreeAsync_ReturnsRootAndDescendants()
    {
        var organizationId = Guid.NewGuid();
        var rootId = Guid.NewGuid();
        var childAId = Guid.NewGuid();
        var childBId = Guid.NewGuid();
        var grandChildId = Guid.NewGuid();

        var root = CreateDepartment(rootId, organizationId, null, 0);
        var childA = CreateDepartment(childAId, organizationId, rootId, 1, root.Path);
        var childB = CreateDepartment(childBId, organizationId, rootId, 1, root.Path);
        var grandChild = CreateDepartment(grandChildId, organizationId, childAId, 2, childA.Path);

        await SeedAsync(root, childA, childB, grandChild).ConfigureAwait(false);

        var subtree = await _sut.GetSubtreeAsync(rootId, CancellationToken.None).ConfigureAwait(false);

        Assert.Equal(4, subtree.Count);
        Assert.Equal(rootId, subtree.First().Id);
        Assert.Contains(subtree, department => department.Id == grandChildId);
    }

    [Fact]
    public async Task GetAncestorsAsync_ReturnsOrderedAncestors()
    {
        var organizationId = Guid.NewGuid();
        var rootId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var root = CreateDepartment(rootId, organizationId, null, 0);
        var parent = CreateDepartment(parentId, organizationId, rootId, 1, root.Path);
        var child = CreateDepartment(childId, organizationId, parentId, 2, parent.Path);

        await SeedAsync(root, parent, child).ConfigureAwait(false);

        var ancestors = await _sut.GetAncestorsAsync(childId, CancellationToken.None).ConfigureAwait(false);

        Assert.Equal(new[] { rootId, parentId }, ancestors.Select(department => department.Id));
    }

    [Fact]
    public async Task GetBreadcrumbAsync_IncludesDepartment()
    {
        var organizationId = Guid.NewGuid();
        var rootId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var childId = Guid.NewGuid();

        var root = CreateDepartment(rootId, organizationId, null, 0);
        var parent = CreateDepartment(parentId, organizationId, rootId, 1, root.Path);
        var child = CreateDepartment(childId, organizationId, parentId, 2, parent.Path);

        await SeedAsync(root, parent, child).ConfigureAwait(false);

        var breadcrumb = await _sut.GetBreadcrumbAsync(childId, CancellationToken.None).ConfigureAwait(false);

        Assert.Equal(new[] { rootId, parentId, childId }, breadcrumb.Select(department => department.Id));
    }

    private static Department CreateDepartment(
        Guid id,
        Guid organizationId,
        Guid? parentDepartmentId,
        int level,
        string? parentPath = null)
    {
        var basePath = parentDepartmentId is null
            ? $"/org/{organizationId}/dept"
            : parentPath ?? throw new ArgumentNullException(nameof(parentPath));

        var path = $"{basePath}/{id}";

        return new Department
        {
            Id = id,
            OrganizationId = organizationId,
            ParentDepartmentId = parentDepartmentId,
            Name = $"Department-{id.ToString()[..8]}",
            Code = $"DEPT-{id.ToString()[..6].ToUpperInvariant()}",
            Path = path,
            Level = level,
            CreatedAtUtc = DateTime.UtcNow,
            IsActive = true
        };
    }

    private async Task SeedAsync(params Department[] departments)
    {
        foreach (var department in departments)
        {
            await _repository.AddAsync(department, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
