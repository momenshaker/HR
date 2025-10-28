using System;
using System.Collections.Generic;
using System.Linq;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="SelfServiceAccount" /> entities.
/// </summary>
public static class SelfServiceAccountMappings
{
    public static SelfServiceAccountDto ToDto(this SelfServiceAccount account)
    {
        ArgumentNullException.ThrowIfNull(account);

        return new SelfServiceAccountDto(
            account.Id,
            account.EmployeeId,
            account.Email,
            account.OAuthProvider,
            account.ExternalIdentifier,
            account.IsMfaEnabled,
            account.IsLocked,
            account.CreatedOnUtc,
            account.UpdatedOnUtc,
            account.LastSignInUtc,
            account.FeatureAccess.AsReadOnly());
    }

    public static SelfServiceAccount ToEntity(this CreateSelfServiceAccountRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new SelfServiceAccount
        {
            Id = Guid.NewGuid(),
            EmployeeId = request.EmployeeId,
            Email = request.Email.Trim().ToLowerInvariant(),
            OAuthProvider = request.OAuthProvider.Trim(),
            ExternalIdentifier = request.ExternalIdentifier.Trim(),
            IsMfaEnabled = request.IsMfaEnabled,
            IsLocked = request.IsLocked,
            CreatedOnUtc = DateTimeOffset.UtcNow,
            UpdatedOnUtc = DateTimeOffset.UtcNow,
            LastSignInUtc = request.LastSignInUtc,
            FeatureAccess = Normalize(request.FeatureAccess)
        };
    }

    public static SelfServiceAccount ApplyUpdates(this UpdateSelfServiceAccountRequest request, SelfServiceAccount existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new SelfServiceAccount
        {
            Id = existing.Id,
            EmployeeId = existing.EmployeeId,
            Email = request.Email.Trim().ToLowerInvariant(),
            OAuthProvider = request.OAuthProvider.Trim(),
            ExternalIdentifier = request.ExternalIdentifier.Trim(),
            IsMfaEnabled = request.IsMfaEnabled,
            IsLocked = request.IsLocked,
            CreatedOnUtc = existing.CreatedOnUtc,
            UpdatedOnUtc = DateTimeOffset.UtcNow,
            LastSignInUtc = request.LastSignInUtc,
            FeatureAccess = Normalize(request.FeatureAccess)
        };
    }

    private static List<string> Normalize(IEnumerable<string> values)
    {
        return values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
