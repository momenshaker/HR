API Specs

- Main OpenAPI: `docs/api/openapi.yaml`
- Recruitment module (new): `docs/api/recruitment.openapi.yaml`

Notes
- The recruitment spec is a self-contained OpenAPI 3.0 document for Jobs, Applications, Candidates, and Interviews.
- If you keep a single combined spec, import/merge `docs/api/recruitment.openapi.yaml` into `docs/api/openapi.yaml` (e.g., via `$ref` tooling or manual merge).
- Endpoints covered:
  - `GET /api/recruitment/jobs?orgId=&status=`
  - `POST /api/recruitment/jobs`
  - `PUT /api/recruitment/jobs/{id}`
  - `POST /api/recruitment/jobs/{id}:close`
  - `GET /api/recruitment/applications?jobId=&stage=`
  - `POST /api/recruitment/applications`
  - `POST /api/recruitment/applications/{id}:advance`
  - `POST /api/recruitment/interviews`
  - `PUT /api/recruitment/interviews/{id}`

Validation Highlights
- Candidate email uniqueness enforced on application create (409 on conflict).
- Application stage transitions validated; invalid transitions return 422.
- Hire stage supports optional employee creation via `createEmployee` flag.
