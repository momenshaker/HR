using HR.Application.Validation;

namespace HR.Api.Contracts;

/// <summary>
///     Request payload used to confirm a user's email address.
/// </summary>
public sealed record ConfirmEmailRequest(Guid UserId, string Token) : IValidatableRequest;
