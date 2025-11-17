using System;

namespace HR.Application.DTOs;

public sealed record AttendancePunchConfigurationDto(
    Guid Id,
    string PunchType,
    string DisplayName,
    string Description,
    int SortOrder,
    bool IsActive);
