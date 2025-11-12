**Controller**
- Name: `DepartmentsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/organizations/{organizationId}/departments`
- Version: unversioned route
- Auth: not specified at controller level

**Endpoints**
- GET `/api/organizations/{organizationId}/departments` – List departments for organization
- POST `/api/organizations/{organizationId}/departments` – Create department
- GET `/api/organizations/{organizationId}/departments/{departmentId}` – Get department by id
- PUT `/api/organizations/{organizationId}/departments/{departmentId}` – Update department
- POST `/api/organizations/{organizationId}/departments/{departmentId}:move` – Move department
- DELETE `/api/organizations/{organizationId}/departments/{departmentId}` – Delete department

**Object Schemas**
- Requests: `CreateDepartmentRequest`, `UpdateDepartmentRequest`, `MoveDepartmentRequest`
- Responses: `DepartmentDto`, `IReadOnlyCollection<DepartmentDto>`, `ErrorResponse`

**cURL Examples**
- List departments
  - curl -X GET "$BASE_URL/api/organizations/<organizationId>/departments" -H "Authorization: Bearer <token>"
- Create department
  - curl -X POST "$BASE_URL/api/organizations/<organizationId>/departments" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "name": "Engineering",
  "code": "ENG",
  "organizationId": "<uuid>",
  "parentDepartmentId": null,
  "managerId": null,
  "branch": "",
  "location": "",
  "description": "",
  "isActive": true
}
JSON
- Get by id
  - curl -X GET "$BASE_URL/api/organizations/<organizationId>/departments/<departmentId>" -H "Authorization: Bearer <token>"
- Update
  - curl -X PUT "$BASE_URL/api/organizations/<organizationId>/departments/<departmentId>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "name": "R&D",
  "code": "ENG",
  "organizationId": "<uuid>",
  "parentDepartmentId": null,
  "managerId": null,
  "branch": "",
  "location": "",
  "description": "",
  "isActive": true
}
JSON
- Move
  - curl -X POST "$BASE_URL/api/organizations/<organizationId>/departments/<departmentId>:move" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d '{"newParentDepartmentId":"<uuid>"}'
- Delete
  - curl -X DELETE "$BASE_URL/api/organizations/<organizationId>/departments/<departmentId>" -H "Authorization: Bearer <token>"
