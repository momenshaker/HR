export interface PaginatedResponse<T> {
  /**
   * Collection of items returned by the API for the requested page.
   */
  items: ReadonlyArray<T>;

  /**
   * Total number of records available server-side.
   */
  totalCount: number;

  /**
   * Optional current page number (1-indexed) if provided by the backend.
   */
  pageNumber?: number;

  /**
   * Optional page size if provided by the backend.
   */
  pageSize?: number;
}
