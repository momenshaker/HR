using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
using HR.Application.Abstractions.Services;
using HR.Application.DTOs;

namespace HR.Application.Services;

/// <inheritdoc />
public sealed class OnboardingService : IOnboardingService
{
    private readonly ICustomerService _customerService;
    private readonly IOrganizationService _organizationService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPlanService _planService;
    private readonly IAuthenticationService _authenticationService;
    private readonly IDepartmentService _departmentService;
    private readonly IEmployeeService _employeeService;

    public OnboardingService(
        ICustomerService customerService,
        IOrganizationService organizationService,
        ISubscriptionService subscriptionService,
        IPlanService planService,
        IAuthenticationService authenticationService,
        IDepartmentService departmentService,
        IEmployeeService employeeService)
    {
        _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
        _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
        _subscriptionService = subscriptionService ?? throw new ArgumentNullException(nameof(subscriptionService));
        _planService = planService ?? throw new ArgumentNullException(nameof(planService));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _departmentService = departmentService ?? throw new ArgumentNullException(nameof(departmentService));
        _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
    }

    /// <inheritdoc />
    public async Task<OnboardingResult> StartAsync(OnboardingRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var plan = (await _planService.GetPlansAsync(cancellationToken).ConfigureAwait(false))
            .FirstOrDefault(item => item.Id == request.Subscription.PlanId);

        if (plan is null)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure("subscription.planId", "Selected plan does not exist.", "PlanNotFound")
            });
        }

        var organizationRequest = new CreateOrganizationRequest
        {
            Name = request.Organization.Name,
            Code = request.Organization.Code,
            Description = request.Organization.Description,
            Industry = request.Organization.Industry,
            Region = request.Organization.Region,
            HeadquartersAddress = request.Organization.HeadquartersAddress,
            TimeZone = request.Organization.TimeZone,
            PrimaryContactEmail = request.Organization.PrimaryContactEmail,
            WebsiteUrl = request.Organization.WebsiteUrl,
            IsActive = true
        };

        var billingPhone = string.IsNullOrWhiteSpace(request.Organization.BillingPhone)
            ? request.Account.PhoneNumber
            : request.Organization.BillingPhone;

        var customerRequest = new CreateCustomerRequest
        {
            Name = request.Organization.Name,
            BillingEmail = request.Account.Email,
            BillingPhone = billingPhone,
            AddressLine1 = request.Organization.BillingAddressLine1,
            AddressLine2 = request.Organization.BillingAddressLine2,
            City = request.Organization.BillingCity,
            State = request.Organization.BillingState,
            PostalCode = request.Organization.BillingPostalCode,
            Country = request.Organization.BillingCountry,
            Status = "Active",
            TrialPeriodDays = request.Subscription.TrialPeriodDays
        };

        var customer = await _customerService.CreateAsync(customerRequest, cancellationToken).ConfigureAwait(false);
        var organization = await _organizationService.CreateAsync(organizationRequest, cancellationToken).ConfigureAwait(false);

        var subscriptionRequest = new CreateSubscriptionRequest
        {
            PlanId = plan.Id,
            Seats = request.Subscription.Seats,
            TrialPeriodDays = request.Subscription.TrialPeriodDays,
            CustomerId = customer.Id
        };

        var subscription = await _subscriptionService.CreateAsync(subscriptionRequest, cancellationToken).ConfigureAwait(false);
        await _subscriptionService.SetOrganizationsAsync(subscription.Id, new[] { organization.Id }, cancellationToken).ConfigureAwait(false);

        var departmentId = await CreateDefaultDepartmentAsync(
                organization.Id,
                request.Organization.Name,
                request.Organization.Code,
                request.Organization.Description,
                request.Organization.HeadquartersAddress,
                cancellationToken)
            .ConfigureAwait(false);

        var employee = await CreateAdminEmployeeAsync(
                request.Account.FullName,
                request.Account.Email,
                request.Account.PhoneNumber,
                departmentId,
                cancellationToken)
            .ConfigureAwait(false);

        var claims = new Dictionary<string, string>
        {
            [ClaimTypes.Name] = request.Account.FullName
        };

        var (result, adminUserId) = await _authenticationService.RegisterUserAsync(
            request.Account.Email,
            request.Account.Password,
            customer.Id.ToString(),
            new[] { "Manager" },
            claims,
            employee.Id,
            cancellationToken).ConfigureAwait(false);

        if (!result.Succeeded || adminUserId is null)
        {
            var failures = result.Errors.Select(error =>
                new ValidationFailure("account", error.Description ?? "Failed to register the account.", error.Code));

            throw new ValidationException("Failed to register the admin account.", failures);
        }

        return new OnboardingResult(customer.Id, organization.Id, subscription.Id, adminUserId.Value, employee.Id);
    }

    private async Task<Guid> CreateDefaultDepartmentAsync(
        Guid organizationId,
        string organizationName,
        string organizationCode,
        string organizationDescription,
        string headquartersAddress,
        CancellationToken cancellationToken)
    {
        var departmentRequest = new CreateDepartmentRequest
        {
            OrganizationId = organizationId,
            Name = string.IsNullOrWhiteSpace(organizationName) ? "Headquarters" : organizationName.Trim(),
            Code = BuildDepartmentCode(organizationCode),
            Description = organizationDescription,
            Branch = "Headquarters",
            Location = headquartersAddress,
            IsActive = true
        };

        var department = await _departmentService.CreateAsync(organizationId, departmentRequest, cancellationToken)
            .ConfigureAwait(false);
        return department.Id;
    }

    private async Task<EmployeeDto> CreateAdminEmployeeAsync(
        string fullName,
        string email,
        string? phoneNumber,
        Guid departmentId,
        CancellationToken cancellationToken)
    {
        var (firstName, lastName) = SplitFullName(fullName);
        var employeeRequest = new CreateEmployeeRequest
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            EmploymentStartDate = DateOnly.FromDateTime(DateTime.UtcNow),
            JobTitle = "Administrator",
            PhoneNumber = phoneNumber ?? string.Empty,
            EmploymentType = "FullTime",
            DepartmentAssignment = new EmployeeDepartmentAssignmentRequest
            {
                PrimaryDepartmentId = departmentId
            }
        };

        return await _employeeService.CreateAsync(employeeRequest, cancellationToken).ConfigureAwait(false);
    }

    private static string BuildDepartmentCode(string organizationCode)
    {
        var trimmed = string.IsNullOrWhiteSpace(organizationCode) ? "DEPT" : organizationCode.Trim();
        var normalized = string.Concat(trimmed.Where(char.IsLetterOrDigit));
        if (normalized.Length == 0)
        {
            normalized = "DEPT";
        }

        var baseSegment = normalized.Length > 15 ? normalized[..15] : normalized;
        var code = $"{baseSegment.ToUpperInvariant()}-HQ";
        return code.Length <= 20 ? code : code[..20];
    }

    private static (string FirstName, string LastName) SplitFullName(string fullName)
    {
        var trimmed = fullName?.Trim() ?? string.Empty;
        var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            const string fallback = "User";
            return (fallback, fallback);
        }

        if (parts.Length == 1)
        {
            return (parts[0], parts[0]);
        }

        var first = parts[0];
        var last = string.Join(" ", parts.Skip(1));
        return (first, last);
    }
}
