using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace HR.Api.Controllers;

/// <summary>
///     Provides CRUD endpoints for tenant-managed lookup values.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/lookups")]
[Authorize]
[AuditResource("LookupValue")]
[FeatureRequirement(HrFeature.PlatformServices)]
public sealed class LookupsController(ILookupService lookupService) : ControllerBase
{
    private readonly ILookupService _lookupService = lookupService;

    /// <summary>
    ///     Returns all lookup categories and values. Supports ETag cache validation.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(LookupCollectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var collection = await _lookupService.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var etag = FormatEtag(collection.VersionToken);

        if (etag is not null &&
            Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var requestedEtags) &&
            requestedEtags.Any(value => string.Equals(value, etag, StringComparison.Ordinal)))
        {
            Response.Headers.ETag = etag;
            return StatusCode(StatusCodes.Status304NotModified);
        }

        if (etag is not null)
        {
            Response.Headers.ETag = etag;
        }

        return Ok(collection);
    }

    /// <summary>
    ///     Returns lookup values for a specific category.
    /// </summary>
    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<LookupValueDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCategoryAsync(string category, CancellationToken cancellationToken)
    {
        var values = await _lookupService.GetByCategoryAsync(category, cancellationToken).ConfigureAwait(false);
        return Ok(values);
    }

    /// <summary>
    ///     Returns a lookup value by identifier.
    /// </summary>
    [HttpGet("value/{id:guid}")]
    [ProducesResponseType(typeof(LookupValueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var value = await _lookupService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return value is null ? NotFound() : Ok(value);
    }

    /// <summary>
    ///     Creates a lookup value.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    [ProducesResponseType(typeof(LookupValueDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> PostAsync(
        [FromBody] CreateLookupValueRequest request,
        CancellationToken cancellationToken)
    {
        var created = await _lookupService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    /// <summary>
    ///     Updates an existing lookup value.
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,HR")]
    [ProducesResponseType(typeof(LookupValueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(
        Guid id,
        [FromBody] UpdateLookupValueRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await _lookupService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Deletes a lookup value.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,HR")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _lookupService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }

    private static string? FormatEtag(string versionToken)
    {
        if (string.IsNullOrWhiteSpace(versionToken))
        {
            return null;
        }

        return versionToken.StartsWith('"', StringComparison.Ordinal)
            ? versionToken
            : $"\"{versionToken}\"";
    }
}
