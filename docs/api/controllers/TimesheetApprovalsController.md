**Controller**
- Name: `TimesheetApprovalsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/time/approvals` → `/api/v1/time/approvals`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `AttendanceAndTimeTracking`
- Audit: `AuditResource("TimesheetApproval")`

**Endpoints**
- GET `/api/v1/time/approvals` – List timesheets pending approval (paginated)

**Object Schemas**
- Responses: `PaginatedResponse<TimesheetDto>`

**cURL Examples**
- List approvals
  - curl -X GET "$BASE_URL/api/v1/time/approvals" -H "Authorization: Bearer <token>" -H "Accept: application/json"
