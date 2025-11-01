**Overview**
- Feature: Audit
- Auth: Bearer JWT for all endpoints.

**Endpoints**
- GET `/api/audit` — List audit log entries
  - Request: none
  - Responses:
    - 200 → array<`AuditEntryDto`>
    - 401 → `ErrorResponse`

**DTOs**
- AuditEntryDto — Audit log entry
  - id (string, uuid)
  - action (string)
  - actor (string)
  - occurredAt (string, date-time)
  - target (string)
  - metadata (object<string,string>)
- ErrorResponse — Error payload
  - code (string), message (string), traceId (string)

