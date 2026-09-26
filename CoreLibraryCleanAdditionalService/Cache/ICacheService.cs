namespace Core.Library.Clean.AdditionalService
{
    /// <summary>
    /// Interface for cache service operations
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Gets a value from cache by key
        /// </summary>
        Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets a value in cache with optional expiration
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes a value from cache by key
        /// </summary>
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes values from cache by pattern
        /// </summary>
        Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a key exists in cache
        /// </summary>
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes all keys matching a pattern
        /// </summary>
        Task RemoveAllByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    }
}