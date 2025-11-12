**Controller**
- Name: `EmployeeSelfServiceController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/employees/{employeeId}/self-service` → `/api/v1/employees/{employeeId}/self-service`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Employee")]`
- Feature: Multiple – EmployeeManagement, LeaveManagement, AttendanceAndTimeTracking, PayrollManagement, TrainingAndDevelopment, OrganizationStructure, DelegatedAuthority, SelfService
- Audit: `AuditResource("EmployeeSelfService")`

**Endpoints**
- GET `/api/v1/employees/{employeeId}/self-service/leave-requests` – List own leave requests
- POST `/api/v1/employees/{employeeId}/self-service/leave-requests` – Create leave request
- POST `/api/v1/employees/{employeeId}/self-service/attendance/clock-in` – Clock in
- POST `/api/v1/employees/{employeeId}/self-service/attendance/{attendanceRecordId}/clock-out` – Clock out
- GET `/api/v1/employees/{employeeId}/self-service/salary-slips` – List salary slips
- GET `/api/v1/employees/{employeeId}/self-service/training-courses` – List available training courses
- GET `/api/v1/employees/{employeeId}/self-service/organization` – Get organization snapshot
- GET `/api/v1/employees/{employeeId}/self-service/delegated-authorities` – List delegated authorities
- GET `/api/v1/employees/{employeeId}/self-service/account` – Get account
- POST `/api/v1/employees/{employeeId}/self-service/account` – Create account
- PUT `/api/v1/employees/{employeeId}/self-service/account` – Update account
- DELETE `/api/v1/employees/{employeeId}/self-service/account` – Delete account

**Object Schemas**
- Responses: `IReadOnlyCollection<LeaveRequestDto>`, `LeaveRequestDto`, `AttendanceRecordDto`, `IReadOnlyCollection<SalarySlipDto>`, `IReadOnlyCollection<TrainingCourseDto>`, `EmployeeOrganizationSnapshotDto`, `IReadOnlyCollection<DelegatedAuthorityDto>`, `SelfServiceAccountDto`

**cURL Examples**
- List leave requests
  - curl -X GET "$BASE_URL/api/v1/employees/<employeeId>/self-service/leave-requests" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Create leave request
  - curl -X POST "$BASE_URL/api/v1/employees/<employeeId>/self-service/leave-requests" -H "Authorization: Bearer <token>" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"leaveTypeId\":\"<uuid>\",\"from\":\"2025-01-10\",\"to\":\"2025-01-12\"}"
- Clock in
  - curl -X POST "$BASE_URL/api/v1/employees/<employeeId>/self-service/attendance/clock-in" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Clock out
  - curl -X POST "$BASE_URL/api/v1/employees/<employeeId>/self-service/attendance/<attendanceRecordId>/clock-out" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Salary slips
  - curl -X GET "$BASE_URL/api/v1/employees/<employeeId>/self-service/salary-slips" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Training courses
  - curl -X GET "$BASE_URL/api/v1/employees/<employeeId>/self-service/training-courses" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Organization snapshot
  - curl -X GET "$BASE_URL/api/v1/employees/<employeeId>/self-service/organization" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Delegated authorities
  - curl -X GET "$BASE_URL/api/v1/employees/<employeeId>/self-service/delegated-authorities" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Get account
  - curl -X GET "$BASE_URL/api/v1/employees/<employeeId>/self-service/account" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Create account
  - curl -X POST "$BASE_URL/api/v1/employees/<employeeId>/self-service/account" -H "Authorization: Bearer <token>" -H "Accept: application/json" -H "Content-Type: application/json" -d "{ }"
- Update account
  - curl -X PUT "$BASE_URL/api/v1/employees/<employeeId>/self-service/account" -H "Authorization: Bearer <token>" -H "Accept: application/json" -H "Content-Type: application/json" -d "{ }"
- Delete account
  - curl -X DELETE "$BASE_URL/api/v1/employees/<employeeId>/self-service/account" -H "Authorization: Bearer <token>" -H "Accept: application/json"
