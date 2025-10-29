using HR.Application.Validation;
namespace HR.Application.DTOs;

/// <summary>
///     Request payload for creating a recognition programme.
/// </summary>
public sealed class CreateRecognitionProgramRequest : IValidatableRequest
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Criteria { get; init; } = string.Empty;

    public string Reward { get; init; } = string.Empty;

    public bool IsPeerToPeer { get; init; }

    public bool IsActive { get; init; } = true;

    public Guid OwnerId { get; init; }
}