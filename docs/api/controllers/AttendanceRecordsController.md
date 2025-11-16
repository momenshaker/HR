**Controller**
- Name: `AttendanceRecordsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/AttendanceRecords`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `AttendanceAndTimeTracking`
- Audit: `AuditResource("AttendanceRecord")`

**Endpoints**
- GET `/api/v1/AttendanceRecords` - List attendance records
  - Headers: Authorization, Accept
- GET `/api/v1/AttendanceRecords/{id}` - Get attendance record by id
  - Headers: Authorization, Accept
  - Path: `id` (uuid, required)
- POST `/api/v1/AttendanceRecords` - Create attendance record
  - Headers: Authorization, Accept, Content-Type: application/json
  - Body: `CreateAttendanceRecordRequest`
- PUT `/api/v1/AttendanceRecords/{id}` - Update attendance record
  - Headers: Authorization, Accept, Content-Type: application/json
  - Path: `id` (uuid, required)
  - Body: `UpdateAttendanceRecordRequest`
- DELETE `/api/v1/AttendanceRecords/{id}` - Delete attendance record
  - Headers: Authorization, Accept
  - Path: `id` (uuid, required)

**Object Schemas**
- CreateAttendanceRecordRequest / UpdateAttendanceRecordRequest
  - employeeId (uuid, required)
  - workDate (date, required)
  - shiftName (string, ≤100)
  - scheduledStartTimeUtc / scheduledEndTimeUtc (string, date-time)
  - scheduledWorkMinutes, breakMinutes, gracePeriodMinutes (int)
  - checkInTimeUtc / checkOutTimeUtc (string, date-time)
  - totalWorkedMinutes, lateMinutes, earlyLeaveMinutes, overtimeMinutes, absenceMinutes (int, 0..1440)
  - status (string, ≤50)
  - source (string, ≤50)
  - remarks (string, ≤500)
  - punches[] (id?, type, timestampUtc, source, deviceId, location, notes)
- Responses: `AttendanceRecordDto`, `IReadOnlyCollection<AttendanceRecordDto>`

**cURL Examples**
- List records
  - curl -X GET "$BASE_URL/api/v1/AttendanceRecords" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/AttendanceRecords/<id>" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Create
  - curl -X POST "$BASE_URL/api/v1/AttendanceRecords" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "employeeId": "<uuid>",
  "workDate": "2025-01-01",
  "shiftName": "Day",
  "scheduledStartTimeUtc": "2025-01-01T09:00:00Z",
  "scheduledEndTimeUtc": "2025-01-01T18:00:00Z",
  "scheduledWorkMinutes": 480,
  "breakMinutes": 60,
  "gracePeriodMinutes": 10,
  "punches": [
    {
      "type": "ClockIn",
      "timestampUtc": "2025-01-01T09:05:00Z",
      "source": "SelfService",
      "deviceId": "web",
      "location": "HQ",
      "notes": ""
    }
  ],
  "status": "InProgress",
  "source": "SelfService",
  "remarks": "First punch"
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/AttendanceRecords/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "employeeId": "<uuid>",
  "workDate": "2025-01-01",
  "shiftName": "Day",
  "scheduledStartTimeUtc": "2025-01-01T09:00:00Z",
  "scheduledEndTimeUtc": "2025-01-01T18:00:00Z",
  "scheduledWorkMinutes": 480,
  "breakMinutes": 60,
  "gracePeriodMinutes": 10,
  "punches": [
    {
      "id": "<existing-punch-id>",
      "type": "ClockIn",
      "timestampUtc": "2025-01-01T09:05:00Z",
      "source": "SelfService",
      "deviceId": "web",
      "location": "HQ",
      "notes": ""
    },
    {
      "type": "ClockOut",
      "timestampUtc": "2025-01-01T18:15:00Z",
      "source": "SelfService",
      "deviceId": "web",
      "location": "HQ",
      "notes": "Overtime"
    }
  ],
  "status": "Completed",
  "source": "SelfService",
  "remarks": "Completed"
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/AttendanceRecords/<id>" -H "Authorization: Bearer <token>" -H "Accept: application/json"

