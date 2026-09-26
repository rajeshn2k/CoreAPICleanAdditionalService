namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// Pagination metadata for list responses
    /// </summary>
    public class PaginationMetadata
    {
        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Indicates whether there is a previous page
        /// </summary>
        public bool HasPrevious { get; set; }

        /// <summary>
        /// Indicates whether there is a next page
        /// </summary>
        public bool HasNext { get; set; }

        public PaginationMetadata()
        {
        }

        public PaginationMetadata(int currentPage, int pageSize, int totalItems)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalItems = totalItems;
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            HasPrevious = CurrentPage > 1;
            HasNext = CurrentPage < TotalPages;
        }

        public static PaginationMetadata Create(int currentPage, int pageSize, int totalItems)
        {
            return new PaginationMetadata(currentPage, pageSize, totalItems);
        }
    }
}