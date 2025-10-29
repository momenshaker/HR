using HR.Application.Validation;

namespace HR.Api.Contracts;

/// <summary>
///     Request payload used to generate a new email confirmation token.
/// </summary>
public sealed record ResendConfirmationRequest(Guid UserId) : IValidatableRequest;
