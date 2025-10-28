# Changelog

## [Unreleased]
### Added
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
### Changed
- Refined public-facing feature descriptions in the README to better reflect the professional scope of the HR platform.

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
