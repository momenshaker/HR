**Controller**
- Name: `LeaveRequestsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/LeaveRequests`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `LeaveManagement`
- Audit: `AuditResource("LeaveRequest")`

**Endpoints**
- GET `/api/v1/LeaveRequests` – List leave requests
- GET `/api/v1/LeaveRequests/{id}` – Get leave request by id
- POST `/api/v1/LeaveRequests` – Create leave request
- PUT `/api/v1/LeaveRequests/{id}` – Update leave request
- DELETE `/api/v1/LeaveRequests/{id}` – Delete leave request

**Object Schemas**
- Responses: `LeaveRequestDto`, `IReadOnlyCollection<LeaveRequestDto>`

**cURL Examples**
- List leave requests
  - curl -X GET "$BASE_URL/api/v1/LeaveRequests" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/LeaveRequests/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/LeaveRequests" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "employeeId": "<uuid>",
  "leaveTypeId": "<uuid>",
  "startDate": "2025-02-01",
  "endDate": "2025-02-05",
  "reason": "Vacation",
  "attachmentPath": "https://files.example.com/medical-note.pdf"
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/LeaveRequests/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "leaveTypeId": "<uuid>",
  "startDate": "2025-02-01",
  "endDate": "2025-02-05",
  "reason": "Updated reason",
  "status": "PendingApproval",
  "approverId": "<uuid>",
  "approvedAtUtc": null,
  "rejectedAtUtc": null,
  "cancelledAtUtc": null,
  "attachmentPath": "https://files.example.com/updated-note.pdf"
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/LeaveRequests/<id>" -H "Authorization: Bearer <token>"
