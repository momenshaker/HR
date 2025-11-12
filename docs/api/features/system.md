**Overview**
- Feature: System
- Auth: Public (no bearer required).

**Endpoints**
- GET `/api/v1/health` - Report API health status
  - Auth: Public
  - Request: none
  - Responses:
    - 200 - `SystemHealthDto`
- GET `/api/v1/version` - Retrieve API version information
  - Auth: Public
  - Request: none
  - Responses:
    - 200 - `SystemVersionDto`

**DTOs**
- SystemHealthDto - API health snapshot
  - status (string)
  - environment (string)
  - timestamp (string, date-time)
- SystemVersionDto - API version info
  - version (string)
  - environment (string)
- ErrorResponse - Error payload
  - code (string) - Machine readable error code
  - message (string) - Human readable message
  - traceId (string) - Distributed trace id

