# Lookups Feature

Dynamic lookup management lets administrators own every drop-down/value list (branches, industries, business units, operating hours, etc.) without redeploying the platform.

## Highlights
- **Single source of truth** – The `LookupService` normalizes categories/codes, enforces uniqueness, assigns sort order, and emits deterministic version tokens for caching.
- **Cache friendly API** – `GET /api/v1/lookups` returns an `ETag` header (the version token); clients send `If-None-Match` to receive `304 Not Modified` when nothing changed.
- **Granular reads** – `GET /api/v1/lookups/category/{category}` powers lightweight drop-down hydration without downloading every category.
- **Full CRUD** – HR/Admin roles call `POST/PUT/DELETE /api/v1/lookups/{id}` to manage values. Duplicate `(category, code)` pairs surface HTTP 409 conflicts.
- **Audited + authorized** – Controller is protected by `[Authorize]` + `[AuditResource("LookupValue")]`, so every mutation is logged with the actor/trace id.
- **Signal-driven UI** – The Angular `/lookups` page uses the `LookupStore` signal to surface categories, inline editing dialogs, and optimistic updates with snackbar feedback.

## API Contracts
- `LookupCollectionDto` – `{ versionToken: string, categories: LookupCategoryDto[] }`.
- `LookupCategoryDto` – `{ category: string, values: LookupValueDto[] }` (values sorted by `sortOrder`, then name).
- `LookupValueDto` – `{ id, category, code, displayName, description?, sortOrder, isActive, updatedAtUtc }`.
- `CreateLookupValueRequest` / `UpdateLookupValueRequest` – same shape as DTO but without `id`/timestamps; `sortOrder` is optional (auto assigned when omitted).

## Usage Patterns
1. **Bootstrap** – On app start, the frontend seeds from `DEFAULT_LOOKUP_SEED` while the store issues `GET /api/v1/lookups` with `If-None-Match` so cached payloads can skip the download.
2. **Mutations** – Dialogs issue `POST/PUT` and update the local signal immediately; the store clears the cached ETag so the next reload forces the server to recompute the collection.
3. **Invalidation** – Backend writes update the `UpdatedAtUtc` timestamp, which feeds the SHA-256 version token used for conditional requests.
4. **Fallbacks** – If the API is unreachable the UI surfaces the seed data and shows inline snackbar errors; administrators can still read cached categories.

## UI Snapshot
- Route: `/lookups` (guarded for Admin + HR).
- Components: `LookupsPageComponent` (category list + table) and `LookupValueDialogComponent` (create/edit form with autocomplete category input and status toggle).
- Actions: Refresh (force API call), add/edit/delete values, automatic selection of newly created categories.

## Operational Notes
- Adding a new category is as simple as typing a new name when creating a value; no extra API needed.
- Sorting: if `sortOrder` <= 0 the service asks the repository for `GetNextSortOrderAsync` and defaults to `1` if empty.
- Repositories implement both EF Core and in-memory versions, and the EF migration seeds the canonical categories so existing deployments start populated.
