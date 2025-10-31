using HR.Application.Abstractions.Repositories;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;
using HR.Application.Mappings;
using HR.Domain.Entities;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class EmployeeService : IEmployeeService, IEmployeeSearchService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeeService(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyCollection<EmployeeDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        return employees.Select(employee => employee.ToDto()).ToArray();
    }

    /// <inheritdoc />
    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        return employee?.ToDto();
    }

    /// <inheritdoc />
    public async Task<PaginatedResponse<EmployeeDto>> SearchAsync(
        EmployeeSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var allEmployees = await _employeeRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var departments = await _departmentRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        var departmentsById = departments.ToDictionary(department => department.Id);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var filteredEmployees = ApplyFilters(allEmployees, request, today, departmentsById);
        var orderedEmployees = ApplySorting(filteredEmployees, request, today).ToList();

        var totalCount = orderedEmployees.Count;
        var skip = (request.PageNumber - 1) * request.PageSize;
        var pageItems = orderedEmployees
            .Skip(skip)
            .Take(request.PageSize)
            .Select(employee => employee.ToDto())
            .ToArray();

        return new PaginatedResponse<EmployeeDto>(request.PageNumber, request.PageSize, totalCount, pageItems);
    }

    public async Task<IReadOnlyCollection<EmployeeDto>> GetByDepartmentAsync(
        Guid departmentId,
        CancellationToken cancellationToken = default)
    {
        if (departmentId == Guid.Empty)
        {
            throw new ArgumentException("Department identifier must be provided.", nameof(departmentId));
        }

        var employees = await _employeeRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);

        return employees
            .Where(employee => employee.DepartmentIds.Contains(departmentId))
            .Select(employee => employee.ToDto())
            .ToArray();
    }

    private static IEnumerable<Employee> ApplyFilters(
        IEnumerable<Employee> employees,
        EmployeeSearchRequest request,
        DateOnly referenceDate,
        IReadOnlyDictionary<Guid, Department> departmentsById)
    {
        ArgumentNullException.ThrowIfNull(employees);

        IEnumerable<Employee> filteredEmployees = employees;

        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            var query = request.Query.Trim();
            filteredEmployees = filteredEmployees.Where(employee =>
                employee.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                employee.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                employee.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                employee.JobTitle.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        if (request.DepartmentId.HasValue)
        {
            filteredEmployees = filteredEmployees.Where(employee => employee.DepartmentIds.Contains(request.DepartmentId.Value));
        }

        if (request.OrganizationId.HasValue)
        {
            var organizationId = request.OrganizationId.Value;
            filteredEmployees = filteredEmployees.Where(employee => employee.DepartmentIds.Any(departmentId =>
                departmentsById.TryGetValue(departmentId, out var department) &&
                department.OrganizationId == organizationId));
        }

        if (!string.IsNullOrWhiteSpace(request.JobTitle))
        {
            var jobTitle = request.JobTitle.Trim();
            filteredEmployees = filteredEmployees.Where(employee =>
                employee.JobTitle.Contains(jobTitle, StringComparison.OrdinalIgnoreCase));
        }

        if (request.IsActive.HasValue)
        {
            filteredEmployees = filteredEmployees.Where(employee => IsActive(employee, referenceDate) == request.IsActive.Value);
        }

        if (request.HiredFrom.HasValue)
        {
            filteredEmployees = filteredEmployees.Where(employee => employee.EmploymentStartDate >= request.HiredFrom.Value);
        }

        if (request.HiredTo.HasValue)
        {
            filteredEmployees = filteredEmployees.Where(employee => employee.EmploymentStartDate <= request.HiredTo.Value);
        }

        if (request.EmploymentEndFrom.HasValue)
        {
            filteredEmployees = filteredEmployees.Where(employee =>
                employee.EmploymentEndDate.HasValue && employee.EmploymentEndDate.Value >= request.EmploymentEndFrom.Value);
        }

        if (request.EmploymentEndTo.HasValue)
        {
            filteredEmployees = filteredEmployees.Where(employee =>
                employee.EmploymentEndDate.HasValue && employee.EmploymentEndDate.Value <= request.EmploymentEndTo.Value);
        }

        return filteredEmployees;
    }

    /// <inheritdoc />
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var entity = request.ToEntity();
        var createdEmployee = await _employeeRepository.AddAsync(entity, cancellationToken).ConfigureAwait(false);

        return createdEmployee.ToDto();
    }

    /// <inheritdoc />
    public async Task<EmployeeDto?> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingEmployee = await _employeeRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (existingEmployee is null)
        {
            return null;
        }

        var updatedEntity = request.ApplyUpdates(existingEmployee);
        var persistedEmployee = await _employeeRepository.UpdateAsync(updatedEntity, cancellationToken).ConfigureAwait(false);

        return persistedEmployee?.ToDto();
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _employeeRepository.RemoveAsync(id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EmployeeWorkforceSnapshotDto> GetWorkforceSnapshotAsync(
        CancellationToken cancellationToken = default)
    {
        var employeesTask = _employeeRepository.GetAllAsync(cancellationToken);
        var departmentsTask = _departmentRepository.GetAllAsync(cancellationToken);

        await Task.WhenAll(employeesTask, departmentsTask).ConfigureAwait(false);

        var employees = (await employeesTask.ConfigureAwait(false)).ToArray();
        var departments = await departmentsTask.ConfigureAwait(false);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var thirtyDaysAgo = today.AddDays(-30);
        var thirtyDaysAhead = today.AddDays(30);

        var activeEmployees = employees.Where(employee => IsActive(employee, today)).ToArray();
        var totalEmployees = employees.Length;
        var inactiveEmployees = totalEmployees - activeEmployees.Length;

        var newHiresLast30Days = activeEmployees.Count(employee => employee.EmploymentStartDate >= thirtyDaysAgo);
        var departuresLast30Days = employees.Count(employee =>
            employee.EmploymentEndDate.HasValue &&
            employee.EmploymentEndDate.Value >= thirtyDaysAgo &&
            employee.EmploymentEndDate.Value <= today);
        var upcomingDepartures = employees.Count(employee =>
            employee.EmploymentEndDate.HasValue &&
            employee.EmploymentEndDate.Value > today &&
            employee.EmploymentEndDate.Value <= thirtyDaysAhead);

        var averageTenureYears = activeEmployees.Length == 0
            ? 0d
            : Math.Round(activeEmployees.Average(employee => CalculateTenureInYears(employee, today)), 2);

        var departmentLookup = departments.ToDictionary(department => department.Id, department => department.Name);
        var employeesPerDepartment = employees
            .SelectMany(employee => employee.DepartmentIds.Select(departmentId => (employee.Id, departmentId)))
            .GroupBy(item => item.departmentId)
            .ToDictionary(group => group.Key, group => group.Count());

        var departmentHeadcounts = activeEmployees
            .GroupBy(employee => employee.PrimaryDepartmentId)
            .Select(group =>
            {
                var departmentName = departmentLookup.TryGetValue(group.Key, out var name)
                    ? name
                    : "Unknown";
                var totalEmployeesInDepartment = employeesPerDepartment.TryGetValue(group.Key, out var count)
                    ? count
                    : group.Count();

                return new EmployeeDepartmentHeadcountDto(
                    group.Key,
                    departmentName,
                    group.Count(),
                    totalEmployeesInDepartment);
            })
            .OrderByDescending(dto => dto.ActiveEmployees)
            .ThenBy(dto => dto.DepartmentName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new EmployeeWorkforceSnapshotDto(
            totalEmployees,
            activeEmployees.Length,
            inactiveEmployees,
            newHiresLast30Days,
            departuresLast30Days,
            upcomingDepartures,
            averageTenureYears,
            departmentHeadcounts);
    }

    private static IEnumerable<Employee> ApplySorting(
        IEnumerable<Employee> employees,
        EmployeeSearchRequest request,
        DateOnly referenceDate)
    {
        return request.SortBy switch
        {
            EmployeeSortField.EmploymentStartDate when request.SortDirection == SortDirection.Ascending =>
                employees.OrderBy(employee => employee.EmploymentStartDate),
            EmployeeSortField.EmploymentStartDate =>
                employees.OrderByDescending(employee => employee.EmploymentStartDate),
            EmployeeSortField.EmploymentEndDate when request.SortDirection == SortDirection.Ascending =>
                employees.OrderBy(employee => employee.EmploymentEndDate ?? DateOnly.MaxValue),
            EmployeeSortField.EmploymentEndDate =>
                employees.OrderByDescending(employee => employee.EmploymentEndDate ?? DateOnly.MinValue),
            EmployeeSortField.JobTitle when request.SortDirection == SortDirection.Ascending =>
                employees.OrderBy(employee => employee.JobTitle, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(employee => employee.LastName, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(employee => employee.FirstName, StringComparer.OrdinalIgnoreCase),
            EmployeeSortField.JobTitle =>
                employees.OrderByDescending(employee => employee.JobTitle, StringComparer.OrdinalIgnoreCase)
                    .ThenByDescending(employee => employee.LastName, StringComparer.OrdinalIgnoreCase)
                    .ThenByDescending(employee => employee.FirstName, StringComparer.OrdinalIgnoreCase),
            EmployeeSortField.Tenure when request.SortDirection == SortDirection.Ascending =>
                employees.OrderBy(employee => CalculateTenureInDays(employee, referenceDate)),
            EmployeeSortField.Tenure =>
                employees.OrderByDescending(employee => CalculateTenureInDays(employee, referenceDate)),
            _ when request.SortDirection == SortDirection.Ascending =>
                employees.OrderBy(employee => employee.LastName, StringComparer.OrdinalIgnoreCase)
                    .ThenBy(employee => employee.FirstName, StringComparer.OrdinalIgnoreCase),
            _ =>
                employees.OrderByDescending(employee => employee.LastName, StringComparer.OrdinalIgnoreCase)
                    .ThenByDescending(employee => employee.FirstName, StringComparer.OrdinalIgnoreCase)
        };
    }

    private static bool IsActive(Employee employee, DateOnly referenceDate)
    {
        return !employee.EmploymentEndDate.HasValue || employee.EmploymentEndDate.Value >= referenceDate;
    }

    private static double CalculateTenureInYears(Employee employee, DateOnly referenceDate)
    {
        return Math.Round(CalculateTenureInDays(employee, referenceDate) / 365.25d, 4);
    }

    private static double CalculateTenureInDays(Employee employee, DateOnly referenceDate)
    {
        var comparisonDate = employee.EmploymentEndDate.HasValue && employee.EmploymentEndDate.Value < referenceDate
            ? employee.EmploymentEndDate.Value
            : referenceDate;

        var tenure = (comparisonDate.ToDateTime(TimeOnly.MinValue) - employee.EmploymentStartDate.ToDateTime(TimeOnly.MinValue))
            .TotalDays;

        return tenure < 0 ? 0 : tenure;
    }
}
