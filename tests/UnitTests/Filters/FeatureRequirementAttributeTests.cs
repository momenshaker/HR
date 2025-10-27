using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HR.Api.Filters;
using HR.Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HR.UnitTests.Filters;

public sealed class FeatureRequirementAttributeTests
{
    [Fact]
    public async Task OnActionExecutionAsync_WhenFeatureDisabled_ReturnsNotFound()
    {
        // Arrange
        var attribute = new FeatureRequirementAttribute(HrFeature.HrAnalytics);
        var context = CreateContext(features =>
        {
            features.EmployeeManagement = true;
            features.HrAnalytics = false;
        });

        // Act
        await attribute.OnActionExecutionAsync(context.ExecutingContext, context.Next);

        // Assert
        Assert.IsType<NotFoundResult>(context.ExecutingContext.Result);
        Assert.False(context.ActionInvoked);
    }

    [Fact]
    public async Task OnActionExecutionAsync_WhenFeatureEnabled_InvokesNext()
    {
        // Arrange
        var attribute = new FeatureRequirementAttribute(HrFeature.EmployeeManagement);
        var context = CreateContext(features =>
        {
            features.EmployeeManagement = true;
            features.HrAnalytics = false;
        });

        // Act
        await attribute.OnActionExecutionAsync(context.ExecutingContext, context.Next);

        // Assert
        Assert.Null(context.ExecutingContext.Result);
        Assert.True(context.ActionInvoked);
    }

    private static FeatureRequirementTestContext CreateContext(Action<HrPlatformOptions.FeatureToggleOptions> configureFeatures)
    {
        ArgumentNullException.ThrowIfNull(configureFeatures);

        var options = new HrPlatformOptions();
        configureFeatures(options.Features);

        var services = new ServiceCollection();
        services.AddSingleton<IOptionsSnapshot<HrPlatformOptions>>(new TestOptionsSnapshot(options));

        var serviceProvider = services.BuildServiceProvider();
        var httpContext = new DefaultHttpContext { RequestServices = serviceProvider };

        var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        var executingContext = new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object?>(), new object());

        var testContext = new FeatureRequirementTestContext(executingContext);
        return testContext;
    }

    private sealed class FeatureRequirementTestContext
    {
        public FeatureRequirementTestContext(ActionExecutingContext executingContext)
        {
            ExecutingContext = executingContext;
        }

        public ActionExecutingContext ExecutingContext { get; }

        public bool ActionInvoked { get; private set; }

        public ActionExecutionDelegate Next => async () =>
        {
            ActionInvoked = true;
            var executedContext = new ActionExecutedContext(ExecutingContext, new List<IFilterMetadata>(), ExecutingContext.Controller)
            {
                Result = ExecutingContext.Result
            };

            await Task.CompletedTask;
            return executedContext;
        };
    }

    private sealed class TestOptionsSnapshot : IOptionsSnapshot<HrPlatformOptions>
    {
        public TestOptionsSnapshot(HrPlatformOptions value)
        {
            Value = value;
        }

        public HrPlatformOptions Value { get; }

        public HrPlatformOptions Get(string? name) => Value;
    }
}
