**Controller**
- Name: `PlatformConfigurationController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/platform/configuration` → `/api/v1/platform/configuration`
- Version: `[ApiVersion("1.0")]`
- Auth: `[Authorize(Roles = "Admin")]`
- Audit: `AuditResource("PlatformConfiguration")`

**Endpoints**
- GET `/api/v1/platform/configuration` – Get platform configuration

**Object Schemas**
- Responses: `PlatformConfigurationDto`

**cURL Examples**
- Get platform configuration
  - curl -X GET "$BASE_URL/api/v1/platform/configuration" -H "Authorization: Bearer <token>"
