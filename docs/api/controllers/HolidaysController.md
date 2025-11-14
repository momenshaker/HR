**Controller**
- Name: `HolidaysController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `/api/v1/Holidays`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `AttendanceAndTimeTracking`
- Audit: `AuditResource("Holiday")`

**Endpoints**
- GET `/api/v1/Holidays` – List holiday definitions
- GET `/api/v1/Holidays/{id}` – Get holiday by id
- POST `/api/v1/Holidays` – Create holiday (body: `CreateHolidayRequest`)
- PUT `/api/v1/Holidays/{id}` – Update holiday (body: `UpdateHolidayRequest`)
- DELETE `/api/v1/Holidays/{id}` – Delete holiday

**Object Schemas**
- CreateHolidayRequest / UpdateHolidayRequest
  - organizationId (uuid, required)
  - date (date, required)
  - name (string, ≤200, required)
  - isPaid (bool)
  - countryCode (string, ≤10)
  - description (string, ≤500)
- Response: `HolidayDto`

**Usage Notes**
- The API enforces uniqueness per organization/date to avoid duplicate holiday entries.
- Attendance generation can treat holidays as auto-approved time off when combined with schedule rules.
