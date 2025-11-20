namespace HR.Application.DTOs;

/// <summary>
///     Provides identifiers for the resources created during onboarding.
/// </summary>
public sealed record OnboardingResult(
    Guid CustomerId,
    Guid OrganizationId,
    Guid SubscriptionId,
    Guid AdminUserId,
    Guid AdminEmployeeId);
