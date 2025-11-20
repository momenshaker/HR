using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Coordinates the tenant onboarding experience.
/// </summary>
public interface IOnboardingService
{
    Task<OnboardingResult> StartAsync(OnboardingRequest request, CancellationToken cancellationToken = default);
}
