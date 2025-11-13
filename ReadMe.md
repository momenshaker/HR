🧭 HR Project
🏢 Human Resources Management System (HRMS)










📘 Overview

HR Project is a full-featured Human Resources Management System (HRMS) built with .NET 8 and Next.js, providing seamless integration across HR, payroll, recruitment, and analytics.

It is designed for scalability, modularity, and extensibility — suitable for startups to enterprise-grade organizations.

💡 Built by Hercules IT Solutions under the technical direction of Mo’men Shaker (CTO).

🧩 Core Platform Capabilities

The platform ships with a mature catalogue of HR modules that are ready for enterprise adoption out of the box:

| Module | Enterprise-grade capabilities |
| --- | --- |
| 👤 Employee Management | Centralised employee master data with contract history, compliance documents, job architecture, and department alignment. |
| 🏢 Organisation Design | Multi-level organisational charts, reporting hierarchies, position management, and delegated authority modelling. |
| 🗓️ Attendance & Leave | Shift orchestration, time capture, entitlement policies, approval flows, and real-time balance reconciliation. |
| 💸 Payroll Management | Gross-to-net payroll automation, statutory deductions, payslip distribution, and audit-ready payroll cycle tracking. |
| 📈 Performance Management | Goal cascading, KPI tracking, feedback cycles, and structured performance and compensation reviews. |
| 📋 Recruitment & ATS | Vacancy publishing, candidate pipeline automation, interview scheduling, and collaborative hiring insights. |
| 🎓 Learning & Development | Competency-aligned courses, enrolment management, progress analytics, and certification governance. |
| 💬 Employee Communications | Multi-channel announcements, engagement campaigns, pulse surveys, and recognition frameworks. |
| 📊 People Analytics | Executive dashboards, workforce benchmarking, predictive attrition modelling, and exportable compliance reports. |

### Capability deep dives

- **Employee communications.** Campaign managers can plan and deliver multi-channel announcements (email, in-app, SMS), automate engagement campaigns, launch quick pulse surveys, and drive culture programmes with built-in recognition frameworks.
- **Attendance, time, and leave.** Shift orchestration, real-time time capture (web, kiosk, mobile), configurable entitlement policies, routed approval flows, and instant balance reconciliation give managers total visibility of workforce availability.
- **Organisation design.** Visual, multi-level organisational charts, flexible reporting hierarchies, position management, and delegated authority modelling provide accurate insight into reporting structures.
- **Employee master data.** Maintain a centralised employee master record with full contract history, compliance document storage, job architecture mapping, and department alignment for downstream systems.
- **Payroll management.** Automate gross-to-net payroll runs, calculate statutory deductions, distribute payslips securely, and maintain audit-ready payroll cycle tracking for compliance reviews.
⚙️ Architecture Overview

Architecture Pattern:
🧱 Clean Architecture (Domain-Driven + Layered + CQRS)

Structure:

src/
 ├── Api/                # Controllers, routes
 ├── Application/        # Business logic, CQRS handlers, DTOs
 ├── Domain/             # Entities, aggregates, value objects
 ├── Infrastructure/     # Data, repositories, integrations
 └── Web/                # Next.js frontend


Key Technologies:

Backend: .NET 8 Web API, C#

Frontend: Next.js (TypeScript)

Database: SQL Server / PostgreSQL

Realtime: Azure SignalR / WebSockets

Auth: Azure AD B2C / JWT

Cloud: Azure Functions, Storage, Service Bus

Testing: xUnit, Jest, React Testing Library

DevOps: GitHub Actions / Azure Pipelines

🏗️ Organization hierarchy model

### Materialized path strategy

- Departments are stored using a **materialized path** with the format `/org/{organizationId}/dept/{departmentId}/...`. The `Level` column mirrors the depth encoded in the path so tree traversal and sorting can be performed directly inside SQL queries.
- Each node keeps immutable audit metadata (`CreatedAtUtc`, `UpdatedAtUtc`) and the path is recomputed whenever a department is moved so that every descendant inherits the updated prefix.
- The canonical sample data (Acme Corp, Head Office → Engineering → Platform/Applications, HR) is seeded through `OrganizationStructureSeeder` and mirrored in the EF migration to keep the database and tests aligned.

### Department API overview

