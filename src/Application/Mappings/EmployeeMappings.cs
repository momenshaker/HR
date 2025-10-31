using System.ComponentModel.DataAnnotations;
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
        var contracts = employee.Contracts?.Select(contract => contract.ToDto()).ToArray() ?? Array.Empty<EmploymentContractDto>();
        var complianceDocuments = employee.ComplianceDocuments?
            .Select(document => document.ToDto())
            .ToArray() ?? Array.Empty<EmployeeComplianceDocumentDto>();

        var departmentIds = employee.DepartmentIds;
        var primaryDepartmentId = employee.PrimaryDepartmentId;

        return new EmployeeDto(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.JobTitle,
            primaryDepartmentId ?? Guid.Empty,
            departmentIds,
            employee.EmploymentStartDate,
            employee.EmploymentEndDate,
            employee.DateOfBirth,
            jobArchitecture.ToDto(),
            contracts,
            complianceDocuments);
    }

    public static Employee ToEntity(this CreateEmployeeRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var jobArchitecture = request.JobArchitecture.ToDomain();
        var contracts = request.Contracts.ToDomainContracts();
        var complianceDocuments = request.ComplianceDocuments.ToDomainComplianceDocuments();
        var departments = BuildDepartmentAssignments(
            request.DepartmentAssignment.PrimaryDepartmentId,
            request.DepartmentAssignment.SecondaryDepartmentIds);

        return new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            JobTitle = request.JobTitle.Trim(),
            EmploymentStartDate = request.EmploymentStartDate,
            EmploymentEndDate = request.EmploymentEndDate,
            DateOfBirth = request.DateOfBirth,
            Departments = departments,
            JobArchitecture = jobArchitecture,
            Contracts = contracts.ToList(),
            ComplianceDocuments = complianceDocuments.ToList()
        };
    }

    public static Employee ApplyUpdates(this UpdateEmployeeRequest request, Employee existingEmployee)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(existingEmployee);

        var jobArchitecture = request.JobArchitecture.ToDomain();
        var contracts = request.Contracts.ToDomainContracts();
        var complianceDocuments = request.ComplianceDocuments.ToDomainComplianceDocuments();
        var departments = BuildDepartmentAssignments(
            request.DepartmentAssignment.PrimaryDepartmentId,
            request.DepartmentAssignment.SecondaryDepartmentIds);

        return new Employee
        {
            Id = existingEmployee.Id,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim(),
            JobTitle = request.JobTitle.Trim(),
            EmploymentStartDate = request.EmploymentStartDate,
            EmploymentEndDate = request.EmploymentEndDate,
            DateOfBirth = request.DateOfBirth,
            Departments = departments,
            JobArchitecture = jobArchitecture,
            Contracts = contracts.ToList(),
            ComplianceDocuments = complianceDocuments.ToList()
        };
    }

    private static ICollection<EmployeeDepartment> BuildDepartmentAssignments(
        Guid primaryDepartmentId,
        IReadOnlyCollection<Guid> secondaryDepartmentIds)
    {
        if (primaryDepartmentId == Guid.Empty)
        {
            throw new ValidationException("A primary department must be supplied for the employee.");
        }

        var assignments = new List<EmployeeDepartment>
        {
            new()
            {
                DepartmentId = primaryDepartmentId,
                IsPrimary = true
            }
        };

        if (secondaryDepartmentIds is not null)
        {
            foreach (var departmentId in secondaryDepartmentIds.Where(id => id != Guid.Empty && id != primaryDepartmentId).Distinct())
            {
                assignments.Add(new EmployeeDepartment
                {
                    DepartmentId = departmentId,
                    IsPrimary = false
                });
            }
        }

        return assignments;
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

    private static IReadOnlyCollection<EmploymentContract> ToDomainContracts(this IEnumerable<EmploymentContractRequest>? requests)
    {
        if (requests is null)
        {
            return Array.Empty<EmploymentContract>();
        }

        return requests
            .Select(contract => new EmploymentContract
            {
                Id = contract.Id.GetValueOrDefault() == Guid.Empty ? Guid.NewGuid() : contract.Id.GetValueOrDefault(),
                ContractType = contract.ContractType?.Trim() ?? string.Empty,
                ContractNumber = contract.ContractNumber?.Trim() ?? string.Empty,
                Status = contract.Status?.Trim() ?? string.Empty,
                EffectiveFrom = contract.EffectiveFrom,
                EffectiveTo = contract.EffectiveTo,
                FtePercentage = contract.FtePercentage,
                WorkLocation = contract.WorkLocation?.Trim() ?? string.Empty,
                CompensationCurrency = contract.CompensationCurrency?.Trim() ?? string.Empty,
                AnnualCompensation = contract.AnnualCompensation,
                Notes = contract.Notes?.Trim() ?? string.Empty
            })
            .ToArray();
    }

    private static IReadOnlyCollection<EmployeeComplianceDocument> ToDomainComplianceDocuments(
        this IEnumerable<EmployeeComplianceDocumentRequest>? requests)
    {
        if (requests is null)
        {
            return Array.Empty<EmployeeComplianceDocument>();
        }

        return requests
            .Select(document => new EmployeeComplianceDocument
            {
                Id = document.Id.GetValueOrDefault() == Guid.Empty ? Guid.NewGuid() : document.Id.GetValueOrDefault(),
                DocumentType = document.DocumentType?.Trim() ?? string.Empty,
                ReferenceNumber = document.ReferenceNumber?.Trim() ?? string.Empty,
                Status = document.Status?.Trim() ?? string.Empty,
                IssuedOn = document.IssuedOn,
                ExpiresOn = document.ExpiresOn,
                StoragePath = document.StoragePath?.Trim() ?? string.Empty
            })
            .ToArray();
    }
}
