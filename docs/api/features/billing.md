**Overview**
- Feature: Billing
- Auth: Bearer JWT for plan/usage endpoints. Webhook is typically verified by Stripe signature.

**Endpoints**
- GET `/api/billing/plans` — Retrieve billing plans
  - Auth: Bearer
  - Request: none
  - Responses:
    - 200 → array<`PlanDto`>
    - 401 → `ErrorResponse`
- GET `/api/billing/usage` — Retrieve usage snapshots
  - Auth: Bearer
  - Request: none
  - Responses:
    - 200 → `UsageSnapshotDto`
    - 401 → `ErrorResponse`
- POST `/api/billing/webhooks/stripe` — Receive Stripe webhook notifications
  - Auth: No bearer (use Stripe signature verification)
  - Request body (inline):
    - id (string) — Stripe event id
    - type (string) — Stripe event type
    - created (integer, int64) — Unix timestamp
    - data (object) — Raw event payload
  - Responses:
    - 202 → Accepted (no body)
    - 401 → `ErrorResponse`

**DTOs**
- PlanDto — Billing plan
  - id (string, uuid)
  - name (string)
  - currency (string)
  - amount (number, double)
  - interval (string: Monthly|Yearly)
  - description (string)
  - features (array<string>)
- UsageSnapshotDto — Usage period snapshot
  - periodStart (string, date-time)
  - periodEnd (string, date-time)
  - unitsConsumed (number, double)
  - unitType (string)
  - cost (number, double)
  - currency (string)
- ErrorResponse — Error payload
  - code (string), message (string), traceId (string)

