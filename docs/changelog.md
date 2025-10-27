# Changelog

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
