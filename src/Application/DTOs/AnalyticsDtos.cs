using System;
using System.Collections.Generic;

namespace HR.Application.DTOs;

public sealed record HeadcountItemDto(Guid DepartmentId, string DepartmentName, int Count);

public sealed record UtilizationPeriodDto(DateOnly PeriodStart, DateOnly PeriodEnd, decimal ApprovedHours, decimal CapacityHours, decimal UtilizationRate);

public sealed record LeaveUsageItemDto(string LeaveType, int Days);

public sealed record PayrollRunTotalsDto(Guid RunId, DateOnly PeriodStart, DateOnly PeriodEnd, decimal TotalGross, decimal TotalNet);

public sealed record DepartmentPayrollTotalsDto(Guid DepartmentId, string DepartmentName, decimal TotalGross, decimal TotalNet);

public sealed record PayrollTotalsResponseDto(IReadOnlyCollection<PayrollRunTotalsDto> Runs, IReadOnlyCollection<DepartmentPayrollTotalsDto> ByDepartment);

public sealed record StageCountDto(string Stage, int Count);

public sealed record TrainingComplianceDto(Guid OrganizationId, int MandatoryCourseCount, int ObservedEmployeeCount, int CompliantEmployeeCount, decimal ComplianceRate);

