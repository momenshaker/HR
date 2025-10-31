namespace HR.Application.DTOs;

public sealed record PagedLeaveRequestsDto(
    int Page,
    int PageSize,
    int TotalCount,
    IReadOnlyCollection<LeaveRequestDto> Items);

