using HR.Application.Validation;

namespace HR.Api.Contracts;

/// <summary>
///     Request used to initiate password reset flow for a user.
/// </summary>
public sealed record ForgotPasswordRequest(string Email) : IValidatableRequest;
