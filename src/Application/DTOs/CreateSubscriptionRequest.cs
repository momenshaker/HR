using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Request payload for provisioning a new subscription.
/// </summary>
public sealed class CreateSubscriptionRequest : IValidatableRequest
{
    [Required]
    public Guid PlanId { get; set; }

    [Range(1, int.MaxValue)]
    public int Seats { get; set; }

    public Guid? CustomerId { get; set; }

    [Range(0, 30)]
    public int? TrialPeriodDays { get; set; }

    public string? PaymentMethodId { get; set; }

    public IDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
