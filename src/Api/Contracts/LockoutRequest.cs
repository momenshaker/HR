using HR.Application.Validation;

namespace HR.Api.Contracts;

/// <summary>
///     Represents a request to update lockout settings for a user.
/// </summary>
public sealed record LockoutRequest(bool Enabled, DateTimeOffset? LockoutEnd) : IValidatableRequest;
