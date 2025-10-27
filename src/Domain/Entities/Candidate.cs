namespace HR.Domain.Entities;

/// <summary>
///     Represents a candidate progressing through the recruitment pipeline.
/// </summary>
public sealed class Candidate
{
    public Guid Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string AppliedRole { get; init; } = string.Empty;

    public string Stage { get; init; } = string.Empty;

    public string Source { get; init; } = string.Empty;

    public DateTime AppliedAtUtc { get; init; }

    public DateTime? NextInterviewAtUtc { get; init; }

    public string ResumeUrl { get; init; } = string.Empty;

    public string Notes { get; init; } = string.Empty;
}
