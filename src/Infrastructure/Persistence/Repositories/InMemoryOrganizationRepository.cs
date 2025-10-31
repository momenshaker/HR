using System.Collections.Concurrent;
using System.Linq;
using HR.Application.Abstractions.Repositories;
using HR.Domain.Entities;

namespace HR.Infrastructure.Persistence.Repositories;

public sealed class InMemoryOrganizationRepository : IOrganizationRepository
{
    private readonly ConcurrentDictionary<Guid, Organization> _organizations = new();

    public Task<IReadOnlyCollection<Organization>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Organization> snapshot = _organizations.Values.ToList();
        return Task.FromResult(snapshot);
    }

    public Task<Organization?> GetByIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        _organizations.TryGetValue(organizationId, out var organization);
        return Task.FromResult(organization);
    }

    public Task<Organization> AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(organization);

        if (!_organizations.TryAdd(organization.Id, organization))
        {
            throw new InvalidOperationException($"An organization with id '{organization.Id}' already exists.");
        }

        return Task.FromResult(organization);
    }

    public Task<Organization?> UpdateAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(organization);

        if (!_organizations.ContainsKey(organization.Id))
        {
            return Task.FromResult<Organization?>(null);
        }

        _organizations[organization.Id] = organization;

        return Task.FromResult<Organization?>(organization);
    }

    public Task<bool> RemoveAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_organizations.TryRemove(organizationId, out _));
    }

    public Task<bool> ExistsByNameAsync(
        string name,
        Guid? excludingOrganizationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var trimmed = name.Trim();
        var exists = _organizations.Values.Any(organization =>
            (!excludingOrganizationId.HasValue || organization.Id != excludingOrganizationId.Value) &&
            string.Equals(organization.Name, trimmed, StringComparison.OrdinalIgnoreCase));

        return Task.FromResult(exists);
    }

    public Task<bool> ExistsByCodeAsync(
        string code,
        Guid? excludingOrganizationId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var normalizedCode = code.Trim().ToUpperInvariant();
        var exists = _organizations.Values.Any(organization =>
            (!excludingOrganizationId.HasValue || organization.Id != excludingOrganizationId.Value) &&
            string.Equals(organization.Code, normalizedCode, StringComparison.Ordinal));

        return Task.FromResult(exists);
    }
}
