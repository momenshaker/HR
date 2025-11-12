**Controller**
- Name: `TimesheetsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/time/timesheets` → `/api/v1/time/timesheets`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `AttendanceAndTimeTracking`
- Audit: `AuditResource("Timesheet")`

**Endpoints**
- GET `/api/v1/time/timesheets` – Get timesheet
- PUT `/api/v1/time/timesheets/{id}/entries` – Update entries
- POST `/api/v1/time/timesheets/{id}:submit` – Submit timesheet
- POST `/api/v1/time/timesheets/{id}:approve` – Approve timesheet
- POST `/api/v1/time/timesheets/{id}:reject` – Reject timesheet

**Object Schemas**
- Responses: `TimesheetDto`, `TimesheetEntryDto`

**cURL Examples**
- Get timesheets
  - curl -X GET "$BASE_URL/api/v1/time/timesheets" -H "Authorization: Bearer <token>"
- Update entries
  - curl -X PUT "$BASE_URL/api/v1/time/timesheets/<id>/entries" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "[{\"date\":\"2025-01-02\",\"hours\":8}]"
- Submit
  - curl -X POST "$BASE_URL/api/v1/time/timesheets/<id>:submit" -H "Authorization: Bearer <token>"
- Approve
  - curl -X POST "$BASE_URL/api/v1/time/timesheets/<id>:approve" -H "Authorization: Bearer <token>"
- Reject
  - curl -X POST "$BASE_URL/api/v1/time/timesheets/<id>:reject" -H "Authorization: Bearer <token>"
