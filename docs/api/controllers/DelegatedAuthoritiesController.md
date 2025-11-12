**Controller**
- Name: `DelegatedAuthoritiesController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/DelegatedAuthorities`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `DelegatedAuthority`
- Audit: `AuditResource("DelegatedAuthority")`

**Endpoints**
- GET `/api/v1/DelegatedAuthorities` – List delegated authorities
- GET `/api/v1/DelegatedAuthorities/grantor/{employeeId}` – By grantor employee id
- GET `/api/v1/DelegatedAuthorities/delegate/{employeeId}` – By delegate employee id
- GET `/api/v1/DelegatedAuthorities/{id}` – Get by id
- POST `/api/v1/DelegatedAuthorities` – Create delegated authority
- PUT `/api/v1/DelegatedAuthorities/{id}` – Update delegated authority
- DELETE `/api/v1/DelegatedAuthorities/{id}` – Delete delegated authority

**Object Schemas**
- Requests: `CreateDelegatedAuthorityRequest`, `UpdateDelegatedAuthorityRequest`
- Responses: `DelegatedAuthorityDto`, `IReadOnlyCollection<DelegatedAuthorityDto>`

**cURL Examples**
- List authorities
  - curl -X GET "$BASE_URL/api/v1/DelegatedAuthorities" -H "Authorization: Bearer <token>"
- By grantor
  - curl -X GET "$BASE_URL/api/v1/DelegatedAuthorities/grantor/<employeeId>" -H "Authorization: Bearer <token>"
- By delegate
  - curl -X GET "$BASE_URL/api/v1/DelegatedAuthorities/delegate/<employeeId>" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/DelegatedAuthorities/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/DelegatedAuthorities" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "grantorEmployeeId": "<uuid>",
  "delegateEmployeeId": "<uuid>",
  "grantorPositionId": null,
  "delegatePositionId": null,
  "authorityScope": "Approvals",
  "approvalLimit": 10000,
  "grantedOnUtc": "2025-01-01T00:00:00Z",
  "expiresOnUtc": null,
  "notes": "Year start delegation"
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/DelegatedAuthorities/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "grantorEmployeeId": "<uuid>",
  "delegateEmployeeId": "<uuid>",
  "grantorPositionId": null,
  "delegatePositionId": null,
  "authorityScope": "Approvals",
  "approvalLimit": 20000,
  "grantedOnUtc": "2025-01-01T00:00:00Z",
  "expiresOnUtc": null,
  "notes": "Updated limit"
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/DelegatedAuthorities/<id>" -H "Authorization: Bearer <token>"
