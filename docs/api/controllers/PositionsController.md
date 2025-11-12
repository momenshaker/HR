**Controller**
- Name: `PositionsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/Positions`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `OrganizationStructure`
- Audit: `AuditResource("Position")`

**Endpoints**
- GET `/api/v1/Positions` – List positions
- GET `/api/v1/Positions/organization-unit/{organizationUnitId}` – Positions by organization unit
- GET `/api/v1/Positions/employee/{employeeId}` – Positions by employee
- GET `/api/v1/Positions/{id}` – Get position by id
- POST `/api/v1/Positions` – Create position
- PUT `/api/v1/Positions/{id}` – Update position
- DELETE `/api/v1/Positions/{id}` – Delete position

**Object Schemas**
- Responses: `PositionDto`, `IReadOnlyCollection<PositionDto>`

**cURL Examples**
- List positions
  - curl -X GET "$BASE_URL/api/v1/Positions" -H "Authorization: Bearer <token>"
- By org unit
  - curl -X GET "$BASE_URL/api/v1/Positions/organization-unit/<organizationUnitId>" -H "Authorization: Bearer <token>"
- By employee
  - curl -X GET "$BASE_URL/api/v1/Positions/employee/<employeeId>" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/Positions/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/Positions" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "title": "Engineer",
  "jobCode": "ENG1",
  "organizationUnitId": "<uuid>",
  "reportsToPositionId": null,
  "occupiedByEmployeeId": null,
  "grade": "G7",
  "employmentType": "FullTime",
  "effectiveFrom": "2025-01-01",
  "effectiveTo": null,
  "isCriticalRole": false,
  "isVacant": true
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/Positions/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "title": "Senior Engineer",
  "jobCode": "ENG2",
  "organizationUnitId": "<uuid>",
  "reportsToPositionId": null,
  "occupiedByEmployeeId": null,
  "grade": "G8",
  "employmentType": "FullTime",
  "effectiveFrom": "2025-01-01",
  "effectiveTo": null,
  "isCriticalRole": false,
  "isVacant": false
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/Positions/<id>" -H "Authorization: Bearer <token>"
