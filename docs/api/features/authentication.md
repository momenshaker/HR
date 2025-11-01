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

