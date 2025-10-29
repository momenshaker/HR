using HR.Application.Validation;

namespace HR.Api.Contracts;

/// <summary>
///     Represents a request to update a user's role assignments.
/// </summary>
public sealed record UpdateUserRolesRequest(IReadOnlyCollection<string> Roles) : IValidatableRequest;
