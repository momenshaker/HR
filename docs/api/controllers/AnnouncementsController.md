**Controller**
- Name: `AnnouncementsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/Announcements`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `InternalCommunication`
- Audit: `AuditResource("Announcement")`

**Endpoints**
- GET `/api/v1/Announcements` – List announcements
  - Produces: `IReadOnlyCollection<AnnouncementDto>` (200)
- GET `/api/v1/Announcements/{id}` – Get announcement by id
  - Produces: `AnnouncementDto` (200), 404
- POST `/api/v1/Announcements` – Create announcement
  - Produces: `AnnouncementDto` (201), 400
- PUT `/api/v1/Announcements/{id}` – Update announcement
  - Produces: `AnnouncementDto` (200), 400/404
- DELETE `/api/v1/Announcements/{id}` – Delete announcement
  - Produces: 204, 404

**Object Schemas**
- Requests: `CreateAnnouncementRequest`, `UpdateAnnouncementRequest`
- Responses: `AnnouncementDto`

**cURL Examples**
- List announcements
  - curl -X GET "$BASE_URL/api/v1/Announcements" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/Announcements/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/Announcements" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "title": "Notice",
  "message": "Details",
  "audience": "All",
  "createdBy": "<uuid>",
  "requiresAcknowledgement": false
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/Announcements/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "title": "Notice v2",
  "message": "Updated details",
  "audience": "All",
  "createdBy": "<uuid>",
  "requiresAcknowledgement": false,
  "publishedAtUtc": "2025-01-02T10:00:00Z"
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/Announcements/<id>" -H "Authorization: Bearer <token>"