| Route | Description |
| --- | --- |
| `GET /api/organizations/{organizationId}/departments?hierarchy=true` | Returns the full hierarchy with nested `Children` collections ordered by materialized path. |
| `POST /api/organizations/{organizationId}/departments` | Creates a department and automatically computes `Path`, `Level`, and timestamps. |
| `POST /api/organizations/{organizationId}/departments/{departmentId}:move` | Reparents a department (and its subtree) within the same organization. |
| `PUT /api/organizations/{organizationId}/departments/{departmentId}` | Updates metadata; parent changes are blocked and must go through the move endpoint. |
| `DELETE /api/organizations/{organizationId}/departments/{departmentId}?cascade=true` | Removes a department or its entire subtree when cascading. |

### Hierarchy invariants

- Department names are unique per `(OrganizationId, ParentDepartmentId)` and codes are unique per organization (empty codes are ignored).
- Cross-organization moves and assignments are rejected; every hierarchy mutation validates that the target parent belongs to the same organization.
- The move workflow prevents cycles (a department cannot become its own ancestor) and updates both `Path` and `Level` for every affected descendant.
- Employees may belong to multiple departments, but assignment operations deduplicate identifiers and enforce that all chosen departments originate from the same organization.

🚀 Getting Started
🧰 Prerequisites
Requirement	Version
.NET SDK
	8.0+
Node.js
	18+
SQL Server
 or PostgreSQL	Latest
Docker
	(optional)
🔧 Installation
# Clone the repository
git clone https://github.com/your-org/hr-project.git
cd hr-project

# Backend setup
cd src/Api
dotnet restore
dotnet build
dotnet ef database update

# Frontend setup
cd ../Web
npm install
npm run dev


Then open:

Frontend: http://localhost:3000
API: http://localhost:5000/api
Swagger: http://localhost:5000/swagger

⚙️ Configuration

Create a .env file (for frontend) and appsettings.Development.json (for backend) with:

// appsettings.Development.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HRProject;Trusted_Connection=True;"
  },
  "HrPlatform": {
    "Data": {
      "RepositoryProvider": "SqlServer",
      "Database": {
        "ConnectionStringName": "DefaultConnection",
        "Provider": "SqlServer",
        "EnableDetailedErrors": false,
        "EnableSensitiveDataLogging": false
      }
    }
  },
  "Jwt": {
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourdomain.com",
    "Key": "supersecretkey",
    "CustomerClaim": "cust"
  },
  "RateLimit": {
    "RequestsPerWindow": 100,
    "WindowSeconds": 60
  },
  "Idempotency": {
    "WindowHours": 24
  },
  "Azure": {
    "SignalR": "Endpoint=https://signalr.yourdomain.net;",
    "ServiceBus": "Endpoint=sb://hr-bus.servicebus.windows.net/"
  }
}

# .env (Next.js)
NEXT_PUBLIC_API_URL=http://localhost:5000/api/v1
NEXT_PUBLIC_SIGNALR_URL=https://signalr.yourdomain.net

🧪 Testing
✅ Backend
cd tests/UnitTests
dotnet test

✅ Frontend
cd src/Web
npm test


Coverage Target: ≥ 80%

📦 Deployment
Environment	Example URL	Description
Development	localhost	Local dev environment
Staging	staging.hr.yourdomain.com	QA / UAT testing
Production	hr.yourdomain.com	Live environment

Deployment Options:

Azure Container Apps / AKS

## Identity and Auth

- ASP.NET Core Identity + EF Core 8 with `ApplicationUser` linked 1:1 to `Employee` via `ApplicationUser.EmployeeId` (unique when present).
- JWT configuration in `appsettings.json` under `Jwt`: `Issuer`, `Audience`, `Key`, `AccessTokenMinutes`, `RefreshTokenDays`.
- Endpoints under `api/v1.0/auth`:
  - POST `/auth/register-employee` { email, userName, password, employeeId }
  - POST `/auth/login` { email, password } (email can be username or email)
  - POST `/auth/refresh` { refreshToken }
  - GET `/auth/me`
  - POST `/auth/change-password` { currentPassword, newPassword }
  - POST `/auth/forgot-password` { email }
  - POST `/auth/reset-password` { email, token, newPassword }
  - POST `/auth/link-employee` (Admin) { email, userName, password, employeeId }

