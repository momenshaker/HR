using System.ComponentModel.DataAnnotations;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating an existing employee.
/// </summary>
public sealed class UpdateEmployeeRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;

    [Required]
    public Guid DepartmentId { get; init; }

    [Required]
    public DateOnly EmploymentStartDate { get; init; }

    [MaxLength(150)]
    public string JobTitle { get; init; } = string.Empty;

    public DateOnly? EmploymentEndDate { get; init; }

    public DateOnly? DateOfBirth { get; init; }

    public EmployeeDepartmentAlignmentRequest? DepartmentAlignment { get; init; }

    public EmployeeJobArchitectureRequest? JobArchitecture { get; init; }

    public IReadOnlyCollection<EmploymentContractRequest> Contracts { get; init; } = Array.Empty<EmploymentContractRequest>();

    public IReadOnlyCollection<EmployeeComplianceDocumentRequest> ComplianceDocuments { get; init; } = Array.Empty<EmployeeComplianceDocumentRequest>();
}
