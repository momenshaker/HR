**Controller**
- Name: `WorkSchedulesController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `/api/v1/WorkSchedules`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `AttendanceAndTimeTracking`
- Audit: `AuditResource("WorkSchedule")`

**Endpoints**
- GET `/api/v1/WorkSchedules` – List work schedules
- GET `/api/v1/WorkSchedules/{id}` – Get work schedule by id
- POST `/api/v1/WorkSchedules` – Create work schedule (body: `CreateWorkScheduleRequest`)
- PUT `/api/v1/WorkSchedules/{id}` – Update work schedule (body: `UpdateWorkScheduleRequest`)
- DELETE `/api/v1/WorkSchedules/{id}` – Delete work schedule

**Object Schemas**
- CreateWorkScheduleRequest / UpdateWorkScheduleRequest
  - name (string, ≤200, required)
  - organizationId / departmentId (uuid)
  - isDefaultForOrganization (bool)
  - timeZoneId (string, ≤100)
  - shiftTemplates[] (dayOfWeek, startTime, endTime, breakMinutes, gracePeriodMinutes, minimumOvertimeMinutes)
- Response: `WorkScheduleDto`

**Usage Notes**
- All shifts are expressed in the organization's local time zone. Break, grace, and overtime thresholds are stored in minutes.
- Deleting a work schedule cascades to its shift templates.
