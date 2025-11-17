namespace HR.Application.DTOs;

/// <summary>
///     Defines a subscription plan that can be purchased or assigned.
/// </summary>
public sealed record PlanDto(
    Guid Id,
    string Code,
    string Name,
    string Description,
    decimal Price,
    string BillingInterval,
    IReadOnlyCollection<PlanEntitlementDto> Entitlements);

/// <summary>
///     Describes a quantitative entitlement that is part of a plan.
/// </summary>
public sealed record PlanEntitlementDto(
    string FeatureKey,
    string DisplayName,
    string Description,
    string MeasurementUnit,
    int? Quantity);

/// <summary>
///     Request payload for creating a plan.
/// </summary>
public sealed class CreatePlanRequest
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required string BillingInterval { get; init; }
    public required IReadOnlyCollection<PlanEntitlementRequest> Entitlements { get; init; }
}

/// <summary>
///     Request payload for updating a plan.
/// </summary>
public sealed class UpdatePlanRequest
{
    public string? Code { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public decimal? Price { get; init; }
    public string? BillingInterval { get; init; }
    public IReadOnlyCollection<PlanEntitlementRequest>? Entitlements { get; init; }
}

/// <summary>
///     Describes an entitlement supplied as part of a plan payload.
/// </summary>
public sealed class PlanEntitlementRequest
{
    public required string FeatureKey { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
    public required string MeasurementUnit { get; init; }
    public int? Quantity { get; init; }
}
