# HR Frontend (Angular)

This project provides the Angular 17 + Angular Material frontend for the HR platform. The application uses standalone components, Angular signals for local state management, and clients generated from the backend OpenAPI specifications.

## Prerequisites

- Node.js 18+
- npm 9+

## Installation

```bash
npm install
```

## Development server

```bash
npm start
```

Navigate to `http://localhost:4200/`. The app automatically reloads if you change any of the source files.

## Build

```bash
npm run build
```

The build artifacts are stored in the `dist/` directory.

## Running tests

```bash
npm test
```

## Environment configuration

Environment variables are defined in `src/environments/`. Update the following keys based on your deployment target:

- `apiBaseUrl`: Base URL of the HR backend API.
- `tokenStorageKey` / `refreshTokenStorageKey`: Local storage keys for JWT tokens.
- `themeStorageKey`: Local storage key for UI theme preference.

## OpenAPI client generation

The API clients are generated with [`openapi-typescript-codegen`](https://github.com/ferdikoomen/openapi-typescript-codegen). Run the following commands whenever the backend contract changes:

```bash
npm run generate:api
npm run generate:api:core
npm run generate:api:recruitment
```

Generated files are written to `src/app/core/api-client`. Do not edit them manually.

## Folder structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── core/          # Authentication, configuration, interceptors, API client factory
│   │   ├── shared/        # Reusable UI components
│   │   ├── features/      # Feature domains (organizations, employees, payroll, etc.)
│   │   └── pages/         # Error and fallback pages
│   ├── assets/
│   ├── environments/
│   └── main.ts
├── angular.json
├── package.json
└── README.md
```

## Problem details & error handling

The HTTP interceptors translate backend `ProblemDetails` responses into user-friendly snackbar messages and inline form errors. Validation errors are surfaced next to form inputs.

## Role-based access control

Navigation items and feature routes are protected by the `authGuard` and `roleGuard`. The guards rely on the roles array returned by the `/auth/me` endpoint.

## Theme toggle

The light/dark theme preference is stored in local storage and applied on bootstrap. Use the toolbar toggle to switch between themes.

## API pagination

All list pages consume the backend pagination metadata via the shared `DataTableComponent`. Server-side pagination and filtering are forwarded to the API with consistent query parameters (`page`, `pageSize`, `search`, `sort`, `direction`).
