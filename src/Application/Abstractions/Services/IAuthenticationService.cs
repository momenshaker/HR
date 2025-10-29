using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Provides authentication capabilities for issuing access tokens.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    ///     Validates the supplied credentials and issues an access token when successful.
    /// </summary>
    /// <param name="email">The user email attempting to authenticate.</param>
    /// <param name="password">The password associated with the supplied email.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>
    ///     An <see cref="AuthenticationResult"/> when authentication succeeds; otherwise <c>null</c> when the credentials are invalid.
    /// </returns>
    Task<AuthenticationResult?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}
