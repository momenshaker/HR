**Controller**
- Name: `OrganizationUnitsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/OrganizationUnits`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `OrganizationStructure`
- Audit: `AuditResource("OrganizationUnit")`

**Endpoints**
- GET `/api/v1/OrganizationUnits` – List organization units
- GET `/api/v1/OrganizationUnits/hierarchy` – Get organization hierarchy
- GET `/api/v1/OrganizationUnits/{id}` – Get organization unit by id
- POST `/api/v1/OrganizationUnits` – Create organization unit
- PUT `/api/v1/OrganizationUnits/{id}` – Update organization unit
- DELETE `/api/v1/OrganizationUnits/{id}` – Delete organization unit

**Object Schemas**
- Responses: `OrganizationUnitDto`, `IReadOnlyCollection<OrganizationUnitDto>`, `IReadOnlyCollection<OrganizationHierarchyNodeDto>`

**cURL Examples**
- List org units
  - curl -X GET "$BASE_URL/api/v1/OrganizationUnits" -H "Authorization: Bearer <token>"
- Get hierarchy
  - curl -X GET "$BASE_URL/api/v1/OrganizationUnits/hierarchy" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/OrganizationUnits/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/OrganizationUnits" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "name": "Finance",
  "code": "FIN",
  "type": "Department",
  "parentUnitId": null,
  "departmentId": null,
  "leadPositionId": null,
  "level": 2,
  "description": "",
  "isActive": true
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/OrganizationUnits/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "name": "Finance Ops",
  "code": "FIN",
  "type": "Department",
  "parentUnitId": null,
  "departmentId": null,
  "leadPositionId": null,
  "level": 2,
  "description": "",
  "isActive": true
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/OrganizationUnits/<id>" -H "Authorization: Bearer <token>"
