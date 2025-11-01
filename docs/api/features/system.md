**Overview**
- Feature: System
- Auth: Bearer JWT for all endpoints in this feature.

**Endpoints**
- GET `/api/health` — Report API health status
  - Auth: Bearer
  - Request: none
  - Responses:
    - 200 → `SystemHealthDto`
    - 401 → `ErrorResponse`
- GET `/api/version` — Retrieve API version information
  - Auth: Bearer
  - Request: none
  - Responses:
    - 200 → `SystemVersionDto`
    - 401 → `ErrorResponse`

**DTOs**
- SystemHealthDto — API health snapshot
  - status (string)
  - environment (string)
  - timestamp (string, date-time)
- SystemVersionDto — API version info
  - version (string)
  - environment (string)
- ErrorResponse — Error payload
  - code (string) — Machine readable error code
  - message (string) — Human readable message
  - traceId (string) — Distributed trace id

