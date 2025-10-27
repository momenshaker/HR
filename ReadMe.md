🧭 HR Project
🏢 Human Resources Management System (HRMS)










📘 Overview

HR Project is a full-featured Human Resources Management System (HRMS) built with .NET 8 and Next.js, providing seamless integration across HR, payroll, recruitment, and analytics.

It is designed for scalability, modularity, and extensibility — suitable for startups to enterprise-grade organizations.

💡 Built by Hercules IT Solutions under the technical direction of Mo’men Shaker (CTO).

🧩 Core Features
Category	Highlights
👤 Employee Management	Employee profiles, contracts, documents, job & department mapping
🏢 Organization Structure	Company hierarchy, reporting lines, role definitions
🗓️ Attendance & Leave	Shift tracking, leave requests, approvals, balance calculation
💸 Payroll Management	Automated payroll, payslips, tax/insurance deductions
📈 Performance Management	Goal setting, KPIs, feedback, performance reviews
📋 Recruitment	Job postings, candidate tracking, interview pipeline
🎓 Training	Learning modules, course tracking, certificates
💬 Communication	Announcements, feedback, recognition system
📊 Analytics	Dashboards, reporting, predictive insights
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
  "Jwt": {
    "Issuer": "https://yourdomain.com",
    "Audience": "https://yourdomain.com",
    "Key": "supersecretkey"
  },
  "Azure": {
    "SignalR": "Endpoint=https://signalr.yourdomain.net;",
    "ServiceBus": "Endpoint=sb://hr-bus.servicebus.windows.net/"
  }
}

# .env (Next.js)
NEXT_PUBLIC_API_URL=http://localhost:5000/api
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

GitHub Actions → Docker → Azure

Auto migration and secret sync via CI/CD

🔐 Security

Role-based access (Admin, HR, Manager, Employee)

JWT / Azure AD B2C authentication

Data encryption (at rest + in transit)

GDPR compliant data handling

Audit logs for all critical operations

📚 Documentation

AGENT.md
 — AI/Automation agent guide

/docs/api/ — OpenAPI specifications

/docs/architecture/ — Diagrams & design notes

/docs/changelog.md — Version history

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
