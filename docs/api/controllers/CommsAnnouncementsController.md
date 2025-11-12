**Controller**
- Name: `CommsAnnouncementsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/comms/announcements`
- Version: unversioned route
- Auth: `[Authorize(Roles = "Admin,HR,Manager,Employee")]`
- Feature: `InternalCommunication`
- Audit: `AuditResource("CommsAnnouncement")`

**Endpoints**
- GET `/api/comms/announcements` – List announcements (filters: `orgId`, `departmentId?`, `unreadForEmployeeId?`, `page`, `pageSize`)
  - Produces: `PaginatedResponse<CommsAnnouncementDto>` (200)
- POST `/api/comms/announcements` – Publish announcement
  - Produces: `CommsAnnouncementDto` (201), 400
- POST `/api/comms/announcements/{id}:pin` – Pin announcement
  - Produces: 204, 404
- POST `/api/comms/announcements/{id}:unpin` – Unpin announcement
  - Produces: 204, 404
- POST `/api/comms/announcements/{id}:read` – Mark announcement read (query: `employeeId`)
  - Produces: 204, 400

**Object Schemas**
- Requests: `CreateCommsAnnouncementRequest`
- Responses: `CommsAnnouncementDto`, `PaginatedResponse<CommsAnnouncementDto>`

**cURL Examples**
- List announcements
  - curl -X GET "$BASE_URL/api/comms/announcements?orgId=<uuid>&departmentId=<uuid>&page=1&pageSize=25" -H "Authorization: Bearer <token>"
- Publish announcement
  - curl -X POST "$BASE_URL/api/comms/announcements" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "organizationId": "<uuid>",
  "departmentId": null,
  "title": "Update",
  "body": "Hello team",
  "publishedById": "<uuid>"
}
JSON
- Pin
  - curl -X POST "$BASE_URL/api/comms/announcements/<id>:pin" -H "Authorization: Bearer <token>"
- Unpin
  - curl -X POST "$BASE_URL/api/comms/announcements/<id>:unpin" -H "Authorization: Bearer <token>"
- Mark read
  - curl -X POST "$BASE_URL/api/comms/announcements/<id>:read?employeeId=<uuid>" -H "Authorization: Bearer <token>"
