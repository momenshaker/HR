**Overview**
- Feature: Communication
- Auth: Bearer JWT for all endpoints.

**Endpoints**
- GET `/api/comms/announcements` — List announcements
  - Params:
    - `orgId` (string, uuid, required)
    - `departmentId` (string, uuid, optional)
    - `unreadForEmployeeId` (string, uuid, optional)
    - `page` (integer, int32, minimum 1, default 1)
    - `pageSize` (integer, int32, 1–200, default 25)
  - Responses:
    - 200 → object { data: array<`CommsAnnouncementDto`>, page (int32), pageSize (int32), totalRecords (int64) }
    - 401 → `ErrorResponse`
- POST `/api/comms/announcements` — Publish announcement
  - Request: `CreateCommsAnnouncementRequest`
  - Responses:
    - 201 → `CommsAnnouncementDto`
    - 400 → `ErrorResponse`
- POST `/api/comms/announcements/{id}:pin` — Pin announcement
  - Path: `id` (string, uuid)
  - Responses:
    - 204 → No Content
    - 404 → Not found
- POST `/api/comms/announcements/{id}:unpin` — Unpin announcement
  - Path: `id` (string, uuid)
  - Responses:
    - 204 → No Content
    - 404 → Not found
- POST `/api/comms/announcements/{id}:read` — Mark announcement read
  - Path: `id` (string, uuid)
  - Params: `employeeId` (string, uuid, required, query)
  - Responses:
    - 204 → No Content (read receipt recorded)
    - 400 → `ErrorResponse` (missing/invalid `employeeId`)

**DTOs**
- CommsAnnouncementDto — Announcement
  - id (string, uuid)
  - organizationId (string, uuid)
  - departmentId (string, uuid, nullable)
  - title (string)
  - body (string)
  - publishedAtUtc (string, date-time)
  - publishedById (string, uuid)
  - isPinned (boolean)
- CreateCommsAnnouncementRequest — Create announcement
  - organizationId (string, uuid)
  - departmentId (string, uuid, nullable)
  - title (string, max 200)
  - body (string, max 5000)
  - publishedById (string, uuid)
- ErrorResponse — Error payload: code, message, traceId

