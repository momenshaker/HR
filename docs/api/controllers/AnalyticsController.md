**Controller**
- Name: `AnalyticsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/analytics`
- Version: unversioned route
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `HrAnalytics`
- Audit: `AuditResource("Analytics")`

**Endpoints**
- GET `/api/analytics/headcount` – Headcount per department
  - Produces: `IReadOnlyCollection<HeadcountItemDto>` (200)
- GET `/api/analytics/utilization` – Timesheet utilization by period
  - Produces: `IReadOnlyCollection<UtilizationPeriodDto>` (200)
- GET `/api/analytics/leave-usage` – Leave usage by type
  - Produces: `IReadOnlyCollection<LeaveUsageItemDto>` (200)
- GET `/api/analytics/payroll-totals` – Payroll totals per run and department
  - Produces: `PayrollTotalsResponseDto` (200)
- GET `/api/analytics/recruitment-funnel` – Recruitment funnel by stage
  - Produces: `IReadOnlyCollection<StageCountDto>` (200)
- GET `/api/analytics/training-compliance` – Mandatory training compliance
  - Produces: `TrainingComplianceDto` (200)

**Object Schemas**
- Responses: `HeadcountItemDto`, `UtilizationPeriodDto`, `LeaveUsageItemDto`, `PayrollTotalsResponseDto`, `StageCountDto`, `TrainingComplianceDto`

**cURL Examples**
- Headcount
  - curl -X GET "$BASE_URL/api/analytics/headcount?orgId=<uuid>&departmentId=<uuid>" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Utilization
  - curl -X GET "$BASE_URL/api/analytics/utilization?orgId=<uuid>&from=2025-01-01&to=2025-01-31" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Leave usage
  - curl -X GET "$BASE_URL/api/analytics/leave-usage?orgId=<uuid>&year=2025" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Payroll totals
  - curl -X GET "$BASE_URL/api/analytics/payroll-totals?orgId=<uuid>&from=2025-01-01&to=2025-01-31" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Recruitment funnel
  - curl -X GET "$BASE_URL/api/analytics/recruitment-funnel?jobId=<uuid>" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Training compliance
  - curl -X GET "$BASE_URL/api/analytics/training-compliance?orgId=<uuid>" -H "Authorization: Bearer <token>" -H "Accept: application/json"
