namespace HR.Application.DTOs;

public sealed record PayslipDto(
    Guid Id,
    Guid RunId,
    Guid EmployeeId,
    string? PublicUrl,
    DateTime GeneratedAtUtc);

