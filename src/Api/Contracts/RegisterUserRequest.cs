using HR.Application.Validation;

namespace HR.Api.Contracts;

/// <summary>
///     Represents a request to create a new identity user account.
/// </summary>
public sealed record RegisterUserRequest(
    string Email,
    string Password,
    string? CustomerId,
    IReadOnlyCollection<string>? Roles,
    IReadOnlyDictionary<string, string>? Claims) : IValidatableRequest;
