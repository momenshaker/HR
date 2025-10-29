using HR.Application.Validation;
namespace HR.Application.DTOs;

/// <summary>
///     Request payload for updating an existing subscription.
/// </summary>
public sealed class UpdateSubscriptionRequest : IValidatableRequest
{
    public Guid? PlanId { get; set; }

    public int? Seats { get; set; }

    public string? Status { get; set; }

    public IDictionary<string, string>? Metadata { get; set; }

    public DateTime? RenewsAt { get; set; }
}