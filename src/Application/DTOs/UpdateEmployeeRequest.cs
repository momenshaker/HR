using System.ComponentModel.DataAnnotations;
using HR.Application.Validation;

namespace HR.Application.DTOs;

/// <summary>
///     Incoming request payload for updating an existing employee.
/// </summary>
public sealed class UpdateEmployeeRequest : IValidatableRequest
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
    public DateOnly EmploymentStartDate { get; init; }

    [MaxLength(150)]
    public string JobTitle { get; init; } = string.Empty;

    [MaxLength(20)]
    [Phone]
    public string PhoneNumber { get; init; } = string.Empty;

    [MaxLength(40)]
    public string EmploymentType { get; init; } = string.Empty;

    public DateOnly? EmploymentEndDate { get; init; }

    public DateOnly? DateOfBirth { get; init; }

    [Required]
    public EmployeeDepartmentAssignmentRequest DepartmentAssignment { get; init; } = new();

    public EmployeeJobArchitectureRequest? JobArchitecture { get; init; }

    public IReadOnlyCollection<EmploymentContractRequest> Contracts { get; init; } = Array.Empty<EmploymentContractRequest>();

    public IReadOnlyCollection<EmployeeComplianceDocumentRequest> ComplianceDocuments { get; init; } = Array.Empty<EmployeeComplianceDocumentRequest>();

    public IReadOnlyCollection<EmployeeProfileDocumentRequest> ProfileDocuments { get; init; } = Array.Empty<EmployeeProfileDocumentRequest>();
}
