# Changelog

## [Unreleased]
### Added
- Expanded leave management with richer leave type metadata, per-organization policies, enhanced request tracking (days, attachments, multi-step statuses), updated REST/EF layers, and refreshed Angular self-service workflows that surface dynamic leave types.
- Delivered end-to-end lookup management covering the `LookupService`, repositories, controller, Angular admin workspace, and documentation for cache-aware dynamic catalogues.
- Exposed organization, department, and employee assignment endpoints with nested routing, pagination, and documented OpenAPI updates for hierarchy management workflows.
- Enriched employee master data with contract history, compliance documentation, job architecture, and department alignment surfaced through the Employees API and DTOs.
- Introduced a development authentication endpoint issuing JWTs for Postman and manual testing, backed by configurable in-memory accounts and documented default credentials.
- Delivered platform system diagnostics endpoints for health and version reporting alongside integration test coverage.
- Published v0.4.0 OpenAPI specification detailing authentication, billing, subscription, and audit surfaces with JWT security.
- Introduced a platform configuration API and service exposing feature toggle state and repository settings for clients.
- Delivered advanced employee search API supporting filtering, sorting, and pagination for operational reporting.
- Introduced workforce analytics snapshot endpoint exposing headcount, movement, and department-level insights.
- Added comprehensive employee self-service endpoints for submitting leave, clocking in/out, viewing salary slips, and browsing training opportunities.
- Implemented the EmployeeSelfService application service with dedicated DTOs and test coverage to orchestrate cross-module workflows.
- Extended the public OpenAPI specification with salary slip models and self-service routes for employee experiences.
- Enriched performance management with cascaded goals, KPI tracking, structured feedback, and compensation review modelling for end-to-end appraisal cycles.
- Delivered recruitment enhancements covering vacancy publishing APIs, pipeline automation with interview scheduling, and collaborative hiring insights for hiring teams.
- Expanded training and development with competency-aligned course metadata, enrollment management workflows, progress analytics, and certification governance endpoints.
- Documented capability deep dives in the README to highlight communications, time and attendance, organisation design, master data, and payroll coverage.
- Rolled out internal communications APIs spanning announcements, engagement campaigns, pulse surveys, and recognition programme governance with supporting services and repositories.
- Expanded organization design with multi-level units, position management, reporting hierarchies, delegated authority modelling, and OAuth-enabled self-service account management across services, repositories, APIs, and documentation.
### Changed
- Synced the public OpenAPI specification with the new leave types, balances, and request lifecycle endpoints exposed by the v1
  LeaveController.
- Updated README and API documentation to clarify that all default lookups are now dynamic, ETag-enabled resources managed via `/lookups` in the frontend.
- Refined public-facing feature descriptions in the README to better reflect the professional scope of the HR platform.
- Clarified attendance and time tracking narratives across platform configuration and API documentation to emphasise shift
  orchestration, time capture, entitlement policies, approval flows, and real-time balance reconciliation.
- Introduced pre-validation for organization, department, and employee uniqueness to surface 409 Conflict responses before
  persistence and mapped detailed 422 validation codes for hierarchy consistency checks.
- Updated department DTOs to emit child collections for hierarchy views and aligned API documentation with the new payload shape
  and error responses.
- Scoped the DepartmentEmployees API under organizations to enforce the organization → department → employee hierarchy and
  return 404 responses for cross-organization lookups.

### Fixed
- Added an Entity Framework Core migration to provision ASP.NET Core Identity tables so seeded admin accounts can be created without runtime SQL errors.

# [0.3.2] - 2025-02-28
### Added
- Introduced strongly-typed database configuration options allowing connection strings to be resolved from the configuration root.
- Extended default configuration to include relational database settings and documented usage in the README.

## [0.3.1] - 2025-02-20
### Added
- Introduced centralised configuration via <code>appsettings.json</code>, exposing feature toggles and repository provider settings.
- Added feature gating attribute for API controllers to respect configuration switches at runtime.
- Covered feature gate behaviour with dedicated unit tests to guard configuration changes.

## [0.3.0] - 2025-02-14
### Added
- Delivered full suite of core HR modules including organization structure, attendance, leave, payroll, performance, recruitment, training, communications, and analytics services.
- Introduced REST controllers, DTOs, mappings, service abstractions, and in-memory repositories for each new module.
- Expanded automated unit test coverage across all core services to protect CRUD workflows.

## [0.2.0] - 2024-06-10
### Added
- Implemented update and delete operations for the Employees API and supporting service/repository layers.
- Added unit tests covering employee update and delete scenarios.

## [0.1.0] - 2024-06-09
### Added
- Initialized backend solution structure following clean architecture guidelines.
- Added Employee domain entity, application services, and in-memory infrastructure.
- Implemented initial RESTful Employees API endpoints.
- Added unit tests for employee service behavior.
