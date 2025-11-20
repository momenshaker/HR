using System.Security.Claims;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Identity;

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

    Task<(IdentityResult Result, Guid? UserId)> RegisterUserAsync(
        string email,
        string password,
        string? customerId = null,
        IEnumerable<string>? roles = null,
        IDictionary<string, string>? claims = null,
        Guid? employeeId = null,
        CancellationToken cancellationToken = default);

    Task<string?> GenerateEmailConfirmationTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IdentityResult> ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default);

    Task<string?> GeneratePasswordResetTokenAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IdentityResult> ResetPasswordAsync(Guid userId, string token, string newPassword, CancellationToken cancellationToken = default);

    Task<IdentityResult> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    Task<Guid?> GetUserIdByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IdentityResult> AddToRolesAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);

    Task<IdentityResult> RemoveFromRolesAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Claim>> GetClaimsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<IdentityResult> AddClaimsAsync(Guid userId, IDictionary<string, string> claims, CancellationToken cancellationToken = default);

    Task<IdentityResult> RemoveClaimsAsync(Guid userId, IDictionary<string, string> claims, CancellationToken cancellationToken = default);

    Task<IdentityResult> SetLockoutAsync(Guid userId, bool enabled, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default);
}
