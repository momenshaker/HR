using System.Linq;
using HR.Application.DTOs;
using HR.Domain.Entities;

namespace HR.Application.Mappings;

/// <summary>
///     Helper methods for transforming <see cref="Employee" /> entities into transport-friendly representations.
/// </summary>
public static class EmployeeMappings
{
    public static EmployeeDto ToDto(this Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);

        var jobArchitecture = employee.JobArchitecture ?? EmployeeJobArchitecture.Empty;
        var departmentAlignmentDto = (employee.DepartmentAlignment ?? EmployeeDepartmentAlignment.Empty)
            .ToDto(employee.DepartmentId);
        var contracts = employee.Contracts?.Select(contract => contract.ToDto()).ToArray() ?? Array.Empty<EmploymentContractDto>();
        var complianceDocuments = employee.ComplianceDocuments?
            .Select(document => document.ToDto())
            .ToArray() ?? Array.Empty<EmployeeComplianceDocumentDto>();

        return new EmployeeDto(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.JobTitle,
            departmentAlignmentDto.PrimaryDepartmentId == Guid.Empty ? employee.DepartmentId : departmentAlignmentDto.PrimaryDepartmentId,
            employee.EmploymentStartDate,
            employee.EmploymentEndDate,
            employee.DateOfBirth,
            jobArchitecture.ToDto(),
            departmentAlignmentDto,
            contracts,
            complianceDocuments);
    }

    public static Employee ToEntity(this CreateEmployeeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var departmentAlignment = request.DepartmentAlignment.ToDomain(request.DepartmentId);
        var jobArchitecture = request.JobArchitecture.ToDomain();
        var contracts = request.Contracts.ToDomainContracts();
        var complianceDocuments = request.ComplianceDocuments.ToDomainComplianceDocuments();

        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            JobTitle = request.JobTitle.Trim(),
            DepartmentId = departmentAlignment.PrimaryDepartmentId,
            EmploymentStartDate = request.EmploymentStartDate,
            EmploymentEndDate = request.EmploymentEndDate,
            DateOfBirth = request.DateOfBirth,
            DepartmentAlignment = departmentAlignment,
            JobArchitecture = jobArchitecture,
            Contracts = contracts,
            ComplianceDocuments = complianceDocuments
        };
    }

    public static Employee ApplyUpdates(this UpdateEmployeeRequest request, Employee existingEmployee)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existingEmployee);

        var departmentAlignment = request.DepartmentAlignment.ToDomain(request.DepartmentId);
        var jobArchitecture = request.JobArchitecture.ToDomain();
        var contracts = request.Contracts.ToDomainContracts();
        var complianceDocuments = request.ComplianceDocuments.ToDomainComplianceDocuments();

        return new Employee
        {
            Id = existingEmployee.Id,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            JobTitle = request.JobTitle.Trim(),
            DepartmentId = departmentAlignment.PrimaryDepartmentId,
            EmploymentStartDate = request.EmploymentStartDate,
            EmploymentEndDate = request.EmploymentEndDate,
            DateOfBirth = request.DateOfBirth,
            DepartmentAlignment = departmentAlignment,
            JobArchitecture = jobArchitecture,
            Contracts = contracts,
            ComplianceDocuments = complianceDocuments
        };
    }

    private static EmployeeJobArchitectureDto ToDto(this EmployeeJobArchitecture jobArchitecture)
    {
        var architecture = jobArchitecture ?? EmployeeJobArchitecture.Empty;

        return new EmployeeJobArchitectureDto(
            architecture.JobFamily ?? string.Empty,
            architecture.JobFunction ?? string.Empty,
            architecture.JobLevel ?? string.Empty,
            architecture.JobCode ?? string.Empty,
            architecture.CareerTrack ?? string.Empty);
    }

    private static EmployeeDepartmentAlignmentDto ToDto(this EmployeeDepartmentAlignment alignment, Guid fallbackDepartmentId)
    {
        var resolvedAlignment = alignment ?? EmployeeDepartmentAlignment.Empty;
        var primaryDepartmentId = resolvedAlignment.PrimaryDepartmentId != Guid.Empty
            ? resolvedAlignment.PrimaryDepartmentId
            : fallbackDepartmentId;

        var secondaryDepartments = resolvedAlignment.SecondaryDepartmentIds?.Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray() ?? Array.Empty<Guid>();

        var reportingDepartmentId = resolvedAlignment.ReportingDepartmentId;
        if (reportingDepartmentId == Guid.Empty)
        {
            reportingDepartmentId = null;
        }

        return new EmployeeDepartmentAlignmentDto(
            primaryDepartmentId,
            secondaryDepartments,
            reportingDepartmentId,
            resolvedAlignment.CostCenter ?? string.Empty,
            resolvedAlignment.BusinessUnit ?? string.Empty);
    }

    private static EmploymentContractDto ToDto(this EmploymentContract contract)
    {
        ArgumentNullException.ThrowIfNull(contract);

        return new EmploymentContractDto(
            contract.Id,
            contract.ContractType ?? string.Empty,
            contract.ContractNumber ?? string.Empty,
            contract.Status ?? string.Empty,
            contract.EffectiveFrom,
            contract.EffectiveTo,
            contract.FtePercentage,
            contract.WorkLocation ?? string.Empty,
            contract.CompensationCurrency ?? string.Empty,
            contract.AnnualCompensation,
            contract.Notes ?? string.Empty);
    }

    private static EmployeeComplianceDocumentDto ToDto(this EmployeeComplianceDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new EmployeeComplianceDocumentDto(
            document.Id,
            document.DocumentType ?? string.Empty,
            document.ReferenceNumber ?? string.Empty,
            document.Status ?? string.Empty,
            document.IssuedOn,
            document.ExpiresOn,
            document.StoragePath ?? string.Empty);
    }

    private static EmployeeJobArchitecture ToDomain(this EmployeeJobArchitectureRequest? request)
    {
        if (request is null)
        {
            return EmployeeJobArchitecture.Empty;
        }

        return new EmployeeJobArchitecture
        {
            JobFamily = request.JobFamily?.Trim() ?? string.Empty,
            JobFunction = request.JobFunction?.Trim() ?? string.Empty,
            JobLevel = request.JobLevel?.Trim() ?? string.Empty,
            JobCode = request.JobCode?.Trim() ?? string.Empty,
            CareerTrack = request.CareerTrack?.Trim() ?? string.Empty
        };
    }

    private static EmployeeDepartmentAlignment ToDomain(this EmployeeDepartmentAlignmentRequest? request, Guid fallbackDepartmentId)
    {
        if (request is null)
        {
            return new EmployeeDepartmentAlignment
            {
                PrimaryDepartmentId = fallbackDepartmentId,
                SecondaryDepartmentIds = Array.Empty<Guid>(),
                ReportingDepartmentId = null,
                CostCenter = string.Empty,
                BusinessUnit = string.Empty
            };
        }

        var primaryDepartmentId = request.PrimaryDepartmentId != Guid.Empty
            ? request.PrimaryDepartmentId
            : fallbackDepartmentId;

        var secondaryDepartments = request.SecondaryDepartmentIds?.Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray() ?? Array.Empty<Guid>();

        var reportingDepartmentId = request.ReportingDepartmentId;
        if (reportingDepartmentId == Guid.Empty)
        {
            reportingDepartmentId = null;
        }

        return new EmployeeDepartmentAlignment
        {
            PrimaryDepartmentId = primaryDepartmentId,
            SecondaryDepartmentIds = secondaryDepartments,
            ReportingDepartmentId = reportingDepartmentId,
            CostCenter = request.CostCenter?.Trim() ?? string.Empty,
            BusinessUnit = request.BusinessUnit?.Trim() ?? string.Empty
        };
    }

    private static IReadOnlyCollection<EmploymentContract> ToDomainContracts(this IEnumerable<EmploymentContractRequest>? requests)
    {
        if (requests is null)
        {
            return Array.Empty<EmploymentContract>();
        }

        return requests
            .Select(contract => contract.ToDomain())
            .ToArray();
    }

    private static EmploymentContract ToDomain(this EmploymentContractRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new EmploymentContract
        {
            Id = request.Id ?? Guid.NewGuid(),
            ContractType = request.ContractType.Trim(),
            ContractNumber = request.ContractNumber?.Trim() ?? string.Empty,
            Status = request.Status?.Trim() ?? string.Empty,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            FtePercentage = request.FtePercentage,
            WorkLocation = request.WorkLocation?.Trim() ?? string.Empty,
            CompensationCurrency = request.CompensationCurrency?.Trim().ToUpperInvariant() ?? string.Empty,
            AnnualCompensation = request.AnnualCompensation,
            Notes = request.Notes?.Trim() ?? string.Empty
        };
    }

    private static IReadOnlyCollection<EmployeeComplianceDocument> ToDomainComplianceDocuments(this IEnumerable<EmployeeComplianceDocumentRequest>? requests)
    {
        if (requests is null)
        {
            return Array.Empty<EmployeeComplianceDocument>();
        }

        return requests
            .Select(document => document.ToDomain())
            .ToArray();
    }

    private static EmployeeComplianceDocument ToDomain(this EmployeeComplianceDocumentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return new EmployeeComplianceDocument
        {
            Id = request.Id ?? Guid.NewGuid(),
            DocumentType = request.DocumentType.Trim(),
            ReferenceNumber = request.ReferenceNumber.Trim(),
            Status = request.Status?.Trim() ?? string.Empty,
            IssuedOn = request.IssuedOn,
            ExpiresOn = request.ExpiresOn,
            StoragePath = request.StoragePath?.Trim() ?? string.Empty
        };
    }
}
