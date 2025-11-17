using System;
using System.Collections.Generic;
using System.Linq;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class PlanService : IPlanService
{
    private readonly List<PlanDto> _plans = new()
    {
        new PlanDto(
            Guid.Parse("4ec1e9c4-2a1d-4f7b-94e5-7d7c5f5d2a2c"),
            "starter",
            "Starter",
            "Lightweight subscription for small HR teams.",
            0m,
            "Monthly",
            new[]
            {
                new PlanEntitlementDto("core.users", "Active employee seats", "Up to 25 active employees", "seats", 25),
                new PlanEntitlementDto("storage.documents", "Document storage", "Shared document storage quota", "GB", 50)
            }),
        new PlanDto(
            Guid.Parse("5d75c833-3e0e-44e8-b6d0-0a1b2c3d4e5f"),
            "professional",
            "Professional",
            "Full payroll and automation for growing teams.",
            299m,
            "Monthly",
            new[]
            {
                new PlanEntitlementDto("core.users", "Active employee seats", "Up to 250 active employees", "seats", 250),
                new PlanEntitlementDto("automation.workflows", "Automation workflows", "Automation runs per month", "runs", 500)
            }),
        new PlanDto(
            Guid.Parse("7b0edf6a-1f4a-4b6b-9d78-8bd3e2a4c2b7"),
            "enterprise",
            "Enterprise",
            "Unlimited seats + dedicated success partner.",
            999m,
            "Monthly",
            new[]
            {
                new PlanEntitlementDto("core.users", "Active employee seats", "Unlimited active employees", "seats", null),
                new PlanEntitlementDto("success.manager", "Dedicated success manager", "Quarterly strategic reviews", "engagements", 1)
            })
    };

    private readonly object _lock = new();

    /// <inheritdoc />
    public Task<IReadOnlyCollection<PlanDto>> GetPlansAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult((IReadOnlyCollection<PlanDto>)_plans.ToArray());
        }
    }

    /// <inheritdoc />
    public Task<PlanDto> CreatePlanAsync(CreatePlanRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var plan = new PlanDto(
            Guid.NewGuid(),
            request.Code.Trim(),
            request.Name.Trim(),
            request.Description.Trim(),
            request.Price,
            request.BillingInterval.Trim(),
            ToEntitlements(request.Entitlements));

        lock (_lock)
        {
            _plans.Add(plan);
        }

        return Task.FromResult(plan);
    }

    /// <inheritdoc />
    public Task<PlanDto?> UpdatePlanAsync(Guid id, UpdatePlanRequest request, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var index = _plans.FindIndex(plan => plan.Id == id);
            if (index == -1)
            {
                return Task.FromResult<PlanDto?>(null);
            }

            var existing = _plans[index];
            var updated = existing with
            {
                Code = request.Code?.Trim() ?? existing.Code,
                Name = request.Name?.Trim() ?? existing.Name,
                Description = request.Description?.Trim() ?? existing.Description,
                Price = request.Price ?? existing.Price,
                BillingInterval = request.BillingInterval?.Trim() ?? existing.BillingInterval,
                Entitlements = request.Entitlements is not null
                    ? ToEntitlements(request.Entitlements)
                    : existing.Entitlements
            };

            _plans[index] = updated;
            return Task.FromResult<PlanDto?>(updated);
        }
    }

    /// <inheritdoc />
    public Task<bool> DeletePlanAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_plans.RemoveAll(plan => plan.Id == id) > 0);
        }
    }

    private static IReadOnlyCollection<PlanEntitlementDto> ToEntitlements(
        IReadOnlyCollection<PlanEntitlementRequest> requests)
    {
        return requests
            .Select(request => new PlanEntitlementDto(
                request.FeatureKey.Trim(),
                request.DisplayName.Trim(),
                request.Description.Trim(),
                request.MeasurementUnit.Trim(),
                request.Quantity))
            .ToArray();
    }
}
