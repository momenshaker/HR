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
    Guid? PositionId,
    Guid? ReportsToEmployeeId,
    string PhoneNumber,
    string EmploymentType,
    Guid PrimaryDepartmentId,
    string PrimaryDepartmentName,
    IReadOnlyCollection<Guid> DepartmentIds,
    DateOnly EmploymentStartDate,
    DateOnly? EmploymentEndDate,
    DateOnly? DateOfBirth,
    EmployeeJobArchitectureDto JobArchitecture,
    IReadOnlyCollection<EmploymentContractDto> Contracts,
    IReadOnlyCollection<EmployeeComplianceDocumentDto> ComplianceDocuments,
    IReadOnlyCollection<EmployeeProfileDocumentDto> ProfileDocuments);
