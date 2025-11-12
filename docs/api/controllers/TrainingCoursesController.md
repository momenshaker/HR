**Controller**
- Name: `TrainingCoursesController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/TrainingCourses`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `TrainingAndDevelopment`
- Audit: `AuditResource("TrainingCourse")`

**Endpoints**
- GET `/api/v1/TrainingCourses` – List training courses
  - Produces: `IReadOnlyCollection<TrainingCourseDto>` (200)
- GET `/api/v1/TrainingCourses/competency/{competencyCode}` – List by competency
  - Produces: `IReadOnlyCollection<TrainingCourseDto>` (200), 400
- GET `/api/v1/TrainingCourses/{id}` – Get course by id
  - Produces: `TrainingCourseDto` (200), 404
- POST `/api/v1/TrainingCourses` – Create training course
  - Produces: `TrainingCourseDto` (201), 400
- PUT `/api/v1/TrainingCourses/{id}` – Update training course
  - Produces: `TrainingCourseDto` (200), 400/404
- DELETE `/api/v1/TrainingCourses/{id}` – Delete training course
  - Produces: 204, 404
- POST `/api/v1/TrainingCourses/{courseId}/enrollments` – Enroll employee
  - Produces: `CourseEnrollmentDto` (201), 400

**Object Schemas**
- Requests: `CreateTrainingCourseRequest`, `UpdateTrainingCourseRequest`, `CreateCourseEnrollmentRequest`
- Responses: `TrainingCourseDto`, `CourseEnrollmentDto`

**cURL Examples**
- List courses
  - curl -X GET "$BASE_URL/api/v1/TrainingCourses" -H "Authorization: Bearer <token>"
- By competency
  - curl -X GET "$BASE_URL/api/v1/TrainingCourses/competency/<competencyCode>" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/TrainingCourses/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/TrainingCourses" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"name\":\"Leadership 101\"}"
- Update
  - curl -X PUT "$BASE_URL/api/v1/TrainingCourses/<id>" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"name\":\"Leadership 102\"}"
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/TrainingCourses/<id>" -H "Authorization: Bearer <token>"
- Enroll
  - curl -X POST "$BASE_URL/api/v1/TrainingCourses/<courseId>/enrollments" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"employeeId\":\"<uuid>\"}"
