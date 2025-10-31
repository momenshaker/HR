namespace HR.Api.Contracts;

public sealed record MeResponse(Guid UserId, string UserName, string Email, Guid? EmployeeId, IReadOnlyCollection<string> Roles);

