using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Mapping helpers for <see cref="DelegatedAuthority" /> entities.
/// </summary>
public static class DelegatedAuthorityMappings
{
    public static DelegatedAuthorityDto ToDto(this DelegatedAuthority authority)
    {
        ArgumentNullException.ThrowIfNull(authority);

        return new DelegatedAuthorityDto(
            authority.Id,
            authority.GrantorEmployeeId,
            authority.DelegateEmployeeId,
            authority.GrantorPositionId,
            authority.DelegatePositionId,
            authority.AuthorityScope,
            authority.ApprovalLimit,
            authority.GrantedOnUtc,
            authority.ExpiresOnUtc,
            authority.RevokedOnUtc,
            authority.IsRevoked,
            authority.Notes);
    }

    public static DelegatedAuthority ToEntity(this CreateDelegatedAuthorityRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new DelegatedAuthority
        {
            Id = Guid.NewGuid(),
            GrantorEmployeeId = request.GrantorEmployeeId,
            DelegateEmployeeId = request.DelegateEmployeeId,
            GrantorPositionId = request.GrantorPositionId,
            DelegatePositionId = request.DelegatePositionId,
            AuthorityScope = request.AuthorityScope.Trim(),
            ApprovalLimit = request.ApprovalLimit,
            GrantedOnUtc = request.GrantedOnUtc,
            ExpiresOnUtc = request.ExpiresOnUtc,
            RevokedOnUtc = null,
            IsRevoked = false,
            Notes = request.Notes.Trim()
        };
    }

    public static DelegatedAuthority ApplyUpdates(this UpdateDelegatedAuthorityRequest request, DelegatedAuthority existing)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existing);

        return new DelegatedAuthority
        {
            Id = existing.Id,
            GrantorEmployeeId = request.GrantorEmployeeId,
            DelegateEmployeeId = request.DelegateEmployeeId,
            GrantorPositionId = request.GrantorPositionId,
            DelegatePositionId = request.DelegatePositionId,
            AuthorityScope = request.AuthorityScope.Trim(),
            ApprovalLimit = request.ApprovalLimit,
            GrantedOnUtc = request.GrantedOnUtc,
            ExpiresOnUtc = request.ExpiresOnUtc,
            RevokedOnUtc = request.RevokedOnUtc,
            IsRevoked = request.IsRevoked,
            Notes = request.Notes.Trim()
        };
    }
}
