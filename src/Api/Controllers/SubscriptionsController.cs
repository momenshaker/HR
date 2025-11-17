using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.Api.Filters;
using HR.Api.Middleware;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Provides CRUD operations for managing subscriptions and their invoices.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/subscriptions")]
[Authorize(Roles = "Admin,HR")]
[AuditResource("Subscription")]
[FeatureRequirement(HrFeature.PlatformServices)]
public sealed class SubscriptionsController(
    ISubscriptionService subscriptionService,
    IInvoiceService invoiceService,
    IOrganizationRepository organizationRepository) : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService = subscriptionService;
    private readonly IInvoiceService _invoiceService = invoiceService;
    private readonly IOrganizationRepository _organizationRepository = organizationRepository;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SubscriptionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        int page = 0,
        int pageSize = 25,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(0, page);
        pageSize = Math.Clamp(pageSize, 1, 200);

        var subscriptions = await _subscriptionService.GetAsync(cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(status))
        {
            subscriptions = subscriptions
                .Where(item => string.Equals(item.Status, status, StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }

        var paged = subscriptions
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToArray();

        Response.Headers["X-Total-Count"] = subscriptions.Count.ToString();
        Response.Headers["X-Page-Size"] = pageSize.ToString();

        return Ok(paged);
    }

    [HttpGet("{id:guid}", Name = "GetSubscriptionById")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var subscription = await _subscriptionService.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return subscription is null ? NotFound() : Ok(subscription);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var created = await _subscriptionService.CreateAsync(request, cancellationToken).ConfigureAwait(false);
        var version = HttpContext.GetRequestedApiVersion()?.ToString() ?? "1.0";
        return CreatedAtRoute("GetSubscriptionById", new { version, id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SubscriptionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutAsync(
        Guid id,
        [FromBody] UpdateSubscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var updated = await _subscriptionService.UpdateAsync(id, request, cancellationToken).ConfigureAwait(false);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var canceled = await _subscriptionService.CancelAsync(id, cancellationToken).ConfigureAwait(false);
        return canceled ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/organizations")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignOrganizationAsync(
        Guid id,
        [FromBody] AssignSubscriptionRequest request,
        CancellationToken cancellationToken = default)
    {
        var organizations = await Task.WhenAll(
            request.OrganizationIds.Select(orgId => _organizationRepository.GetByIdAsync(orgId, cancellationToken)));
        if (organizations.Any(org => org is null))
        {
            return NotFound();
        }

        var assigned = await _subscriptionService.SetOrganizationsAsync(id, request.OrganizationIds, cancellationToken).ConfigureAwait(false);
        return assigned ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/invoice")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestInvoiceAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceService.GetLatestAsync(id, cancellationToken).ConfigureAwait(false);
        return invoice is null ? NotFound() : Ok(invoice);
    }
}
