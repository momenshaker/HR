**Controller**
- Name: `EngagementCampaignsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/[controller]` → `/api/v1/EngagementCampaigns`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `InternalCommunication`
- Audit: `AuditResource("EngagementCampaign")`

**Endpoints**
- GET `/api/v1/EngagementCampaigns` – List engagement campaigns
- GET `/api/v1/EngagementCampaigns/{id}` – Get campaign by id
- POST `/api/v1/EngagementCampaigns` – Create campaign
- PUT `/api/v1/EngagementCampaigns/{id}` – Update campaign
- DELETE `/api/v1/EngagementCampaigns/{id}` – Delete campaign

**Object Schemas**
- Responses: `EngagementCampaignDto`, `IReadOnlyCollection<EngagementCampaignDto>`

**cURL Examples**
- List campaigns
  - curl -X GET "$BASE_URL/api/v1/EngagementCampaigns" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/v1/EngagementCampaigns/<id>" -H "Authorization: Bearer <token>"
- Create
  - curl -X POST "$BASE_URL/api/v1/EngagementCampaigns" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "name": "Q1 Engagement",
  "description": "Quarterly engagement push",
  "channels": "Email,InApp",
  "targetAudience": "All",
  "launchDateUtc": "2025-01-15T09:00:00Z",
  "endDateUtc": null,
  "ownerId": "<uuid>",
  "isAutomated": false
}
JSON
- Update
  - curl -X PUT "$BASE_URL/api/v1/EngagementCampaigns/<id>" -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d "{\"name\":\"Q1 Update\"}"
- Delete
  - curl -X DELETE "$BASE_URL/api/v1/EngagementCampaigns/<id>" -H "Authorization: Bearer <token>"
