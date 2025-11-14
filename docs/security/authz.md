# Authorization Matrix

The API enforces bearer authentication and role-based authorization across every module. The table below summarises the required roles per controller. Roles are cumulative; the `Admin` role automatically inherits the permissions of subordinate roles in downstream services.

| Module / Controller | Allowed Roles |
| --- | --- |
| AnalyticsSnapshots | Admin, HR |
| Announcements | Admin, HR, Manager |
| AttendanceRecords | Admin, HR, Manager |
| WorkSchedules | Admin, HR, Manager |
| EmployeeSchedules | Admin, HR, Manager |
| Holidays | Admin, HR, Manager |
| BillingWebhooks | Public (signature verification handled separately) |
| Candidates | Admin, HR |
| DelegatedAuthorities | Admin, HR |
| Departments | Admin, HR |
| EmployeeSelfService | Employee (scoped to `employeeId` path) |
| Employees | Admin, HR, Manager |
| EngagementCampaigns | Admin, HR |
| Interviews | Admin, HR |
| LeaveRequests | Admin, HR, Manager |
| OrganizationUnits | Admin, HR |
| PayrollRuns | Admin, HR |
| PerformanceReviews | Admin, HR, Manager |
| PlatformConfiguration | Admin |
| Positions | Admin, HR |
| PulseSurveys | Admin, HR, Manager |
| RecognitionPrograms | Admin, HR, Manager |
| RecruitmentInsights | Admin, HR |
| ReportingRelationships | Admin, HR |
| System | Public (health & version metadata) |
| TrainingCourses | Admin, HR, Manager |
| Vacancies | Admin, HR |

## Claims & Token Requirements

- All JWTs **must** include the `cust` claim identifying the tenant/customer scope. Requests without this claim are rejected during token validation.
- The `sub` (subject) or `nameidentifier` claim is used to correlate rate limiting, idempotency cache entries, and audit logs.
- Additional claims may be used downstream for fine-grained policy decisions; controllers can layer extra `[Authorize(Policy="...")]` decorators when needed.

## Public Endpoints

Only the following endpoints are publicly accessible:

- `POST /api/v1/billing/webhooks` — validated via Stripe signature header.
- `GET /api/v1/health` and `GET /api/v1/version` — used for monitoring and blue/green probes.

All other endpoints require a valid bearer token and one of the roles listed above.
