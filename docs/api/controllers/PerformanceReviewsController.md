**Controller**
- Name: `PerformanceReviewsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/PerformanceReviews`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR,Manager")]`
- Feature: `PerformanceManagement`
- Audit: `AuditResource("PerformanceReview")`
- Subscription: `[RequiresSubscriptionEntitlement(HrFeature.PerformanceManagement)]`

**Endpoints**
- GET `/api/v1/PerformanceReviews` – List reviews
  - Produces: `IReadOnlyCollection<PerformanceReviewDto>` (200)
- GET `/api/v1/PerformanceReviews/{id}` – Get review by id
  - Produces: `PerformanceReviewDto` (200), 404
- POST `/api/v1/PerformanceReviews` – Create review
  - Produces: `PerformanceReviewDto` (201), 400
- PUT `/api/v1/PerformanceReviews/{id}` – Update review
  - Produces: `PerformanceReviewDto` (200), 400/404
- DELETE `/api/v1/PerformanceReviews/{id}` – Delete review
  - Produces: 204, 404

**Object Schemas**
- Requests: `CreatePerformanceReviewRequest`, `UpdatePerformanceReviewRequest`
- Responses: `PerformanceReviewDto`

**cURL Examples**
- List reviews
  - curl -X GET "$BASE_URL/api/v1/PerformanceReviews" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/PerformanceReviews/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/PerformanceReviews" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"employeeId\":\"<uuid>\",\"cycleId\":\"<uuid>\"}"
- Update
  - curl -X PUT "$BASE_URL/api/v1/PerformanceReviews/<id>" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"overallRating\":5}"
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/PerformanceReviews/<id>" -H "Authorization: Bearer <token>"
