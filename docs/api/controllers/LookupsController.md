**Controller**
- Name: `LookupsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v1/lookups`
- Version: 1.0 (API versioning via `v{version:apiVersion}`)
- Auth: `[Authorize]` (Admin/HR roles required for create/update/delete)
- Features: ETag caching (`ETag` header on GET, `If-None-Match` supported), audit logging for all mutations, and automatic category creation when new names are posted.

**Endpoints**
- GET `/api/v1/lookups` – Return every lookup category with values plus the `versionToken` ETag.
- GET `/api/v1/lookups/category/{category}` – Return the values for a specific category.
- GET `/api/v1/lookups/value/{id}` – Retrieve a lookup value by identifier.
- POST `/api/v1/lookups` – Create a lookup value; duplicates per `(category, code)` return HTTP 409.
- PUT `/api/v1/lookups/{id}` – Update a lookup value (category/code can change with uniqueness enforcement).
- DELETE `/api/v1/lookups/{id}` – Delete a lookup value and invalidate caches.

**Caching**
- GET requests emit a strong `ETag` header computed from the version token. Clients should send `If-None-Match` to avoid transferring the full payload when nothing changed. Mutations reset the token and clear downstream caches.

**cURL Examples**
- List lookups with cache validation
  - `curl -X GET "$BASE_URL/api/v1/lookups" -H "Authorization: Bearer <token>" -H "If-None-Match: \"<etag>\"" -H "Accept: application/json"`
- Create lookup value
  - ```bash
    curl -X POST "$BASE_URL/api/v1/lookups" \
      -H "Authorization: Bearer <admin-token>" \
      -H "Content-Type: application/json" \
      -d @- <<'JSON'
    {
      "category": "branch",
      "code": "HEADQUARTERS",
      "displayName": "Headquarters",
      "description": "Primary corporate office",
      "sortOrder": 1,
      "isActive": true
    }
    JSON
    ```
