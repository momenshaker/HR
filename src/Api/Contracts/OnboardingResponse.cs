namespace HR.Api.Contracts;

/// <summary>
///     Response payload returned after a successful onboarding run.
/// </summary>
public sealed record OnboardingResponse(
    Guid CustomerId,
    Guid OrganizationId,
    Guid SubscriptionId,
    Guid AdminUserId,
    Guid AdminEmployeeId);
