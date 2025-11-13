**Controller**
- Name: `DepartmentEmployeesController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/organizations/{organizationId}/departments/{departmentId}/employees`
- Version: unversioned route
- Auth: not specified at controller level

**Endpoints**
- GET `/api/organizations/{organizationId}/departments/{departmentId}/employees` – List employees in department

**Object Schemas**
- Responses: `IReadOnlyCollection<EmployeeDto>`

**cURL Examples**
- List employees in department
  - curl -X GET "$BASE_URL/api/organizations/<organizationId>/departments/<departmentId>/employees" -H "Authorization: Bearer <token>" -H "Accept: application/json"
