namespace HR.Infrastructure.Security.Identity;

/// <summary>
///     Persisted refresh token associated with a user account.
/// </summary>
public sealed class UserRefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}

