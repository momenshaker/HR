namespace HR.Infrastructure.Options;

/// <summary>
///     Options describing API rate limiting behaviour.
/// </summary>
public sealed class RateLimitOptions
{
    /// <summary>
    ///     Maximum number of requests allowed per window for a single token.
    /// </summary>
    public int RequestsPerWindow { get; set; } = 100;

    /// <summary>
    ///     Length of the rate limiting window in seconds.
    /// </summary>
    public int WindowSeconds { get; set; } = 60;

    public const string SectionName = "RateLimit";
}
