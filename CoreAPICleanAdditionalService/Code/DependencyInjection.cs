using Core.Library.Clean.AdditionalService;
using Core.API.Clean.AdditionalService.Resilience;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Resilience;
using Polly;

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

            ConfigureServices_DataAccess(services, configuration);

            // Dependency Injection and Register for Directors
            services.AddTransient<BookDirector>();
            services.AddTransient<PersonDirector>();

            // Configure API Response settings
            services.Configure<ApiResponseSettings>(
                configuration.GetSection("ApiResponse"));

            // Configure Resilience settings
            services.Configure<ResilienceSettings>(
                configuration.GetSection("Resilience"));

            // Register RabbitMQ resilience pipeline
            services.AddResiliencePipeline("RabbitMQ", pipelineBuilder =>
            {
                var resilienceSettings = configuration.GetSection("Resilience:RabbitMQ").Get<ServiceResilienceSettings>();
                
                pipelineBuilder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = resilienceSettings?.Retry.MaxRetryAttempts ?? 5,
                    Delay = TimeSpan.FromSeconds(resilienceSettings?.Retry.DelaySeconds ?? 2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                });

                pipelineBuilder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = resilienceSettings?.CircuitBreaker.FailureRatio ?? 0.5,
                    MinimumThroughput = resilienceSettings?.CircuitBreaker.MinimumThroughput ?? 10,
                    SamplingDuration = TimeSpan.FromSeconds(resilienceSettings?.CircuitBreaker.SamplingDurationSeconds ?? 30),
                    BreakDuration = TimeSpan.FromSeconds(resilienceSettings?.CircuitBreaker.BreakDurationSeconds ?? 60)
                });

                pipelineBuilder.AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(resilienceSettings?.TimeoutSeconds ?? 10)
                });
            });

            // Register Redis resilience pipeline
            services.AddResiliencePipeline("Redis", pipelineBuilder =>
            {
                var resilienceSettings = configuration.GetSection("Resilience:Redis").Get<ServiceResilienceSettings>();
                
                pipelineBuilder.AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = resilienceSettings?.Retry.MaxRetryAttempts ?? 3,
                    Delay = TimeSpan.FromSeconds(resilience?.Retry.DelaySeconds ?? 1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                });

                pipelineBuilder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = resilienceSettings?.CircuitBreaker.FailureRatio ?? 0.5,
                    MinimumThroughput = resilienceSettings?.CircuitBreaker.MinimumThroughput ?? 10,
                    SamplingDuration = TimeSpan.FromSeconds(resilience?.CircuitBreaker.SamplingDurationSeconds ?? 30),
                    BreakDuration = TimeSpan.FromSeconds(resilience?.CircuitBreaker.BreakDurationSeconds ?? 30)
                });

                pipelineBuilder.AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(resilienceSettings?.TimeoutSeconds ?? 5)
                });
            });

            // Configure Cache services
            services.Configure<CacheSettings>(
                configuration.GetSection("Cache"));

            var cacheSettings = configuration.GetSection("Cache").Get<CacheSettings>();
            if (cacheSettings != null && cacheSettings.EnableCache)
            {
                try
                {
                    services.AddSingleton<IConnectionMultiplexer>(sp =>
                    {
                        var config = ConfigurationOptions.Parse(cacheSettings.ConnectionString);
                        return ConnectionMultiplexer.Connect(config);
                    });

                    services.AddSingleton<ICacheService>(sp =>
                    {
                        var innerCacheService = new RedisCacheService(
                            sp.GetRequiredService<IConnectionMultiplexer>(),
                            sp.GetRequiredService<IOptions<CacheSettings>>(),
                            sp.GetRequiredService<ILogger<RedisCacheService>>());

                        var pipelineProvider = sp.GetRequiredService<ResiliencePipelineProvider<string>>();
                        var logger = sp.GetRequiredService<ILogger<ResilientCacheService>>();

                        return new ResilientCacheService(innerCacheService, pipelineProvider, logger);
                    });
                }
                catch
                {
                    // Fallback to in-memory cache if Redis is unavailable
                    services.AddSingleton<ICacheService>(sp =>
                    {
                        var innerCacheService = new InMemoryCacheService(
                            sp.GetRequiredService<IOptions<CacheSettings>>(),
                            sp.GetRequiredService<ILogger<InMemoryCacheService>>());

                        var pipelineProvider = sp.GetRequiredService<ResiliencePipelineProvider<string>>();
                        var logger = sp.GetRequiredService<ILogger<ResilientCacheService>>();

                        return new ResilientCacheService(innerCacheService, pipelineProvider, logger);
                    });
                }
            }
            else
            {
                services.AddSingleton<ICacheService>(sp =>
                {
                    var innerCacheService = new InMemoryCacheService(
                        sp.GetRequiredService<IOptions<CacheSettings>>(),
                        sp.GetRequiredService<ILogger<InMemoryCacheService>>() );

                    var pipelineProvider = sp.GetRequiredService<ResiliencePipelineProvider<string>>();
                    var logger = sp.GetRequiredService<ILogger<ResilientCacheService>>();

                    return new ResilientCacheService(innerCacheService, pipelineProvider, logger);
                });
            }

            // Configure Messaging services
            services.Configure<MessagingSettings>(
                configuration.GetSection("Messaging"));

            // Register scoped IMessagePublisher with resilience wrapper
            services.AddScoped<IMessagePublisher>(sp =>
            {
                var messagingOptions = sp.GetRequiredService<IOptions<MessagingSettings>>();
                var messagingSettings = messagingOptions.Value;
                IMessagePublisher baseMessagePublisher;

                if (messagingSettings != null && messagingSettings.EnableMessaging)
                {
                    try
                    {
                        baseMessagePublisher = new RabbitMQMessagePublisher(
                            messagingOptions,
                            sp.GetRequiredService<ILogger<RabbitMQMessagePublisher>>());
                    }
                    catch
                    {
                        // Fallback to empty publisher if RabbitMQ is unavailable
                        baseMessagePublisher = new EmptyMessagePublisher(sp.GetRequiredService<ILogger<EmptyMessagePublisher>>());
                    }
                }
                else
                {
                    baseMessagePublisher = new EmptyMessagePublisher(sp.GetRequiredService<ILogger<EmptyMessagePublisher>>());
                }

                var pipelineProvider = sp.GetRequiredService<ResiliencePipelineProvider<string>>();
                var logger = sp.GetRequiredService<ILogger<ResilientMessagePublisher>>();
                return new ResilientMessagePublisher(baseMessagePublisher, pipelineProvider, logger);
            });

            // Configure Rate Limiting settings
            services.Configure<RateLimitingSettings>(
                configuration.GetSection("RateLimiting"));

            // Configure Rate Limiting services
            var rateLimitingSettings = configuration.GetSection("RateLimiting").Get<RateLimitingSettings>();

            if (rateLimitingSettings != null && rateLimitingSettings.EnableRateLimiting)
            {
                if (rateLimitingSettings.UseDistributedRateLimiting && cacheSettings != null && cacheSettings.EnableCache)
                {
                    services.AddSingleton<IRateLimitingService>(sp =>
                    {
                        var connectionMultiplexer = sp.GetService<IConnectionMultiplexer>();
                        if (connectionMultiplexer != null)
                        {
                            return new RedisRateLimitingService(connectionMultiplexer, sp.GetRequiredService<ILogger<RedisRateLimitingService>>());
                        }
                        // Fallback to in-memory rate limiting if Redis is unavailable
                        return new InMemoryRateLimitingService(sp.GetRequiredService<ILogger<InMemoryRateLimitingService>>());
                    });
                }
                else
                {
                    services.AddSingleton<IRateLimitingService, InMemoryRateLimitingService>();
                }
            }
            else
            {
                services.AddSingleton<IRateLimitingService, InMemoryRateLimitingService>();
            }

            return services;
        }

        private static void ConfigureServices_DataAccess(IServiceCollection services, IConfiguration configuration)
        {
            // Dependency Injection for Entity Framework + SQLite

            services.AddDbContext<SqlDataBaseDataContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("SqliteDBContext"));

                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();

                options.LogTo(Console.WriteLine, LogLevel.Information);
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


