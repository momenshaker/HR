using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Application.Abstractions.Repositories;
using HR.Application.DTOs;
using HR.Application.Services;
using HR.Infrastructure.Persistence.Repositories;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class CommsServiceTests
{
    private readonly ICommsAnnouncementRepository _annRepo = new InMemoryCommsAnnouncementRepository();
    private readonly IAnnouncementReadRepository _readRepo = new InMemoryAnnouncementReadRepository();
    private readonly IEmployeeDepartmentRepository _empDeptRepo = new InMemoryEmployeeDepartmentRepository();

    private CommsService CreateSut() => new(_annRepo, _readRepo, _empDeptRepo, TimeProvider.System);

    [Fact]
    public async Task Visibility_Rules_OrgVsDept()
    {
        var sut = CreateSut();
        var org = Guid.NewGuid();
        var deptA = Guid.NewGuid();
        var deptB = Guid.NewGuid();
        var publisher = Guid.NewGuid();

        // org-wide
        await sut.PublishAsync(new CreateCommsAnnouncementRequest
        {
            OrganizationId = org,
            Title = "Org",
            Body = "All",
            PublishedById = publisher
        }, CancellationToken.None);

        // dept A
        await sut.PublishAsync(new CreateCommsAnnouncementRequest
        {
            OrganizationId = org,
            DepartmentId = deptA,
            Title = "A",
            Body = "Dept A",
            PublishedById = publisher
        }, CancellationToken.None);

        // dept B
        await sut.PublishAsync(new CreateCommsAnnouncementRequest
        {
            OrganizationId = org,
            DepartmentId = deptB,
            Title = "B",
            Body = "Dept B",
            PublishedById = publisher
        }, CancellationToken.None);

        var all = await sut.GetAnnouncementsAsync(org, null, null, 1, 50, CancellationToken.None);
        Assert.Equal(3, all.TotalCount);

        var onlyA = await sut.GetAnnouncementsAsync(org, deptA, null, 1, 50, CancellationToken.None);
        Assert.Equal(2, onlyA.TotalCount); // org-wide + A
        Assert.All(onlyA.Items, a => Assert.True(a.DepartmentId == null || a.DepartmentId == deptA));
    }

    [Fact]
    public async Task Unread_Counts_Respect_Employee_Departments()
    {
        var sut = CreateSut();
        var org = Guid.NewGuid();
        var deptA = Guid.NewGuid();
        var deptB = Guid.NewGuid();
        var publisher = Guid.NewGuid();
        var emp = Guid.NewGuid();

        await _empDeptRepo.AssignAsync(emp, new[] { deptA }, CancellationToken.None);

        var orgWide = await sut.PublishAsync(new CreateCommsAnnouncementRequest
        {
            OrganizationId = org,
            Title = "Org",
            Body = "All",
            PublishedById = publisher
        }, CancellationToken.None);

        var a = await sut.PublishAsync(new CreateCommsAnnouncementRequest
        {
            OrganizationId = org,
            DepartmentId = deptA,
            Title = "A",
            Body = "Dept A",
            PublishedById = publisher
        }, CancellationToken.None);

        var b = await sut.PublishAsync(new CreateCommsAnnouncementRequest
        {
            OrganizationId = org,
            DepartmentId = deptB,
            Title = "B",
            Body = "Dept B",
            PublishedById = publisher
        }, CancellationToken.None);

        var unread = await sut.GetAnnouncementsAsync(org, null, emp, 1, 50, CancellationToken.None);
        Assert.Equal(2, unread.TotalCount); // org-wide + deptA
        Assert.DoesNotContain(unread.Items, x => x.Id == b.Id);

        // mark org-wide as read
        await sut.MarkReadAsync(orgWide.Id, emp, DateTime.UtcNow, CancellationToken.None);
        var unreadAfter = await sut.GetAnnouncementsAsync(org, null, emp, 1, 50, CancellationToken.None);
        Assert.Equal(1, unreadAfter.TotalCount);
        Assert.Equal(a.Id, unreadAfter.Items.Single().Id);
    }

    [Fact]
    public async Task Pinned_First_Then_Newest()
    {
        var sut = CreateSut();
        var org = Guid.NewGuid();
        var pub = Guid.NewGuid();

        var older = await sut.PublishAsync(new CreateCommsAnnouncementRequest
        {
            OrganizationId = org,
            Title = "Old",
            Body = "Old",
            PublishedById = pub
        }, CancellationToken.None);

        await Task.Delay(5);

        var newer = await sut.PublishAsync(new CreateCommsAnnouncementRequest
        {
            OrganizationId = org,
            Title = "New",
            Body = "New",
            PublishedById = pub
        }, CancellationToken.None);

        // Initially newest first
        var initial = await sut.GetAnnouncementsAsync(org, null, null, 1, 10, CancellationToken.None);
        Assert.Equal(newer.Id, initial.Items.First().Id);

        // Pin older -> should appear first
        var pinnedOk = await sut.PinAsync(older.Id, CancellationToken.None);
        Assert.True(pinnedOk);

        var afterPin = await sut.GetAnnouncementsAsync(org, null, null, 1, 10, CancellationToken.None);
        Assert.Equal(older.Id, afterPin.Items.First().Id);
    }
}