Authorization:
- Policy `EmployeeSelf`: self-only access to employee profile; Admin/Manager bypass.
- Policy `OrgScoped`: if a token includes `org_id`, the route `organizationId` must match.

Seeding:
- Baseline roles created (Admin, Manager, Employee) and dev users from `Authentication` config.
- A few employees are backfilled with accounts (dev password `DevUser!123`).

GitHub Actions → Docker → Azure

Auto migration and secret sync via CI/CD

🔐 Security

Role-based access (Admin, HR, Manager, Employee)

JWT / Azure AD B2C authentication — tokens must include the `cust` claim representing the tenant/customer scope.

### Development authentication

For local testing (including Postman collections), request a JWT by calling:

```
POST http://localhost:5000/api/v1/auth/login
Content-Type: application/json

{
  "email": "admin@local.dev",
  "password": "ChangeMe123!"
}
```

The response contains a `Bearer` access token valid for 60 minutes. Attach it to subsequent requests using the `Authorization: Bearer {token}` header. The token already includes the required `cust` tenant claim.

Data encryption (at rest + in transit)

GDPR compliant data handling

Audit logs for all critical operations

🚦 API Hardening Essentials

- **Global rate limiting**: 100 authenticated requests per 60 seconds per bearer token (configurable via `RateLimit` section).
- **Idempotency**: All `POST` endpoints require an `Idempotency-Key` header; repeated calls replay the original response for 24 hours.
- **Caching**: `GET` lookups return strong `ETag` headers; send `If-None-Match` to receive `304 Not Modified` when unchanged.
- **Audit logging**: Every create/update/delete operation is logged with actor, action, entity, entity id, and `traceId` correlation.

### Dynamic lookup management

- **Self-serve catalogues**: Branches, business units, time zones, industries, and any other lookup-driven experience are now editable via the Angular admin screen at `/lookups`.
- **Instant propagation**: The lookup store hydrates from the API and broadcasts changes to every form without redeployments; the UI falls back to default seeds when the API is offline.
- **Robust API surface**: `GET /api/v1/lookups` returns the full collection with strong `ETag` headers, `GET /api/v1/lookups/category/{category}` powers lightweight drop-downs, and `POST/PUT/DELETE /api/v1/lookups/{id}` allow HR/Admin roles to manage values.
- **Cache-friendly**: Clients send `If-None-Match` to reuse cached payloads, while mutations automatically invalidate the signal-based store and regenerate version tokens.

📚 Documentation

AGENT.md
 — AI/Automation agent guide

/docs/api/ — OpenAPI specifications

/docs/architecture/ — Diagrams & design notes

/docs/changelog.md — Version history

GET /api/platform/configuration — Runtime feature toggle and repository metadata feed for clients.

🧩 Roadmap

 Mobile App (React Native)

 AI-powered resume screening

 Predictive attrition analytics

 Employee chatbot assistant

 Multi-tenant SaaS configuration

👥 Contributing

See CONTRIBUTING.md
 for:

Branch naming conventions

Code style rules

Commit message format

PR workflow

Test & review checklist

👨‍💻 Maintainers
Name	Role	Contact
Mo’men Shaker	CTO / Lead Engineer	✉️ info@herculesit.com

Hercules IT Solutions	Owner	🌐 www.herculesit.com
🏁 License

This project is proprietary and owned by Hercules IT Solutions.
All rights reserved © 2025. Unauthorized copying or distribution is prohibited.
- #### Employee communications delivery stack

| Surface | Purpose | Key API endpoints |
| --- | --- | --- |
| Announcements | Broadcast urgent and evergreen messages across inbox, in-app, and SMS channels with acknowledgement tracking. | <code>GET/POST/PUT/DELETE</code> <code>/api/Announcements</code> |
| Engagement campaigns | Automate multi-channel nurture journeys with channel mixes, launch windows, and ownership metadata. | <code>GET/POST/PUT/DELETE</code> <code>/api/EngagementCampaigns</code> |
| Pulse surveys | Launch sentiment surveys with configurable question banks, response windows, and close dates. | <code>GET/POST/PUT/DELETE</code> <code>/api/PulseSurveys</code> |
| Recognition programmes | Curate nomination criteria, rewards, and peer-to-peer participation for culture initiatives. | <code>GET/POST/PUT/DELETE</code> <code>/api/RecognitionPrograms</code> |

