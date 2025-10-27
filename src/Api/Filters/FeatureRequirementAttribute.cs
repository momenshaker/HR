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
public sealed class FeatureRequirementAttribute : Attribute, IAsyncActionFilter
{
    private readonly HrFeature _feature;

    public FeatureRequirementAttribute(HrFeature feature)
    {
        _feature = feature;
    }

    /// <inheritdoc />
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (next is null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        var options = context.HttpContext.RequestServices.GetRequiredService<IOptionsSnapshot<HrPlatformOptions>>();

        if (!options.Value.Features.IsEnabled(_feature))
        {
            context.Result = new NotFoundResult();
            return;
        }

        await next().ConfigureAwait(false);
    }
}
