**Controller**
- Name: `RecognitionProgramsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/RecognitionPrograms`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `InternalCommunication`
- Audit: `AuditResource("RecognitionProgram")`

**Endpoints**
- GET `/api/v1/RecognitionPrograms` – List recognition programs
- GET `/api/v1/RecognitionPrograms/{id}` – Get recognition program by id
- POST `/api/v1/RecognitionPrograms` – Create recognition program
- PUT `/api/v1/RecognitionPrograms/{id}` – Update recognition program
- DELETE `/api/v1/RecognitionPrograms/{id}` – Delete recognition program

**Object Schemas**
- Responses: `RecognitionProgramDto`, `IReadOnlyCollection<RecognitionProgramDto>`

**cURL Examples**
- List programs
  - curl -X GET "$BASE_URL/api/v1/RecognitionPrograms" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/RecognitionPrograms/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/RecognitionPrograms" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "name": "Kudos",
  "description": "Peer recognition",
  "criteria": "Impactful contribution",
  "reward": "Gift card",
  "isPeerToPeer": true,
  "isActive": true,
  "ownerId": "<uuid>"
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/RecognitionPrograms/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "name": "Kudos v2",
  "description": "Peer recognition program",
  "criteria": "Outstanding teamwork",
  "reward": "Bonus",
  "isPeerToPeer": true,
  "isActive": true,
  "ownerId": "<uuid>"
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/RecognitionPrograms/<id>" -H "Authorization: Bearer <token>"
