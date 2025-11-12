**Controller**
- Name: `SystemController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}`
- Version: `[ApiVersion("1.0")]`
- Auth: `[AllowAnonymous]` (public)
- Feature: `PlatformServices`

**Endpoints**
- GET `/api/v1/health` – Report API health status
  - Produces: `SystemHealthResponse` (200)
- GET `/api/v1/version` – Retrieve API version information
  - Produces: `SystemVersionResponse` (200)

**Object Schemas**
- `SystemHealthResponse` – inline record (Status, Environment, Timestamp)
- `SystemVersionResponse` – inline record (Version, Environment)

**cURL Examples**
- Health (public)
  - curl -X GET "$BASE_URL/api/v1/health" -H "Accept: application/json"
- Version (public)
  - curl -X GET "$BASE_URL/api/v1/version" -H "Accept: application/json"
