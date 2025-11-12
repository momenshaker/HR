**Controller**
- Name: `CandidatesController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/Candidates`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `RecruitmentAndAts`
- Audit: `AuditResource("Candidate")`

**Endpoints**
- GET `/api/v1/Candidates` – List candidates
- GET `/api/v1/Candidates/{id}` – Get candidate by id
- POST `/api/v1/Candidates` – Create candidate
- PUT `/api/v1/Candidates/{id}` – Update candidate
- POST `/api/v1/Candidates/{id}/advance` – Advance candidate in funnel
- DELETE `/api/v1/Candidates/{id}` – Delete candidate

**Object Schemas**
- Requests: `CreateCandidateRequest`, `UpdateCandidateRequest`
- Responses: `CandidateDto`, `IReadOnlyCollection<CandidateDto>`

**cURL Examples**
- List candidates
  - curl -X GET "$BASE_URL/api/v1/Candidates" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/Candidates/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/Candidates" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "fullName": "Jane Doe",
  "email": "jane@example.com",
  "appliedRole": "Backend Engineer",
  "stage": "Applied",
  "source": "Referral",
  "resumeUrl": "https://example.com/resume.pdf",
  "notes": "Strong portfolio",
  "nextInterviewAtUtc": null
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/Candidates/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "fullName": "Jane Doe",
  "email": "jane@example.com",
  "appliedRole": "Backend Engineer",
  "stage": "Interview",
  "source": "Referral",
  "resumeUrl": "https://example.com/resume.pdf",
  "notes": "Phone screen complete",
  "nextInterviewAtUtc": "2025-01-07T09:00:00Z"
}
JSON
- Advance stage
  - curl -X POST "$BASE_URL/api/v1/Candidates/<id>/advance" -H "Authorization: Bearer <token>"
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/Candidates/<id>" -H "Authorization: Bearer <token>"
