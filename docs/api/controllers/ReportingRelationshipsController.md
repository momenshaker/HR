**Controller**
- Name: `ReportingRelationshipsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/ReportingRelationships`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `OrganizationStructure`
- Audit: `AuditResource("ReportingRelationship")`

**Endpoints**
- GET `/api/v1/ReportingRelationships` – List reporting relationships
- GET `/api/v1/ReportingRelationships/manager/{managerPositionId}` – By manager position id
- GET `/api/v1/ReportingRelationships/report/{reportPositionId}` – By report position id
- GET `/api/v1/ReportingRelationships/{id}` – Get by id
- POST `/api/v1/ReportingRelationships` – Create reporting relationship
- PUT `/api/v1/ReportingRelationships/{id}` – Update reporting relationship
- DELETE `/api/v1/ReportingRelationships/{id}` – Delete reporting relationship

**Object Schemas**
- Responses: `ReportingRelationshipDto`, `IReadOnlyCollection<ReportingRelationshipDto>`

**cURL Examples**
- List relationships
  - curl -X GET "$BASE_URL/api/v1/ReportingRelationships" -H "Authorization: Bearer <token>"
- By manager
  - curl -X GET "$BASE_URL/api/v1/ReportingRelationships/manager/<managerPositionId>" -H "Authorization: Bearer <token>"
- By report
  - curl -X GET "$BASE_URL/api/v1/ReportingRelationships/report/<reportPositionId>" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/ReportingRelationships/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/ReportingRelationships" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"managerPositionId\":\"<uuid>\",\"reportPositionId\":\"<uuid>\"}"
- Update
  - curl -X PUT "$BASE_URL/api/v1/ReportingRelationships/<id>" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{ }"
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/ReportingRelationships/<id>" -H "Authorization: Bearer <token>"
