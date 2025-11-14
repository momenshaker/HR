**Controller**
- Name: `EmployeeSchedulesController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `/api/v1/EmployeeSchedules`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `AttendanceAndTimeTracking`
- Audit: `AuditResource("EmployeeSchedule")`

**Endpoints**
- GET `/api/v1/EmployeeSchedules` – List employee schedule assignments
- GET `/api/v1/EmployeeSchedules/{id}` – Get assignment by id
- POST `/api/v1/EmployeeSchedules` – Assign work schedule (body: `CreateEmployeeScheduleRequest`)
- PUT `/api/v1/EmployeeSchedules/{id}` – Update assignment (body: `UpdateEmployeeScheduleRequest`)
- DELETE `/api/v1/EmployeeSchedules/{id}` – Remove assignment

**Object Schemas**
- CreateEmployeeScheduleRequest / UpdateEmployeeScheduleRequest
  - employeeId (uuid, required)
  - workScheduleId (uuid, required)
  - effectiveFrom (date, required)
  - effectiveTo (date)
- Response: `EmployeeScheduleDto`

**Usage Notes**
- The API does not enforce overlap checks; clients should ensure only one active schedule exists per employee/date range.
- `effectiveTo` can be omitted for open-ended assignments.
