Performance — Test Plan

Scope
- Validate performance review cycles lifecycle and access.
- Enforce rating ranges and goal link integrity.
- Validate review submission workflow basics.

Test Matrix
- Cycle gates
  - Create cycle with valid dates → 201 + fields set.
  - Open cycle via `POST /api/performance/cycles/{id}:open` → `isOpen = true`.
  - Close cycle via `POST /api/performance/cycles/{id}:close` → `isOpen = false`.
  - Close already-closed cycle → 400/409 policy violation.
  - Open already-open cycle → 400/409 policy violation.

- Rating ranges
  - Create review with KPI rating < 1 → 400 validation error.
  - Create review with KPI rating > 5 → 400 validation error.
  - Update review with overallRating outside 1..5 → 400 validation error.

- Goal linkage
  - Create review with KPI referencing existing Goal (by `goalId`) → 201.
  - Create review with KPI referencing non-existent Goal → 400/422 with message.
  - Update review KPI to link/unlink a Goal → 200 and persisted association.

- Reviews listing filters
  - GET reviews by `cycleId` returns only matching records.
  - GET reviews by `employeeId` returns only employee reviews.
  - GET reviews by `managerId` returns only manager reviews.

- Concurrency (RowVersion)
  - Update review with stale `rowVersion` → 409 conflict (if enforced in service).

Notes
- Authentication required for all endpoints; use JWT from login flow.
- Additional policy tests (manager sign‑off, self‑review) can be added when service endpoints are exposed.
