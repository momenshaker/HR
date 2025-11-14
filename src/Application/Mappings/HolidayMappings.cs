using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for holiday definitions.
/// </summary>
public static class HolidayMappings
{
    public static HolidayDto ToDto(this Holiday holiday)
    {
        ArgumentNullException.ThrowIfNull(holiday);

        return new HolidayDto(
            holiday.Id,
            holiday.OrganizationId,
            holiday.Date,
            holiday.Name,
            holiday.IsPaid,
            holiday.CountryCode,
            holiday.Description);
    }

    public static Holiday ToEntity(this CreateHolidayRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new Holiday
        {
            Id = Guid.NewGuid(),
            OrganizationId = request.OrganizationId,
            Date = request.Date,
            Name = request.Name.Trim(),
            IsPaid = request.IsPaid,
            CountryCode = request.CountryCode.Trim(),
            Description = request.Description.Trim()
        };
    }

    public static Holiday ApplyUpdates(this UpdateHolidayRequest request, Holiday existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new Holiday
        {
            Id = existing.Id,
            OrganizationId = request.OrganizationId,
            Date = request.Date,
            Name = request.Name.Trim(),
            IsPaid = request.IsPaid,
            CountryCode = request.CountryCode.Trim(),
            Description = request.Description.Trim()
        };
    }
}
