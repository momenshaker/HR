**Controller**
- Name: `VacanciesController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/Vacancies`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `RecruitmentAndAts`
- Audit: `AuditResource("Vacancy")`

**Endpoints**
- GET `/api/v1/Vacancies` – List vacancies
- GET `/api/v1/Vacancies/{id}` – Get vacancy by id
- POST `/api/v1/Vacancies` – Create vacancy
- PUT `/api/v1/Vacancies/{id}` – Update vacancy
- POST `/api/v1/Vacancies/{id}/close` – Close vacancy

**Object Schemas**
- Responses: `VacancyDto`, `IReadOnlyCollection<VacancyDto>`

**cURL Examples**
- List vacancies
  - curl -X GET "$BASE_URL/api/v1/Vacancies" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/Vacancies/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/Vacancies" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"title\":\"Backend Engineer\"}"
- Update
  - curl -X PUT "$BASE_URL/api/v1/Vacancies/<id>" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"title\":\"Senior Backend Engineer\"}"
- Close
  - curl -X POST "$BASE_URL/api/v1/Vacancies/<id>/close" -H "Authorization: Bearer <token>"
