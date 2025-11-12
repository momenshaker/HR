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
  - clockInUtc (string, date-time)
  - clockOutUtc (string, date-time)
  - overtimeMinutes (int, 0..1440)
  - status (string, ≤50)
  - notes (string, ≤500)
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
  "clockInUtc": "2025-01-01T09:00:00Z",
  "clockOutUtc": null,
  "overtimeMinutes": 0,
  "status": "Open",
  "notes": ""
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
  "clockInUtc": "2025-01-01T09:00:00Z",
  "clockOutUtc": "2025-01-01T17:00:00Z",
  "overtimeMinutes": 0,
  "status": "Closed",
  "notes": "Completed"
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/AttendanceRecords/<id>" -H "Authorization: Bearer <token>" -H "Accept: application/json"

