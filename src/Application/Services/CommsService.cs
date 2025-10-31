using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

public sealed class CommsService : ICommsService
{
    private readonly ICommsAnnouncementRepository _announcements;
    private readonly IAnnouncementReadRepository _reads;
    private readonly IEmployeeDepartmentRepository _employeeDepartments;
    private readonly TimeProvider _clock;

    public CommsService(
        ICommsAnnouncementRepository announcements,
        IAnnouncementReadRepository reads,
        IEmployeeDepartmentRepository employeeDepartments,
        TimeProvider clock)
    {
        _announcements = announcements;
        _reads = reads;
        _employeeDepartments = employeeDepartments;
        _clock = clock;
    }

    public async Task<CommsAnnouncementDto> PublishAsync(CreateCommsAnnouncementRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var entity = request.ToEntity(_clock.GetUtcNow().UtcDateTime);
        var created = await _announcements.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    public async Task<bool> PinAsync(Guid announcementId, CancellationToken cancellationToken = default)
    {
        var existing = await _announcements.GetByIdAsync(announcementId, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return false;
        }
        if (existing.IsPinned)
        {
            return true;
        }
        var updated = new CommsAnnouncement
        {
            Id = existing.Id,
            OrganizationId = existing.OrganizationId,
            DepartmentId = existing.DepartmentId,
            Title = existing.Title,
            Body = existing.Body,
            PublishedAtUtc = existing.PublishedAtUtc,
            PublishedById = existing.PublishedById,
            IsPinned = true
        };
        return await _announcements.UpdateAsync(updated, cancellationToken).ConfigureAwait(false) is not null;
    }

    public async Task<bool> UnpinAsync(Guid announcementId, CancellationToken cancellationToken = default)
    {
        var existing = await _announcements.GetByIdAsync(announcementId, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return false;
        }
        if (!existing.IsPinned)
        {
            return true;
        }
        var updated = new CommsAnnouncement
        {
            Id = existing.Id,
            OrganizationId = existing.OrganizationId,
            DepartmentId = existing.DepartmentId,
            Title = existing.Title,
            Body = existing.Body,
            PublishedAtUtc = existing.PublishedAtUtc,
            PublishedById = existing.PublishedById,
            IsPinned = false
        };
        return await _announcements.UpdateAsync(updated, cancellationToken).ConfigureAwait(false) is not null;
    }

    public Task MarkReadAsync(Guid announcementId, Guid employeeId, DateTime readAtUtc, CancellationToken cancellationToken = default)
    {
        var read = new AnnouncementRead
        {
            AnnouncementId = announcementId,
            EmployeeId = employeeId,
            ReadAtUtc = readAtUtc
        };
        return _reads.MarkReadAsync(read, cancellationToken);
    }

    public async Task<PaginatedResponse<CommsAnnouncementDto>> GetAnnouncementsAsync(
        Guid organizationId,
        Guid? departmentId,
        Guid? unreadForEmployeeId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 25;

        var allInOrg = await _announcements.GetByOrganizationAsync(organizationId, cancellationToken).ConfigureAwait(false);

        IEnumerable<CommsAnnouncement> filtered = allInOrg;

        if (unreadForEmployeeId.HasValue)
        {
            var deptIds = await _employeeDepartments.GetDepartmentIdsByEmployeeAsync(unreadForEmployeeId.Value, cancellationToken).ConfigureAwait(false);
            var deptSet = deptIds.ToHashSet();
            filtered = filtered.Where(a => a.DepartmentId is null || deptSet.Contains(a.DepartmentId.Value));

            var readIds = await _reads.GetReadAnnouncementIdsAsync(unreadForEmployeeId.Value, cancellationToken).ConfigureAwait(false);
            var readSet = readIds.ToHashSet();
            filtered = filtered.Where(a => !readSet.Contains(a.Id));
        }
        else if (departmentId.HasValue)
        {
            filtered = filtered.Where(a => a.DepartmentId == null || a.DepartmentId == departmentId);
        }

        var ordered = filtered
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.PublishedAtUtc)
            .ToArray();

        var total = ordered.Length;
        var pageItems = ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => a.ToDto())
            .ToArray();

        return new PaginatedResponse<CommsAnnouncementDto>(page, pageSize, total, pageItems);
    }
}
