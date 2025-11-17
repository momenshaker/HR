using System.Security.Cryptography;
using System.Text;
using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.Common.Exceptions;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class LookupService : ILookupService
{
    private readonly ILookupRepository _lookupRepository;

    public LookupService(ILookupRepository lookupRepository)
    {
        _lookupRepository = lookupRepository;
    }

    /// <inheritdoc />
    public async Task<LookupCollectionDto> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var values = await _lookupRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var grouped = values
            .GroupBy(value => value.Category, StringComparer.OrdinalIgnoreCase)
            .Select(group => new LookupCategoryDto(
                group.Key,
                group
                    .OrderBy(value => value.SortOrder)
                    .ThenBy(value => value.DisplayName, StringComparer.OrdinalIgnoreCase)
                    .Select(value => value.ToDto())
                    .ToArray()))
            .OrderBy(category => category.Category, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var versionToken = ComputeVersionToken(values);
        return new LookupCollectionDto(versionToken, grouped);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<LookupValueDto>> GetByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category))
        {
            return Array.Empty<LookupValueDto>();
        }

        var normalizedCategory = NormalizeCategory(category);
        var values = await _lookupRepository
            .GetByCategoryAsync(normalizedCategory, cancellationToken)
            .ConfigureAwait(false);

        return values
            .OrderBy(value => value.SortOrder)
            .ThenBy(value => value.DisplayName, StringComparer.OrdinalIgnoreCase)
            .Select(value => value.ToDto())
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<LookupValueDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var value = await _lookupRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return value?.ToDto();
    }

    /// <inheritdoc />
    public async Task<LookupValueDto> CreateAsync(
        CreateLookupValueRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var normalizedCategory = NormalizeCategory(request.Category);
        var normalizedCode = NormalizeCode(request.Code);
        await EnsureUniqueAsync(normalizedCategory, normalizedCode, null, cancellationToken).ConfigureAwait(false);

        var sortOrder = await ResolveSortOrderAsync(
                normalizedCategory,
                request.SortOrder,
                cancellationToken)
            .ConfigureAwait(false);

        var utcNow = DateTime.UtcNow;
        var entity = request.ToEntity(normalizedCategory, normalizedCode, sortOrder, utcNow);
        var created = await _lookupRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        return created.ToDto();
    }

    /// <inheritdoc />
    public async Task<LookupValueDto?> UpdateAsync(
        Guid id,
        UpdateLookupValueRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existing = await _lookupRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existing is null)
        {
            return null;
        }

        var normalizedCategory = NormalizeCategory(request.Category);
        var normalizedCode = NormalizeCode(request.Code);

        var normalizedExistingCategory = NormalizeCategory(existing.Category);
        var normalizedExistingCode = NormalizeCode(existing.Code);

        var categoryChanged = !string.Equals(normalizedExistingCategory, normalizedCategory, StringComparison.Ordinal);
        var codeChanged = !string.Equals(normalizedExistingCode, normalizedCode, StringComparison.Ordinal);
        if (categoryChanged || codeChanged)
        {
            await EnsureUniqueAsync(normalizedCategory, normalizedCode, id, cancellationToken).ConfigureAwait(false);
        }

        var sortOrder = await ResolveSortOrderAsync(
                normalizedCategory,
                request.SortOrder,
                cancellationToken)
            .ConfigureAwait(false);

        var utcNow = DateTime.UtcNow;
        var updated = request.ApplyUpdates(existing, normalizedCategory, normalizedCode, sortOrder, utcNow);
        var persisted = await _lookupRepository.UpdateAsync(updated, cancellationToken).ConfigureAwait(false);
        return persisted?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _lookupRepository.RemoveAsync(id, cancellationToken);
    }

    private async Task EnsureUniqueAsync(
        string normalizedCategory,
        string normalizedCode,
        Guid? excludingId,
        CancellationToken cancellationToken)
    {
        var exists = await _lookupRepository
            .ExistsByCodeAsync(normalizedCategory, normalizedCode, excludingId, cancellationToken)
            .ConfigureAwait(false);

        if (exists)
        {
            throw new UniqueConstraintViolationException(
                "LookupValue",
                "Code",
                $"{normalizedCategory}:{normalizedCode}");
        }
    }

    private async Task<int> ResolveSortOrderAsync(
        string normalizedCategory,
        int? requestedSortOrder,
        CancellationToken cancellationToken)
    {
        if (requestedSortOrder.HasValue && requestedSortOrder.Value > 0)
        {
            return requestedSortOrder.Value;
        }

        var next = await _lookupRepository
            .GetNextSortOrderAsync(normalizedCategory, cancellationToken)
            .ConfigureAwait(false);

        return next <= 0 ? 1 : next;
    }

    private static string NormalizeCategory(string category)
    {
        return category.Trim().ToLowerInvariant();
    }

    private static string NormalizeCode(string code)
    {
        return code.Trim().ToUpperInvariant();
    }

    private static string ComputeVersionToken(IEnumerable<LookupValue> values)
    {
        var total = 0L;
        var count = 0;
        foreach (var value in values)
        {
            total ^= value.UpdatedAtUtc.Ticks;
            count++;
        }

        var payload = $"{total}:{count}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash);
    }
}
