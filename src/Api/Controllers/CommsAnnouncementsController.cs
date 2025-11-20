using HR.Api.Authorization;
using HR.Api.Filters;
using HR.Application.Abstractions.Services;
using HR.Application.Configuration;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HR.Api.Controllers;

/// <summary>
///     Comms endpoints for organization and department announcements.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
[RolePermission(
    "notifications",
    readRoles: new[] { "Admin", "HR", "Manager", "Employee" },
    writeRoles: new[] { "Admin", "HR", "Manager" })]
[AuditResource("CommsAnnouncement")]
[FeatureRequirement(HrFeature.InternalCommunication)]
public sealed class CommsAnnouncementsController(ICommsService commsService, TimeProvider clock) : ControllerBase
{
    private readonly ICommsService _comms = commsService;
    private readonly TimeProvider _clock = clock;

    /// <summary>
    ///     List announcements with optional filters. Pinned first, newest first.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<CommsAnnouncementDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] Guid orgId,
        [FromQuery] Guid? departmentId,
        [FromQuery] Guid? unreadForEmployeeId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        if (orgId == Guid.Empty)
        {
            return BadRequest(new { error = "orgId is required" });
        }

        var result = await _comms.GetAnnouncementsAsync(orgId, departmentId, unreadForEmployeeId, page, pageSize, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    /// <summary>
    ///     Publish a new announcement.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CommsAnnouncementDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostAsync([FromBody] CreateCommsAnnouncementRequest request, CancellationToken cancellationToken)
    {
        var created = await _comms.PublishAsync(request, cancellationToken).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetAsync), new { orgId = created.OrganizationId }, created);
    }

    /// <summary>
    ///     Pin an announcement to the top of the list.
    /// </summary>
    [HttpPost("{id:guid}:pin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PinAsync(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _comms.PinAsync(id, cancellationToken).ConfigureAwait(false);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>
    ///     Remove pin from an announcement.
    /// </summary>
    [HttpPost("{id:guid}:unpin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnpinAsync(Guid id, CancellationToken cancellationToken)
    {
        var ok = await _comms.UnpinAsync(id, cancellationToken).ConfigureAwait(false);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>
    ///     Mark announcement as read for an employee.
    /// </summary>
    [HttpPost("{id:guid}:read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkReadAsync(Guid id, [FromQuery] Guid employeeId, CancellationToken cancellationToken)
    {
        if (employeeId == Guid.Empty)
        {
            return BadRequest(new { error = "employeeId is required" });
        }

        await _comms.MarkReadAsync(id, employeeId, _clock.GetUtcNow().UtcDateTime, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }
}


