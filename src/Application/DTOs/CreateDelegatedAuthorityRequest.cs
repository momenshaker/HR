using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for creating a delegated authority.
/// </summary>
public sealed class CreateDelegatedAuthorityRequest : IValidatableRequest
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

    public DateTimeOffset GrantedOnUtc { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ExpiresOnUtc { get; init; }

    [MaxLength(1000)]
    public string Notes { get; init; } = string.Empty;
}