# Entity Tag (ETag) Support

The HR API issues strong ETags for every successful `GET` request that retrieves a single resource by identifier. The behaviour is implemented at middleware level and applies automatically across controllers.

## How it works

1. The middleware buffers the JSON payload returned by the controller.
2. A SHA-256 hash of the payload is generated and encoded as a base64 ETag.
3. The ETag is written to the `ETag` response header (e.g. `"m1M2..."`).
4. Clients can send the value back via the `If-None-Match` header to perform cache validation.
5. If the hash matches, the API short-circuits the pipeline and returns `304 Not Modified` with an empty body.

## Client Guidance

- Persist the last seen ETag per resource identifier.
- Include `If-None-Match: "<etag>"` on subsequent GET calls to avoid unnecessary payload downloads.
- Treat 304 responses as confirmation that local cache remains valid.
- A change in payload (including ordering) produces a new ETag, ensuring consistency across distributed caches.

## Error Handling

- If the action does not return `200 OK`, the middleware streams the original response without ETag generation.
- When combined with `Idempotency-Key` logic and rate limiting, the middleware ensures safe caching without sacrificing auditability or traceability.
