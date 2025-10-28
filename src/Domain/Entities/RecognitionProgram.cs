namespace HR.Domain.Entities;

/// <summary>
///     Represents an employee recognition programme with defined criteria and rewards.
/// </summary>
public sealed class RecognitionProgram
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string Criteria { get; init; } = string.Empty;

    public string Reward { get; init; } = string.Empty;

    public bool IsPeerToPeer { get; init; }

    public bool IsActive { get; init; }

    public Guid OwnerId { get; init; }

    public DateTime CreatedAtUtc { get; init; }
}
