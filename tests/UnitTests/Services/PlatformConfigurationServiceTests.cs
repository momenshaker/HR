using HR.Application.DTOs;
using HR.Application.Services;
using HR.Application.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace HR.UnitTests.Services;

public sealed class PlatformConfigurationServiceTests
{
    [Fact]
    public async Task GetConfigurationAsync_WhenUsingEntityFramework_ReturnsDatabaseProvider()
    {
        // Arrange
        var options = new HrPlatformOptions
        {
            Data = new HrPlatformOptions.DataOptions
            {
                RepositoryProvider = HrPlatformOptions.DataOptions.RepositoryProviders.EntityFrameworkCore,
                Database = new HrPlatformOptions.DataOptions.DatabaseOptions
                {
                    Provider = HrPlatformOptions.DataOptions.DatabaseOptions.Providers.PostgreSql,
                    ConnectionString = "Host=localhost;Database=hr;"
                }
            }
        };

        options.Features.EmployeeManagement = true;
        options.Features.HrAnalytics = false;

        var service = new PlatformConfigurationService(new TestOptionsSnapshot(options));

        // Act
        var configuration = await service.GetConfigurationAsync();

        // Assert
        Assert.Equal(HrPlatformOptions.DataOptions.RepositoryProviders.EntityFrameworkCore, configuration.RepositoryProvider);
        Assert.Equal(HrPlatformOptions.DataOptions.DatabaseOptions.Providers.PostgreSql, configuration.DatabaseProvider);
        Assert.Equal(Enum.GetValues<HrFeature>().Length, configuration.Features.Count);

        var analyticsFeature = configuration.Features.Single(feature => feature.FeatureKey == "HrAnalytics");
        Assert.False(analyticsFeature.Enabled);
        Assert.Equal("HR Analytics", analyticsFeature.DisplayName);
    }

    [Fact]
    public async Task GetConfigurationAsync_WhenUsingInMemoryProvider_SetsDatabaseProviderToNotApplicable()
    {
        // Arrange
        var options = new HrPlatformOptions
        {
            Data = new HrPlatformOptions.DataOptions
            {
                RepositoryProvider = HrPlatformOptions.DataOptions.RepositoryProviders.InMemory
            }
        };

        var service = new PlatformConfigurationService(new TestOptionsSnapshot(options));

        // Act
        var configuration = await service.GetConfigurationAsync();

        // Assert
        Assert.Equal(HrPlatformOptions.DataOptions.RepositoryProviders.InMemory, configuration.RepositoryProvider);
        Assert.Equal("N/A", configuration.DatabaseProvider);
    }

    private sealed class TestOptionsSnapshot(HrPlatformOptions value) : IOptionsSnapshot<HrPlatformOptions>
    {
        public HrPlatformOptions Value { get; } = value;

        public HrPlatformOptions Get(string? name) => Value;
    }
}
