using Core.API.Clean.AdditionalService.Resilience;
using Core.Library.Clean.AdditionalService;
using Core.Library.Clean.AdditionalService.ResilientTest;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;
using StackExchange.Redis;

namespace Core.API.Clean.AdditionalService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton(configuration);

            // Logger
            //services.AddSingleton<ServiceActionLogger>();

            //Dependency Injection for Entity Framework + SQLite
            ConfigureServices_DataAccess(services, configuration);

            //e-exception, t-timeout, a-default
            services.AddTransient<ResilientTestService>();

            // Dependency Injection and Register for Directors
            services.AddTransient<BookDirector>();
            services.AddTransient<PersonDirector>();


            //Register resilience pipeline with policies for Retry, Circuit Breaker, Timeout
            //Register resilience pipeline for RabbitMQ Message Publisher
            //Configure Message Publisher services (ResilientMessagePublisher => (RabbitMQMessagePublisher, EmptyMessagePublisher)) 
            ConfigureServices_MessagePublish_ResiliencePipeline_BuildPolicies_InjectMessagePublisherService(services, configuration);

            //Register resilience pipeline with policies for Retry, Circuit Breaker, Timeout
            //Register resilience pipeline for Redis Cache Service
            //Configure Cache services (ResilientCacheService => (RedisCacheService, InMemoryCacheService)) 
            ConfigureServices_CacheService_ResiliencePipeline_BuildPolicies_InjectCacheService(services, configuration);

            ConfigureServices_RateLimitingService(services, configuration);

            return services;
        }

        private static void ConfigureServices_RateLimitingService(
            IServiceCollection services, IConfiguration configuration)
        {
            // Configure Rate Limiting settings
            services.Configure<RateLimitingSettings>(configuration.GetSection("RateLimiting"));

            // Configure Rate Limiting services
            var rateLimitingSettings = configuration.GetSection("RateLimiting").Get<RateLimitingSettings>();

            services.AddSingleton<IRateLimitingService, InMemoryRateLimitingService>();
        }

        //Register resilience pipeline with policies for Retry, Circuit Breaker, Timeout
        //Register resilience pipeline for InMemory Cache Service
        //Configure Cache services (ResilientCacheService => (RedisCacheService or InMemoryCacheService)) 
        private static void ConfigureServices_CacheService_ResiliencePipeline_BuildPolicies_InjectCacheService(
            IServiceCollection services, IConfiguration configuration)
        {
            // Configure Cache services
            //services.Configure<CacheSettings>(configuration.GetSection("Cache"));

            // Register InMemory or Redis cache resilience pipeline builder (ResiliencePipelineProvider)

            services.AddResiliencePipeline("InMemoryCacheService", pipelineBuilder =>
            {

                /*
                 * Once there have been at least 2 executions within the 30-second sampling window, 
                 * if at least 30% ( = 0.3) are considered failures, open the circuit.
                 * Meaning out of 2 processed, even if 1 fails in 30 secs its an case for open circuit.
                 * After 60 seconds circuit should get closed.
                 */
                pipelineBuilder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.3,
                    MinimumThroughput = 2,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    BreakDuration = TimeSpan.FromSeconds(60)
                });


                /*
                 * Timeout strategy is cooperative: it cancels the execution via the CancellationToken; 
                 *  15 seconds  ->  Polly cancels token ->  Task.Delay observes cancellation  ↓ 
                    Operation stops  ->  Polly reports TimeoutRejectedException
                */

                pipelineBuilder.AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(10)
                });

                /*
                * -MaxRetryAttempts means 3 retries after the initial attempt, so 4 total attempts.
                * -Exponential backoff + jitter, there can also be additional waiting time between attempts.
                * -Jitter adds some randomness to the delay to avoid
                * multiple requests retrying at exactly the same time.
                */
                pipelineBuilder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                });
            });

            services.AddSingleton<InMemoryCacheService>();

            services.AddSingleton<ICacheService>(sp =>
            {
                var innerCacheService = sp.GetRequiredService<InMemoryCacheService>();

                var pipelineProvider = sp.GetRequiredService<ResiliencePipelineProvider<string>>();

                var logger = sp.GetRequiredService<ILogger<ResilientCacheService>>();

                return new ResilientCacheService(innerCacheService, pipelineProvider, logger);
            });
        }

        //Register resilience pipeline with policies for Retry, Circuit Breaker, Timeout
        //Register resilience pipeline for RabbitMQ Message Publisher
        //Configure Message Publisher services (ResilientMessagePublisher => (RabbitMQMessagePublisher, EmptyMessagePublisher)) 
        private static void ConfigureServices_MessagePublish_ResiliencePipeline_BuildPolicies_InjectMessagePublisherService(
        IServiceCollection services, IConfiguration configuration)
        {
            // Register RabbitMQ resilience pipeline
            //4time it required to test timeout
            services.AddResiliencePipeline("InMemoryMessagePublisher", pipelineBuilder =>
            {
                //var resilienceSettings = configuration.GetSection("Resilience:RabbitMQ").Get<ServiceResilienceSettings>();

                pipelineBuilder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.3,
                    MinimumThroughput = 2,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    BreakDuration = TimeSpan.FromSeconds(60)
                });

                pipelineBuilder.AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(10)
                });

                pipelineBuilder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                });
            });

            services.AddScoped<InMemoryMessagePublisher>();

            // Register scoped IMessagePublisher with resilience wrapper
            services.AddScoped<IMessagePublisher>(sp =>
            {
                var innerMessagePublisher = sp.GetRequiredService<InMemoryMessagePublisher>();

                var pipelineProvider = sp.GetRequiredService<ResiliencePipelineProvider<string>>();

                var logger = sp.GetRequiredService<ILogger<ResilientMessagePublisher>>();

                return new ResilientMessagePublisher(innerMessagePublisher, pipelineProvider, logger);
            });
        }

        //Dependency Injection for Entity Framework + SQLite
        private static void ConfigureServices_DataAccess(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SqlDataBaseDataContext>(options =>
                {
                    options.UseSqlite(configuration.GetConnectionString("SqliteDBContext"));

                    options.EnableDetailedErrors();
                    options.EnableSensitiveDataLogging();

                    options.LogTo(Console.WriteLine, LogLevel.Error);
                });

            services.AddScoped<DbContext>(sp => sp.GetRequiredService<SqlDataBaseDataContext>());

            // Repositories
            services.AddScoped<IEntityGenericRepository<Book>, EntityFrameworkBookRepository>();
            services.AddScoped<IEntityGenericRepository<Person>, EntityFrameworkPersonRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, SqlDatabaseUnitOfWork>();
        }
    }
}