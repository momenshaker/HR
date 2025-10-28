using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="RecognitionProgram" /> entities.
/// </summary>
public static class RecognitionProgramMappings
{
    public static RecognitionProgramDto ToDto(this RecognitionProgram program)
    {
        ArgumentNullException.ThrowIfNull(program);

        return new RecognitionProgramDto(
            program.Id,
            program.Name,
            program.Description,
            program.Criteria,
            program.Reward,
            program.IsPeerToPeer,
            program.IsActive,
            program.OwnerId,
            program.CreatedAtUtc);
    }

    public static RecognitionProgram ToEntity(this CreateRecognitionProgramRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new RecognitionProgram
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Criteria = request.Criteria.Trim(),
            Reward = request.Reward.Trim(),
            IsPeerToPeer = request.IsPeerToPeer,
            IsActive = request.IsActive,
            OwnerId = request.OwnerId,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public static RecognitionProgram ApplyUpdates(this UpdateRecognitionProgramRequest request, RecognitionProgram existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new RecognitionProgram
        {
            Id = existing.Id,
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Criteria = request.Criteria.Trim(),
            Reward = request.Reward.Trim(),
            IsPeerToPeer = request.IsPeerToPeer,
            IsActive = request.IsActive,
            OwnerId = request.OwnerId,
            CreatedAtUtc = request.CreatedAtUtc
        };
    }
}
