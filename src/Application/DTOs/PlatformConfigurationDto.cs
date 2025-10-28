using System.Collections.Generic;

namespace HR.Application.DTOs;

/// <summary>
///     Represents the runtime platform configuration, including repository and feature toggle state.
/// </summary>
public sealed record PlatformConfigurationDto(
    string RepositoryProvider,
    string DatabaseProvider,
    IReadOnlyCollection<FeatureToggleStatusDto> Features);

/// <summary>
///     Describes the enabled state and purpose of a platform feature toggle.
/// </summary>
public sealed record FeatureToggleStatusDto(
    string FeatureKey,
    string DisplayName,
    string Usage,
    bool Enabled);
