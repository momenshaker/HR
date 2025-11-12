**Controller**
- Name: `AnalyticsSnapshotsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/AnalyticsSnapshots`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `HrAnalytics`
- Audit: `AuditResource("AnalyticsSnapshot")`

**Endpoints**
- GET `/api/v1/AnalyticsSnapshots` – List analytics snapshots
  - Produces: `IReadOnlyCollection<AnalyticsSnapshotDto>` (200)
- GET `/api/v1/AnalyticsSnapshots/{id}` – Get analytics snapshot by id
  - Produces: `AnalyticsSnapshotDto` (200), 404
- POST `/api/v1/AnalyticsSnapshots` – Create analytics snapshot
  - Produces: `AnalyticsSnapshotDto` (201), 400
- PUT `/api/v1/AnalyticsSnapshots/{id}` – Update analytics snapshot
  - Produces: `AnalyticsSnapshotDto` (200), 400/404
- DELETE `/api/v1/AnalyticsSnapshots/{id}` – Delete analytics snapshot
  - Produces: 204, 404

**Object Schemas**
- Requests: `CreateAnalyticsSnapshotRequest`, `UpdateAnalyticsSnapshotRequest`
- Responses: `AnalyticsSnapshotDto`

**cURL Examples**
- List snapshots
  - curl -X GET "$BASE_URL/api/v1/AnalyticsSnapshots" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/AnalyticsSnapshots/<id>" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Create
  - curl -X POST "$BASE_URL/api/v1/AnalyticsSnapshots" -H "Authorization: Bearer <token>" -H "Accept: application/json" -H "Content-Type: application/json" -d "{ }"
- Update
  - curl -X PUT "$BASE_URL/api/v1/AnalyticsSnapshots/<id>" -H "Authorization: Bearer <token>" -H "Accept: application/json" -H "Content-Type: application/json" -d "{ }"
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/AnalyticsSnapshots/<id>" -H "Authorization: Bearer <token>" -H "Accept: application/json"
