**Controller**
- Name: `RecruitmentInsightsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/RecruitmentInsights`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `RecruitmentAndAts`
- Audit: `AuditResource("RecruitmentInsight")`

**Endpoints**
- GET `/api/v1/RecruitmentInsights` – Get recruitment insights

**Object Schemas**
- Responses: `RecruitmentInsightsDto`

**cURL Examples**
- Get insights
  - curl -X GET "$BASE_URL/api/v1/RecruitmentInsights" -H "Authorization: Bearer <token>" -H "Accept: application/json"
