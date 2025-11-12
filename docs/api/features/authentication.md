**Overview**
- Feature: Authentication
- Auth: Public for login (no bearer required).

**Endpoints**
- POST `/api/v1/auth/login` — Authenticate user credentials
  - Security: none (overrides default)
  - Request: `LoginRequest`
  - Responses:
    - 200 → `AuthResponse`
    - 400 → `ErrorResponse`

**DTOs**
- LoginRequest — Login credentials
  - email (string, email)
  - password (string, password)
- AuthResponse — JWT tokens
  - accessToken (string) — Signed JWT
  - tokenType (string) — e.g., Bearer
  - expiresIn (integer, int32) — Seconds
  - refreshToken (string)
- ErrorResponse — Error payload
  - code (string), message (string), traceId (string)

**Additional Endpoints (from controllers)**
- POST `/api/v1/auth/register-employee` - Register a user and link to an employee (public)
- POST `/api/v1/auth/refresh` - Refresh access token
- GET `/api/v1/auth/me` - Get current authenticated user profile
- POST `/api/v1/auth/link-employee` - Link an identity to an employee
- POST `/api/v1/auth/register` - Register a new identity user (public)
- POST `/api/v1/auth/confirm-email` - Confirm email using token (public)
- POST `/api/v1/auth/resend-confirmation` - Generate new email confirmation token (public)
- POST `/api/v1/auth/forgot-password` - Generate password reset token (public)
- POST `/api/v1/auth/reset-password` - Reset password using token (public)
- POST `/api/v1/auth/change-password` - Change password
- GET `/api/v1/auth/users/{userId}/roles` - Get roles for a user
- POST `/api/v1/auth/users/{userId}/roles` - Assign roles to a user
- DELETE `/api/v1/auth/users/{userId}/roles` - Remove roles from a user
- GET `/api/v1/auth/users/{userId}/claims` - Get claims for a user
- POST `/api/v1/auth/users/{userId}/claims` - Add claims to a user
- DELETE `/api/v1/auth/users/{userId}/claims` - Remove claims from a user
- POST `/api/v1/auth/users/{userId}/lockout` - Lock out a user
