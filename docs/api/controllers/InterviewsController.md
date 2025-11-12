**Controller**
- Name: `InterviewsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/Interviews`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `RecruitmentAndAts`
- Audit: `AuditResource("Interview")`

**Endpoints**
- GET `/api/v1/Interviews` – List interviews
- POST `/api/v1/Interviews` – Schedule an interview
- PUT `/api/v1/Interviews/{id}` – Update interview
- DELETE `/api/v1/Interviews/{id}` – Cancel interview

**Object Schemas**
- Responses: `InterviewScheduleDto`, `IReadOnlyCollection<InterviewScheduleDto>`

**cURL Examples**
- List interviews
  - curl -X GET "$BASE_URL/api/v1/Interviews" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/Interviews" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "candidateId": "<uuid>",
  "vacancyId": "<uuid>",
  "stage": "PhoneScreen",
  "scheduledAtUtc": "2025-01-05T14:00:00Z",
  "durationMinutes": 45,
  "mode": "Remote",
  "location": "",
  "meetingLink": "https://meet.example.com/abc",
  "interviewers": ["manager@example.com"],
  "notes": "First screen"
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/Interviews/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "candidateId": "<uuid>",
  "vacancyId": "<uuid>",
  "stage": "Onsite",
  "scheduledAtUtc": "2025-01-06T10:00:00Z",
  "durationMinutes": 90,
  "mode": "InPerson",
  "location": "Room 1",
  "meetingLink": "",
  "interviewers": ["lead@example.com"],
  "notes": "Panel"
}
JSON
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/Interviews/<id>" -H "Authorization: Bearer <token>"
