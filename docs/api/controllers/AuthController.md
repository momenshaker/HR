**Controller**
- Name: `AuthController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/auth`
- Version: `[ApiVersion("1.0")]`
- Auth: Mixed – `[AllowAnonymous]` on public operations; otherwise bearer auth
- Feature: `PlatformServices`

**Endpoints**
- POST `/api/v1/auth/login` – Authenticate user (public)
  - Produces: `AuthResponse` (200), `ErrorResponse` (400)
- POST `/api/v1/auth/register-employee` – Register a user and link to an employee (public)
  - Produces: `RegistrationResponse` (201), `ErrorResponse` (400/409/422)
- POST `/api/v1/auth/refresh` – Refresh access token
- GET `/api/v1/auth/me` – Current user profile
- POST `/api/v1/auth/link-employee` – Link identity to employee
- POST `/api/v1/auth/register` – Register identity (public)
  - Produces: `RegistrationResponse` (200), `ErrorResponse` (400)
- POST `/api/v1/auth/confirm-email` – Confirm email (public)
  - Produces: No Content (204), `ErrorResponse` (400)
- POST `/api/v1/auth/resend-confirmation` – Resend confirmation (public)
  - Produces: `TokenResponse` (200), `ErrorResponse` (404)
- POST `/api/v1/auth/forgot-password` – Generate reset token (public)
  - Produces: `TokenResponse` (200), `ErrorResponse` (404)
- POST `/api/v1/auth/reset-password` – Reset password (public)
- POST `/api/v1/auth/change-password` – Change password
- GET `/api/v1/auth/users/{userId}/roles` – Get roles
- POST `/api/v1/auth/users/{userId}/roles` – Assign roles
- DELETE `/api/v1/auth/users/{userId}/roles` – Remove roles
- GET `/api/v1/auth/users/{userId}/claims` – Get claims
- POST `/api/v1/auth/users/{userId}/claims` – Add claims
- DELETE `/api/v1/auth/users/{userId}/claims` – Remove claims
- POST `/api/v1/auth/users/{userId}/lockout` – Lock out user

**Object Schemas**
- Requests: `LoginRequest`, `RegisterEmployeeRequest`, `RegisterUserRequest`, `ConfirmEmailRequest`, `ResendConfirmationRequest`, `ForgotPasswordRequest`, `ResetPasswordRequest`, `ChangePasswordRequest`
- Responses: `AuthResponse`, `RegistrationResponse`, `TokenResponse`, `ErrorResponse`

**cURL Examples**
- Login (public)
  - curl -X POST "$BASE_URL/api/v1/auth/login" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"email\":\"user@example.com\",\"password\":\"P@ssw0rd!\"}"
- Register + link employee (public)
  - curl -X POST "$BASE_URL/api/v1/auth/register-employee" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"employeeId\":\"<uuid>\",\"email\":\"user@example.com\",\"userName\":\"user1\",\"password\":\"P@ssw0rd!\"}"
- Refresh token
  - curl -X POST "$BASE_URL/api/v1/auth/refresh" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Me
  - curl -X GET "$BASE_URL/api/v1/auth/me" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Link employee
  - curl -X POST "$BASE_URL/api/v1/auth/link-employee" -H "Authorization: Bearer <token>" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"userName\":\"user1\",\"employeeId\":\"<uuid>\"}"
- Register (public)
  - curl -X POST "$BASE_URL/api/v1/auth/register" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"email\":\"user@example.com\",\"password\":\"P@ssw0rd!\",\"customerId\":\"<uuid>\"}"
- Confirm email (public)
  - curl -X POST "$BASE_URL/api/v1/auth/confirm-email" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"userId\":\"<uuid>\",\"token\":\"<token>\"}"
- Resend confirmation (public)
  - curl -X POST "$BASE_URL/api/v1/auth/resend-confirmation" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"userId\":\"<uuid>\"}"
- Forgot password (public)
  - curl -X POST "$BASE_URL/api/v1/auth/forgot-password" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"email\":\"user@example.com\"}"
- Reset password (public)
  - curl -X POST "$BASE_URL/api/v1/auth/reset-password" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"userId\":\"<uuid>\",\"token\":\"<token>\",\"newPassword\":\"P@ssw0rd!\"}"
- Change password
  - curl -X POST "$BASE_URL/api/v1/auth/change-password" -H "Authorization: Bearer <token>" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"currentPassword\":\"P@ssw0rd!\",\"newPassword\":\"N3wP@ss!\"}"
- Get roles
  - curl -X GET "$BASE_URL/api/v1/auth/users/<userId>/roles" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Assign roles
  - curl -X POST "$BASE_URL/api/v1/auth/users/<userId>/roles" -H "Authorization: Bearer <token>" -H "Accept: application/json" -H "Content-Type: application/json" -d "[\"Admin\"]"
- Remove roles
  - curl -X DELETE "$BASE_URL/api/v1/auth/users/<userId>/roles" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Get claims
  - curl -X GET "$BASE_URL/api/v1/auth/users/<userId>/claims" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Add claims
  - curl -X POST "$BASE_URL/api/v1/auth/users/<userId>/claims" -H "Authorization: Bearer <token>" -H "Accept: application/json" -H "Content-Type: application/json" -d "{\"department\":\"Engineering\"}"
- Remove claims
  - curl -X DELETE "$BASE_URL/api/v1/auth/users/<userId>/claims" -H "Authorization: Bearer <token>" -H "Accept: application/json"
- Lockout
  - curl -X POST "$BASE_URL/api/v1/auth/users/<userId>/lockout" -H "Authorization: Bearer <token>" -H "Accept: application/json"
