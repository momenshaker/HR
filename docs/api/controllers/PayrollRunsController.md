**Controller**
- Name: `PayrollRunsController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/payroll/runs`
- Version: unversioned route
- Auth: `[Authorize(Roles = "Admin,HR")]`
- Feature: `PayrollManagement`
- Audit: `AuditResource("PayrollRun")`

**Endpoints**
- GET `/api/payroll/runs` – List payroll runs
- GET `/api/payroll/runs/{id}` – Get payroll run by id
- GET `/api/payroll/runs/{id}/items` – Get payroll run items
- POST `/api/payroll/runs` – Create payroll run
- POST `/api/payroll/runs/{id}:calculate` – Calculate payroll run
- POST `/api/payroll/runs/{id}:approve` – Approve payroll run
- POST `/api/payroll/runs/{id}:paid` – Mark payroll run as paid
- POST `/api/payroll/runs/{id}:payslips` – Generate payslips (optional)
- GET `/api/payroll/payslips` – List payslips

**Object Schemas**
- Responses: `PayrollRunDto`, `IReadOnlyCollection<PayrollRunDto>`, `IReadOnlyCollection<PayrollRunItemDto>`, `IReadOnlyCollection<SalarySlipDto>`

**cURL Examples**
- List runs
  - curl -X GET "$BASE_URL/api/payroll/runs" -H "Authorization: Bearer <token>"
- Get by id
  - curl -X GET "$BASE_URL/api/payroll/runs/<id>" -H "Authorization: Bearer <token>"
- Get items
  - curl -X GET "$BASE_URL/api/payroll/runs/<id>/items" -H "Authorization: Bearer <token>"
- Create run
  - curl -X POST "$BASE_URL/api/payroll/runs" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "organizationId": "<uuid>",
  "periodStart": "2025-01-01",
  "periodEnd": "2025-01-31",
  "notes": "January payroll"
}
JSON
- Calculate
  - curl -X POST "$BASE_URL/api/payroll/runs/<id>:calculate" -H "Authorization: Bearer <token>"
- Approve
  - curl -X POST "$BASE_URL/api/payroll/runs/<id>:approve" -H "Authorization: Bearer <token>"
- Mark as paid
  - curl -X POST "$BASE_URL/api/payroll/runs/<id>:paid" -H "Authorization: Bearer <token>"
- Generate payslips
  - curl -X POST "$BASE_URL/api/payroll/runs/<id>:payslips" -H "Authorization: Bearer <token>"
- List payslips
  - curl -X GET "$BASE_URL/api/payroll/payslips" -H "Authorization: Bearer <token>"
