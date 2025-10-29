namespace HR.Infrastructure.Options;

/// <summary>
///     Options used for configuring JWT bearer authentication.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    ///     The authority issuing the JWT tokens.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    ///     The intended audience of the JWT tokens.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    ///     The symmetric signing key used to validate incoming JWT tokens.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    ///     Custom claim required for authorization scoping.
    /// </summary>
    public string CustomerClaim { get; set; } = "cust";

    public const string SectionName = "JWT";
}
