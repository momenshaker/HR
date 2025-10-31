using HR.Application.Validation;

namespace HR.Api.Contracts;

public sealed record RegisterEmployeeRequest(string Email, string UserName, string Password, Guid EmployeeId) : IValidatableRequest;

