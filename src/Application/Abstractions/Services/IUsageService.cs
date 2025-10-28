using HR.Application.Configuration;
using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Application service abstraction for tracking feature usage against subscription entitlements.
/// </summary>
public interface IUsageService
{
    Task<UsageSummaryDto> RecordUsageAsync(Guid subscriptionId, HrFeature feature, int quantity, CancellationToken cancellationToken = default);

    Task<UsageSummaryDto> GetUsageAsync(Guid subscriptionId, HrFeature feature, CancellationToken cancellationToken = default);

    Task ResetUsageAsync(Guid subscriptionId, HrFeature feature, CancellationToken cancellationToken = default);
}
