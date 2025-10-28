namespace HR.Domain.Entities;

/// <summary>
///     Represents the lifecycle states that a subscription can occupy.
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    ///     The subscription is active and entitlements may be used.
    /// </summary>
    Active,

    /// <summary>
    ///     The subscription is temporarily inactive (for example due to non-payment).
    /// </summary>
    Inactive,

    /// <summary>
    ///     The subscription has been canceled and will not renew.
    /// </summary>
    Canceled,

    /// <summary>
    ///     Payment is overdue and access may be restricted.
    /// </summary>
    PastDue
}
