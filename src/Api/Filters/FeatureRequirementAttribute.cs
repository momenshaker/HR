using System;
using System.Threading.Tasks;
using HR.Infrastructure.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace HR.Api.Filters;

/// <summary>
///     Ensures a requested endpoint is only accessible when the associated feature is enabled.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class FeatureRequirementAttribute(HrFeature feature) : Attribute, IAsyncActionFilter
{
    private readonly HrFeature _feature = feature;

    /// <inheritdoc />
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);

        ArgumentNullException.ThrowIfNull(next);

        var options = context.HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<HrPlatformOptions>>();

        if (!options.Value.Features.IsEnabled(_feature))
        {
            context.Result = new NotFoundResult();
            return;
        }

        await next().ConfigureAwait(false);
    }
}
