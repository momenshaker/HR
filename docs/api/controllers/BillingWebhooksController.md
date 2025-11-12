**Controller**
- Name: `BillingWebhooksController`
- Namespace: `HR.Api.Controllers`

**Overview**
- Base Route: `api/v{version:apiVersion}/billing/webhooks` → `/api/v1/billing/webhooks`
- Version: `[ApiVersion("1.0")]`
- Auth: `[AllowAnonymous]` (signature verified)
- Feature: `PlatformServices`
- Audit: `AuditResource("BillingWebhook")`

**Endpoints**
- POST `/api/v1/billing/webhooks/stripe` – Receive Stripe webhook notifications
  - Produces: 200 OK, 400 Bad Request, 401 Unauthorized, 500 Internal Server Error

**Object Schemas**
- Incoming payload: Stripe event JSON (id, type, created, data)
- Internal DTOs: `StripeWebhookEvent`, `StripeInvoiceDto`, `StripeSubscriptionDto`

**cURL Examples**
- Stripe webhook (signature verified, no bearer)
  - curl -X POST "$BASE_URL/api/v1/billing/webhooks/stripe" \
        -H "Stripe-Signature: t=1700000000,v1=abcdef" \
        -H "Accept: application/json" \
        -H "Content-Type: application/json" \
        -d '{"id":"evt_123","type":"invoice.paid","created":1700000000,"data":{"object":{}}}'
