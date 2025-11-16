export interface PaginatedResponse<T> {
  readonly pageNumber: number;
  readonly pageSize: number;
  readonly totalCount: number;
  readonly totalPages: number;
  readonly items: readonly T[];
}
