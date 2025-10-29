using HR.Application.Validation;

namespace HR.Api.Contracts;

/// <summary>
///     Represents a request to update user claims.
/// </summary>
public sealed record UpdateUserClaimsRequest(IReadOnlyDictionary<string, string> Claims) : IValidatableRequest;
