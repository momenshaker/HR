using System;
using System.Collections.Generic;
using HR.Application.Configuration;
using HR.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace HR.UnitTests.Infrastructure.Options;

public sealed class DatabaseConfigurationTests
{
    [Fact]
    public void From_WhenConnectionStringProvided_ReturnsConfiguration()
    {
        // Arrange
        var databaseOptions = new HrPlatformOptions.DataOptions.DatabaseOptions
        {
            Provider = HrPlatformOptions.DataOptions.DatabaseOptions.Providers.PostgreSql,
            ConnectionString = "Host=localhost;Database=hr;Username=postgres;Password=secret;",
            EnableDetailedErrors = true,
            EnableSensitiveDataLogging = true
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .Build();

        // Act
        var configurationResult = DatabaseConfiguration.From(databaseOptions, configuration);

        // Assert
        Assert.Equal(HrPlatformOptions.DataOptions.DatabaseOptions.Providers.PostgreSql, configurationResult.Provider);
        Assert.Equal(databaseOptions.ConnectionString, configurationResult.ConnectionString);
        Assert.True(configurationResult.EnableDetailedErrors);
        Assert.True(configurationResult.EnableSensitiveDataLogging);
    }

    [Fact]
    public void From_WhenConnectionStringNameProvided_ResolvesFromRootSection()
    {
        // Arrange
        var databaseOptions = new HrPlatformOptions.DataOptions.DatabaseOptions
        {
            Provider = HrPlatformOptions.DataOptions.DatabaseOptions.Providers.SqlServer,
            ConnectionStringName = "HrDatabase"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:HrDatabase"] = "Server=.;Database=Hr;Trusted_Connection=True;"
            })
            .Build();

        // Act
        var configurationResult = DatabaseConfiguration.From(databaseOptions, configuration);

        // Assert
        Assert.Equal("Server=.;Database=Hr;Trusted_Connection=True;", configurationResult.ConnectionString);
    }

    [Fact]
    public void From_WhenConnectionStringMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        var databaseOptions = new HrPlatformOptions.DataOptions.DatabaseOptions();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => DatabaseConfiguration.From(databaseOptions, configuration));
    }
}
