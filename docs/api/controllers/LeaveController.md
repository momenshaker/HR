**Controller**
- Name: `LeaveController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/leave` → `/api/v1/leave`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager,Employee")]`
- Feature: `LeaveManagement`
- Audit: `AuditResource("Leave")`

**Endpoints**
- GET `/api/v1/leave/types` – List leave types
- GET `/api/v1/leave/balances` – Get leave balances
- POST `/api/v1/leave/requests` – Create leave request
- GET `/api/v1/leave/requests` – List leave requests
- GET `/api/v1/leave/requests/{id}` – Get leave request by id
- POST `/api/v1/leave/requests/{id}:submit` – Submit leave request
- POST `/api/v1/leave/requests/{id}:approve` – Approve leave request
- POST `/api/v1/leave/requests/{id}:reject` – Reject leave request
- POST `/api/v1/leave/requests/{id}:cancel` – Cancel leave request

**Object Schemas**
- Responses: `IReadOnlyCollection<LeaveTypeDto>`, `IReadOnlyCollection<LeaveBalanceDto>`, `LeaveRequestDto`, `PagedLeaveRequestsDto`
- `LeaveTypeDto` → `{ id, code, name, isPaid, requiresApproval, requiresAttachment, annualAllowanceDays, carryOverDays, maxConsecutiveDays, color }`
- `LeaveBalanceDto` → `{ employeeId, leaveTypeId, year, openingBalance, accrued, taken, carriedForward, reserved, remaining }`
- `LeaveRequestDto` → `{ id, employeeId, leaveTypeId, leaveType, startDate, endDate, numberOfDays, status (Draft|PendingApproval|Approved|Rejected|Cancelled), approverId, reason, attachmentPath, submittedAtUtc, approvedAtUtc, rejectedAtUtc, cancelledAtUtc }`

**cURL Examples**
- List leave types
  - curl -X GET "$BASE_URL/api/v1/leave/types" -H "Authorization: Bearer <token>"
- Get balances
  - curl -X GET "$BASE_URL/api/v1/leave/balances" -H "Authorization: Bearer <token>"
- Create request
  - curl -X POST "$BASE_URL/api/v1/leave/requests" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"employeeId\":\"<uuid>\",\"leaveTypeId\":\"<uuid>\",\"from\":\"2025-01-10\",\"to\":\"2025-01-12\"}"
- List requests
  - curl -X GET "$BASE_URL/api/v1/leave/requests" -H "Authorization: Bearer <token>"
- Get request by id
  - curl -X GET "$BASE_URL/api/v1/leave/requests/<id>" -H "Authorization: Bearer <token>"
- Submit
  - curl -X POST "$BASE_URL/api/v1/leave/requests/<id>:submit" -H "Authorization: Bearer <token>"
- Approve
  - curl -X POST "$BASE_URL/api/v1/leave/requests/<id>:approve" -H "Authorization: Bearer <token>"
- Reject
  - curl -X POST "$BASE_URL/api/v1/leave/requests/<id>:reject" -H "Authorization: Bearer <token>"
- Cancel
  - curl -X POST "$BASE_URL/api/v1/leave/requests/<id>:cancel" -H "Authorization: Bearer <token>"
