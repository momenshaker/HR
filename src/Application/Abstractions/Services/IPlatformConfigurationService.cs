using HR.Application.DTOs;

namespace HR.Application.Abstractions.Services;

/// <summary>
///     Provides access to the platform's runtime configuration and feature catalogue.
/// </summary>
public interface IPlatformConfigurationService
{
    /// <summary>
    ///     Retrieves the consolidated platform configuration including feature toggle state.
    /// </summary>
    Task<PlatformConfigurationDto> GetConfigurationAsync(CancellationToken cancellationToken = default);
}
