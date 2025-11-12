**Controller**
- Name: `PulseSurveysController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/PulseSurveys`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `InternalCommunication`
- Audit: `AuditResource("PulseSurvey")`

**Endpoints**
- GET `/api/v1/PulseSurveys` – List pulse surveys
- GET `/api/v1/PulseSurveys/{id}` – Get pulse survey by id
- POST `/api/v1/PulseSurveys` – Create pulse survey
- PUT `/api/v1/PulseSurveys/{id}` – Update pulse survey
- DELETE `/api/v1/PulseSurveys/{id}` – Delete pulse survey

**Object Schemas**
- Responses: `PulseSurveyDto`, `IReadOnlyCollection<PulseSurveyDto>`

**cURL Examples**
- List surveys
  - curl -X GET "$BASE_URL/api/v1/PulseSurveys" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/PulseSurveys/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/PulseSurveys" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "title": "Quarterly Pulse",
  "description": "Q1 pulse survey",
  "audience": "All",
  "questionSet": "default",
  "responseWindowMinutes": 1440,
  "launchDateUtc": "2025-01-10T10:00:00Z",
  "closeDateUtc": null,
  "ownerId": "<uuid>"
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/PulseSurveys/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "title": "Quarterly Pulse v2",
  "description": "Q1 pulse survey updated",
  "audience": "All",
  "questionSet": "default",
  "responseWindowMinutes": 1440,
  "launchDateUtc": "2025-01-10T10:00:00Z",
  "closeDateUtc": null,
  "ownerId": "<uuid>"
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/PulseSurveys/<id>" -H "Authorization: Bearer <token>"
