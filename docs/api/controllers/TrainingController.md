**Controller**
- Name: `TrainingController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/training`
- Version: unversioned route
- Auth: `[Authorize]` (Bearer JWT)

**Endpoints**
- GET `/api/training/courses` - List courses
  - Headers: Authorization, Accept
  - Query: `orgId` (uuid, required)
  - Produces: `IReadOnlyCollection<LiteCourseDto>` (200)
- POST `/api/training/courses` - Create course
  - Headers: Authorization, Accept, Content-Type: application/json
  - Body: `CreateLiteCourseRequest`
  - Produces: `LiteCourseDto` (201)
- GET `/api/training/courses/{id}/sessions` - List sessions for a course
  - Headers: Authorization, Accept
  - Path: `id` (uuid, required)
  - Produces: `IReadOnlyCollection<LiteCourseSessionDto>` (200)
- POST `/api/training/sessions` - Create course session
  - Headers: Authorization, Accept, Content-Type: application/json
  - Body: `CreateLiteCourseSessionRequest`
  - Produces: `LiteCourseSessionDto` (201)
- POST `/api/training/sessions/{sessionId}/enroll` - Enroll employee
  - Headers: Authorization, Accept
  - Path: `sessionId` (uuid, required)
  - Query: `employeeId` (uuid, required)
  - Produces: `LiteEnrollmentDto` (200)
- POST `/api/training/sessions/{sessionId}/complete` - Complete enrollment
  - Headers: Authorization, Accept
  - Path: `sessionId` (uuid, required)
  - Query: `employeeId` (uuid, required)
  - Produces: `LiteEnrollmentDto` (200)
- POST `/api/training/sessions/{sessionId}/cancel` - Cancel enrollment
  - Headers: Authorization, Accept
  - Path: `sessionId` (uuid, required)
  - Query: `employeeId` (uuid, required)
  - Produces: `LiteEnrollmentDto` (200)

**Object Schemas**
- CreateLiteCourseRequest
  - organizationId (uuid, required)
  - code (string, ≤50, required)
  - title (string, ≤200, required)
  - description (string, ≤2000)
  - durationHours (number)
  - isMandatory (boolean)
- CreateLiteCourseSessionRequest
  - courseId (uuid, required)
  - startUtc (string, date-time, required)
  - endUtc (string, date-time, required)
  - location (string, ≤500)
  - meetingUrl (string, ≤1000)
  - capacity (int, 1..100000)
- LiteCourseDto, LiteCourseSessionDto, LiteEnrollmentDto

**cURL Examples**
- List courses
  - curl -X GET "$BASE_URL/api/training/courses?orgId=<uuid>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json"
- Create course
  - curl -X POST "$BASE_URL/api/training/courses" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "organizationId": "<uuid>",
  "code": "SAFE101",
  "title": "Safety",
  "description": "Intro to workplace safety",
  "durationHours": 2.5,
  "isMandatory": true
}
JSON
- List sessions for course
  - curl -X GET "$BASE_URL/api/training/courses/<id>/sessions" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json"
- Create session
  - curl -X POST "$BASE_URL/api/training/sessions" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "courseId": "<uuid>",
  "startUtc": "2025-01-05T09:00:00Z",
  "endUtc": "2025-01-05T11:00:00Z",
  "location": "HQ Training Room",
  "meetingUrl": null,
  "capacity": 20
}
JSON
- Enroll
  - curl -X POST "$BASE_URL/api/training/sessions/<sessionId>/enroll?employeeId=<uuid>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json"
- Complete
  - curl -X POST "$BASE_URL/api/training/sessions/<sessionId>/complete?employeeId=<uuid>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json"
- Cancel
  - curl -X POST "$BASE_URL/api/training/sessions/<sessionId>/cancel?employeeId=<uuid>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json"

