namespace HR.Infrastructure.Options;

/// <summary>
///     Configuration options for development authentication accounts.
/// </summary>
public sealed class AuthenticationOptions
{
    /// <summary>
    ///     Gets or sets the collection of users available for interactive authentication.
    /// </summary>
    public List<UserOptions> Users { get; set; } = new();

    /// <summary>
    ///     Gets or sets the token lifetime in minutes for issued access tokens.
    /// </summary>
    public int TokenLifetimeMinutes { get; set; } = 60;

    /// <summary>
    ///     Configuration section name bound from <c>appsettings.json</c>.
    /// </summary>
    public const string SectionName = "Authentication";

    /// <summary>
    ///     Represents a user entry capable of authenticating against the platform.
    /// </summary>
    public sealed class UserOptions
    {
        /// <summary>
        ///     Gets or sets the unique identifier for the account. When left empty a value will be generated.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Gets or sets the email address for the account.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the plaintext password for the account. Intended for development scenarios only.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the collection of roles granted to the account.
        /// </summary>
        public List<string> Roles { get; set; } = new();

        /// <summary>
        ///     Gets or sets the customer identifier claim value.
        /// </summary>
        public string CustomerId { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets optional additional claims to append to the token.
        /// </summary>
        public Dictionary<string, string> Claims { get; set; } = new();
    }
}
