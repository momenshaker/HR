using HR.Application.Validation;

namespace HR.Api.Contracts;

public sealed record RefreshRequest(string RefreshToken) : IValidatableRequest;

