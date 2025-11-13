using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="LookupValue"/> entities.
/// </summary>
public static class LookupMappings
{
    public static LookupValueDto ToDto(this LookupValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new LookupValueDto(
            value.Id,
            value.Category,
            value.Code,
            value.DisplayName,
            value.Description,
            value.SortOrder,
            value.IsActive,
            value.UpdatedAtUtc);
    }

    public static LookupValue ToEntity(
        this CreateLookupValueRequest request,
        string normalizedCategory,
        string normalizedCode,
        int sortOrder,
        DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new LookupValue
        {
            Id = Guid.NewGuid(),
            Category = normalizedCategory,
            Code = normalizedCode,
            DisplayName = request.DisplayName.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            SortOrder = sortOrder,
            IsActive = request.IsActive,
            CreatedAtUtc = utcNow,
            UpdatedAtUtc = utcNow
        };
    }

    public static LookupValue ApplyUpdates(
        this UpdateLookupValueRequest request,
        LookupValue existing,
        string normalizedCategory,
        string normalizedCode,
        int sortOrder,
        DateTime utcNow)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new LookupValue
        {
            Id = existing.Id,
            Category = normalizedCategory,
            Code = normalizedCode,
            DisplayName = request.DisplayName.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim(),
            SortOrder = sortOrder,
            IsActive = request.IsActive,
            CreatedAtUtc = existing.CreatedAtUtc,
            UpdatedAtUtc = utcNow
        };
    }
}
