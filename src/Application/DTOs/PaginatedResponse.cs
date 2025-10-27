namespace HR.Application.DTOs;

/// <summary>
///     Represents a single page of results.
/// </summary>
/// <typeparam name="T">The type of payload contained in the result set.</typeparam>
public sealed class PaginatedResponse<T>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PaginatedResponse{T}" /> class.
    /// </summary>
    public PaginatedResponse(int pageNumber, int pageSize, int totalCount, IReadOnlyCollection<T> items)
    {
        if (pageNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber), pageNumber, "Page number must be a positive integer.");
        }

        if (pageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), pageSize, "Page size must be a positive integer.");
        }

        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount < 0 ? 0 : totalCount;
        Items = items ?? Array.Empty<T>();
    }

    /// <summary>
    ///     1-based index representing the current page.
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    ///     Number of items requested per page.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    ///     Total number of items matching the query across all pages.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    ///     Total number of pages available.
    /// </summary>
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    ///     Result items contained on the requested page.
    /// </summary>
    public IReadOnlyCollection<T> Items { get; }

    /// <summary>
    ///     Indicates whether the current page represents the final page of results.
    /// </summary>
    public bool IsLastPage => TotalPages == 0 || PageNumber >= TotalPages;
}
