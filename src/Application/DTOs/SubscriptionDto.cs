namespace HR.Application.DTOs;

/// <summary>
///     Read model representing a subscription.
/// </summary>
public sealed record SubscriptionDto(
    Guid Id,
    Guid PlanId,
    string Status,
    int Seats,
    DateTime CreatedAt,
    DateTime? CanceledAt,
    DateTime? RenewsAt,
    IReadOnlyDictionary<string, string> Metadata);
