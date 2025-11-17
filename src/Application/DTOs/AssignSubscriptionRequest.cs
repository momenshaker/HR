namespace HR.Application.DTOs;

public sealed class AssignSubscriptionRequest
{
    public required IReadOnlyCollection<Guid> OrganizationIds { get; init; }
}
