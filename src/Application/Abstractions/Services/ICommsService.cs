using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

public interface ICommsService
{
    Task<CommsAnnouncementDto> PublishAsync(CreateCommsAnnouncementRequest request, CancellationToken cancellationToken = default);

    Task<bool> PinAsync(Guid announcementId, CancellationToken cancellationToken = default);

    Task<bool> UnpinAsync(Guid announcementId, CancellationToken cancellationToken = default);

    Task MarkReadAsync(Guid announcementId, Guid employeeId, DateTime readAtUtc, CancellationToken cancellationToken = default);

    Task<HR.Application.DTOs.PaginatedResponse<CommsAnnouncementDto>> GetAnnouncementsAsync(
        Guid organizationId,
        Guid? departmentId,
        Guid? unreadForEmployeeId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}

