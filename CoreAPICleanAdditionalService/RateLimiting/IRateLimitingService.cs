namespace Core.API.Clean.AdditionalService.RateLimiting
{
    public interface IRateLimitingService
    {
        Task<bool> IsAllowedAsync(string key, int limit, TimeSpan period, CancellationToken cancellationToken = default);
        Task<int> GetRemainingRequestsAsync(string key, int limit, CancellationToken cancellationToken = default);
    }
}