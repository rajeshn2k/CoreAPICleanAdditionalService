using Core.Library.Clean.AdditionalService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Resilience;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;

namespace Core.API.Clean.AdditionalService.Resilience
{
    /// <summary>
    /// Resilient decorator for ICacheService using Microsoft.Extensions.Resilience
    /// </summary>
    public sealed class ResilientCacheService : ICacheService
    {
        private const string PipelineKey = "Redis";

        private readonly ICacheService _innerCacheService;
        private readonly ResiliencePipeline _pipeline;
        private readonly ILogger<ResilientCacheService> _logger;

        public ResilientCacheService(
            ICacheService innerCacheService,
            ResiliencePipelineProvider<string> pipelineProvider,
            ILogger<ResilientCacheService> logger)
        {
            _innerCacheService = innerCacheService;
            _pipeline = pipelineProvider.GetPipeline(PipelineKey);
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        return await _innerCacheService.GetAsync<T>(key, token);
                    },
                    cancellationToken);
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning(ex, "Redis circuit breaker is open. Cache read skipped for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerCacheService.SetAsync(key, value, expiration, token);
                    },
                    cancellationToken);
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning(ex, "Redis circuit breaker is open. Cache write skipped for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerCacheService.RemoveAsync(key, token);
                    },
                    cancellationToken);
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning(ex, "Redis circuit breaker is open. Cache removal skipped for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerCacheService.RemoveByPatternAsync(pattern, token);
                    },
                    cancellationToken);
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning(ex, "Redis circuit breaker is open. Cache pattern removal skipped: {Pattern}", pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        return await _innerCacheService.ExistsAsync(key, token);
                    },
                    cancellationToken);
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning(ex, "Redis circuit breaker is open. Cache existence check skipped for key: {Key}", key);
                return false;
            }
        }

        public async Task RemoveAllByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                await _pipeline.ExecuteAsync(
                    async token =>
                    {
                        await _innerCacheService.RemoveAllByPatternAsync(pattern, token);
                    },
                    cancellationToken);
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning(ex, "Redis circuit breaker is open. Cache pattern removal skipped: {Pattern}", pattern);
            }
        }
    }
}
