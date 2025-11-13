**Controller**
- Name: `OrganizationsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/organizations`
- Version: unversioned route
- Auth: not specified at controller level

**Endpoints**
- GET `/api/organizations` – List organizations
- GET `/api/organizations/{id}` – Get organization by id
- POST `/api/organizations` – Create organization
- PUT `/api/organizations/{id}` – Update organization
- DELETE `/api/organizations/{id}` – Delete organization

**Object Schemas**
- Responses: `OrganizationDto`, `IReadOnlyCollection<OrganizationDto>`, `ErrorResponse`

**cURL Examples**
- List organizations
  - curl -X GET "$BASE_URL/api/organizations" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/organizations/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/organizations" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "name": "Acme Corp",
  "code": "ACME",
  "description": "Enterprise HR platform",
  "industry": "Professional Services",
  "region": "North America",
  "headquartersAddress": "1 Summit Avenue, New York, NY 10004",
  "timeZone": "America/New_York",
  "primaryContactEmail": "hello@acme.test",
  "websiteUrl": "https://acme.test",
  "isActive": true
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/organizations/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "name": "Acme Inc",
  "code": "ACME",
  "description": "Updated global HR platform",
  "industry": "Technology",
  "region": "Global",
  "headquartersAddress": "1 Summit Avenue, New York, NY 10004",
  "timeZone": "America/New_York",
  "primaryContactEmail": "support@acme.inc",
  "websiteUrl": "https://acme.inc",
  "isActive": true
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/organizations/<id>" -H "Authorization: Bearer <token>"
