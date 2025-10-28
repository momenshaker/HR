using System.Collections.Generic;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides REST endpoints for managing delegated authority models.
/// </summary>
[ApiController]
[Route("api/delegated-authorities")]
[FeatureRequirement(HrFeature.DelegatedAuthority)]
public sealed class DelegatedAuthoritiesController(IDelegatedAuthorityService delegatedAuthorityService) : ControllerBase
{
    private readonly IDelegatedAuthorityService _delegatedAuthorityService = delegatedAuthorityService;

    /// <summary>
    ///     Retrieves all delegated authorities.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<DelegatedAuthorityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var authorities = await _delegatedAuthorityService.GetAsync(cancellationToken).ConfigureAwait(false);
        return Ok(authorities);
    }

    /// <summary>
    ///     Retrieves delegated authorities granted by the specified employee.
    /// </summary>
    [HttpGet("grantor/{employeeId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<DelegatedAuthorityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByGrantorAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var authorities = await _delegatedAuthorityService
            .GetByGrantorAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(authorities);
    }

    /// <summary>
    ///     Retrieves delegated authorities assigned to the specified employee.
    /// </summary>
    [HttpGet("delegate/{employeeId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<DelegatedAuthorityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDelegateAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var authorities = await _delegatedAuthorityService
            .GetByDelegateAsync(employeeId, cancellationToken)
            .ConfigureAwait(false);

        return Ok(authorities);
    }

    /// <summary>
    ///     Retrieves a delegated authority by identifier.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DelegatedAuthorityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var authority = await _delegatedAuthorityService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return authority is null ? NotFound() : Ok(authority);
    }

    /// <summary>
    ///     Creates a new delegated authority record.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(DelegatedAuthorityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateDelegatedAuthorityRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var created = await _delegatedAuthorityService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = created.Id }, created);
    }

    /// <summary>
    ///     Updates an existing delegated authority.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(DelegatedAuthorityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(Guid id, [FromBody] UpdateDelegatedAuthorityRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var updated = await _delegatedAuthorityService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    ///     Deletes a delegated authority record.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _delegatedAuthorityService.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        return deleted ? NoContent() : NotFound();
    }
}
