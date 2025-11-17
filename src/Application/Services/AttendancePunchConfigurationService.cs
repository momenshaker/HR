using System;
using System.Linq;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class AttendancePunchConfigurationService : IAttendancePunchConfigurationService
{
    private readonly IAttendancePunchConfigurationRepository _repository;

    public AttendancePunchConfigurationService(IAttendancePunchConfigurationRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<AttendancePunchConfigurationDto>> GetPunchTypesAsync(CancellationToken cancellationToken = default)
    {
        var configurations = await _repository
            .GetActiveAsync(cancellationToken)
            .ConfigureAwait(false);

        return configurations
            .Select(configuration => configuration.ToDto())
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<AttendancePunchConfigurationDto> SaveAsync(AttendancePunchConfigurationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Id is not null && request.Id != Guid.Empty)
        {
            var existing = await _repository.GetByIdAsync(request.Id.Value, cancellationToken).ConfigureAwait(false)
                           ?? throw new InvalidOperationException("Attendance punch configuration not found.");

            var updatedEntity = new AttendancePunchConfiguration
            {
                Id = existing.Id,
                PunchType = request.PunchType.Trim(),
                DisplayName = request.DisplayName.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                SortOrder = request.SortOrder,
                IsActive = request.IsActive
            };

            var persisted = await _repository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false)
                            ?? throw new InvalidOperationException("Failed to update attendance punch configuration.");

            return persisted.ToDto();
        }

        var newEntity = new AttendancePunchConfiguration
        {
            Id = Guid.NewGuid(),
            PunchType = request.PunchType.Trim(),
            DisplayName = request.DisplayName.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            SortOrder = request.SortOrder,
            IsActive = request.IsActive
        };

        var created = await _repository.AddAsync(newEntity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }
}

internal static class AttendancePunchConfigurationExtensions
{
    public static AttendancePunchConfigurationDto ToDto(this AttendancePunchConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return new AttendancePunchConfigurationDto(
            configuration.Id,
            configuration.PunchType,
            configuration.DisplayName,
            configuration.Description,
            configuration.SortOrder,
            configuration.IsActive);
    }
}
