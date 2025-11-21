**Controller**
- Name: `EmployeesController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/employees`
- Version: unversioned route
- Auth: method-level policies for self operations

**Endpoints**
- GET `/api/employees` – List employees (paginated)
- GET `/api/employees/hierarchy` – Retrieve reporting hierarchy for occupied positions
- POST `/api/employees` – Create employee
- GET `/api/employees/{id}` – Get employee by id
- PUT `/api/employees/{id}` – Update employee (policy `EmployeeSelf`)
- DELETE `/api/employees/{id}` – Delete employee (policy `EmployeeSelf`)
- GET `/api/employees/{employeeId}/departments` – List employee’s departments
- POST `/api/employees/{employeeId}/departments:assign` – Assign department
- POST `/api/employees/{employeeId}/departments:replace` – Replace department assignments
- POST `/api/employees/{employeeId}/departments:unassign` – Remove department assignment

**Object Schemas**
- Requests: `CreateEmployeeRequest`, `UpdateEmployeeRequest`, `AssignEmployeeDepartmentRequest`, `ReplaceEmployeeDepartmentsRequest`
- Responses: `EmployeeDto`, `PaginatedResponse<EmployeeDto>`, `IReadOnlyCollection<DepartmentDto>`, `IReadOnlyCollection<EmployeeHierarchyNodeDto>`

**cURL Examples**
- List employees
  - curl -X GET "$BASE_URL/api/employees" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Get hierarchy
  - curl -X GET "$BASE_URL/api/employees/hierarchy" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Create employee
  - curl -X POST "$BASE_URL/api/employees" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "firstName": "Ada",
  "lastName": "Lovelace",
  "email": "ada@example.com",
  "employmentStartDate": "2025-01-01",
  "jobTitle": "Engineer",
  "employmentEndDate": null,
  "dateOfBirth": "1990-05-10",
  "departmentAssignment": {
    "primaryDepartmentId": "<uuid>",
    "secondaryDepartmentIds": ["<uuid>"]
  },
  "jobArchitecture": null,
  "contracts": [],
  "complianceDocuments": []
}
JSON
- Get by id
  - curl -X GET "$BASE_URL/api/employees/<id>" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Update (self)
  - curl -X PUT "$BASE_URL/api/employees/<id>" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d @- <<'JSON'
{
  "firstName": "Ada",
  "lastName": "Lovelace",
  "email": "ada@example.com",
  "employmentStartDate": "2025-01-01",
  "jobTitle": "Senior Engineer",
  "employmentEndDate": null,
  "dateOfBirth": "1990-05-10",
  "departmentAssignment": {
    "primaryDepartmentId": "<uuid>",
    "secondaryDepartmentIds": []
  },
  "jobArchitecture": null,
  "contracts": [],
  "complianceDocuments": []
}
JSON
- Delete (self)
  - curl -X DELETE "$BASE_URL/api/employees/<id>" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- List departments
  - curl -X GET "$BASE_URL/api/employees/<employeeId>/departments" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Assign department
  - curl -X POST "$BASE_URL/api/employees/<employeeId>/departments:assign" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d '{"departmentId":"<uuid>"}'
- Replace departments
  - curl -X POST "$BASE_URL/api/employees/<employeeId>/departments:replace" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d '{"departmentIds":["<uuid>"]}'
- Unassign department
  - curl -X POST "$BASE_URL/api/employees/<employeeId>/departments:unassign" \
        -H "Authorization: Bearer <token>" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d '{"departmentId":"<uuid>"}'
