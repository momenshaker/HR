namespace HR.Application.DTOs;

/// <summary>
///     Data transfer object for recognition programme information.
/// </summary>
public sealed record RecognitionProgramDto(
    Guid Id,
    string Name,
    string Description,
    string Criteria,
    string Reward,
    bool IsPeerToPeer,
    bool IsActive,
    Guid OwnerId,
    DateTime CreatedAtUtc);
