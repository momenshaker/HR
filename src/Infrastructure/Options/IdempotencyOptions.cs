namespace HR.Infrastructure.Options;

/// <summary>
///     Options controlling API idempotency behaviour.
/// </summary>
public sealed class IdempotencyOptions
{
    /// <summary>
    ///     Duration in hours that idempotent responses are stored and reused.
    /// </summary>
    public int WindowHours { get; set; } = 24;

    public const string SectionName = "Idempotency";
}
