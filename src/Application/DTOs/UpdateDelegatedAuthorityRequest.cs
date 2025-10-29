using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming payload for updating a delegated authority.
/// </summary>
public sealed class UpdateDelegatedAuthorityRequest : IValidatableRequest
{
    public Guid? GrantorEmployeeId { get; init; }

    public Guid? DelegateEmployeeId { get; init; }

    public Guid? GrantorPositionId { get; init; }

    public Guid? DelegatePositionId { get; init; }

    [Required]
    [MaxLength(200)]
    public string AuthorityScope { get; init; } = string.Empty;

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal? ApprovalLimit { get; init; }

    public DateTimeOffset GrantedOnUtc { get; init; }

    public DateTimeOffset? ExpiresOnUtc { get; init; }

    public DateTimeOffset? RevokedOnUtc { get; init; }

    public bool IsRevoked { get; init; }

    [MaxLength(1000)]
    public string Notes { get; init; } = string.Empty;
}