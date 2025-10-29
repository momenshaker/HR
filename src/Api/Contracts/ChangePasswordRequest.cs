using HR.Application.Validation;

namespace HR.Api.Contracts;

/// <summary>
///     Request payload used to change the password for the currently authenticated user.
/// </summary>
public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword) : IValidatableRequest;
