using HR.Application.Validation;

namespace HR.Api.Contracts;

/// <summary>
///     Request payload used to reset a user's password using a verification token.
/// </summary>
public sealed record ResetPasswordRequest(Guid UserId, string Token, string NewPassword) : IValidatableRequest;
