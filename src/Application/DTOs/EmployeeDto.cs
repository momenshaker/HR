namespace HR.Application.DTOs;

/// <summary>
///     Read model representing an employee.
/// </summary>
public sealed record EmployeeDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string JobTitle,
    Guid DepartmentId,
    DateOnly EmploymentStartDate,
    DateOnly? EmploymentEndDate,
    DateOnly? DateOfBirth,
    EmployeeJobArchitectureDto JobArchitecture,
    EmployeeDepartmentAlignmentDto DepartmentAlignment,
    IReadOnlyCollection<EmploymentContractDto> Contracts,
    IReadOnlyCollection<EmployeeComplianceDocumentDto> ComplianceDocuments);
