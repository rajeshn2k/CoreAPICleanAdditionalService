using Core.Library.Clean.AdditionalService;
using Core.API.Clean.AdditionalService.Cache;
using Core.API.Clean.AdditionalService.Messaging;
using Core.API.Clean.AdditionalService.CircuitBreaker;
using Core.API.Clean.AdditionalService.RateLimiting;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using System.Linq;

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

            // Add Unit of Work and Directors
            services.AddTransient<BookDirector>();
            services.AddTransient<PersonDirector>();

            // Configure API Response settings
            services.Configure<ApiResponseSettings>(
                configuration.GetSection("ApiResponse"));

            // Configure Circuit Breaker settings
            services.Configure<CircuitBreakerSettings>(
                configuration.GetSection("CircuitBreaker"));

            // Configure and register Circuit Breaker service
            services.AddSingleton<ICircuitBreakerService>(sp =>
            {
                var circuitBreakerService = new CircuitBreakerService(sp.GetRequiredService<ILogger<CircuitBreakerService>>());
                var circuitBreakerSettings = configuration.GetSection("CircuitBreaker").Get<CircuitBreakerSettings>();
                var logger = sp.GetRequiredService<ILogger<CircuitBreakerService>>();

                if (circuitBreakerSettings?.Redis != null)
                {
                    var redisPolicy = CircuitBreakerPolicyFactory.CreateCircuitBreakerPolicy(
                        "RedisCache",
                        circuitBreakerSettings.Redis.ExceptionsAllowedBeforeBreaking,
                        TimeSpan.FromSeconds(circuitBreakerSettings.Redis.DurationOfBreakInSeconds),
                        logger);

                    var redisRetryPolicy = CircuitBreakerPolicyFactory.CreateRetryPolicy(
                        "RedisCache",
                        circuitBreakerSettings.Redis.RetryCount,
                        TimeSpan.FromSeconds(circuitBreakerSettings.Redis.RetryDelayInSeconds),
                        logger);

                    var redisTimeoutPolicy = CircuitBreakerPolicyFactory.CreateTimeoutPolicy(
                        "RedisCache",
                        TimeSpan.FromSeconds(circuitBreakerSettings.Redis.TimeoutInSeconds),
                        logger);

                    var combinedRedisPolicy = Policy.WrapAsync(redisTimeoutPolicy, redisRetryPolicy, redisPolicy);
                    circuitBreakerService.AddPolicy("RedisCache", combinedRedisPolicy);
                }

                if (circuitBreakerSettings?.RabbitMQ != null)
                {
                    var rabbitMQPolicy = CircuitBreakerPolicyFactory.CreateCircuitBreakerPolicy(
                        "RabbitMQ",
                        circuitBreakerSettings.RabbitMQ.ExceptionsAllowedBeforeBreaking,
                        TimeSpan.FromSeconds(circuitBreakerSettings.RabbitMQ.DurationOfBreakInSeconds),
                        logger);

                    var rabbitMQRetryPolicy = CircuitBreakerPolicyFactory.CreateRetryPolicy(
                        "RabbitMQ",
                        circuitBreakerSettings.RabbitMQ.RetryCount,
                        TimeSpan.FromSeconds(circuitBreakerSettings.RabbitMQ.RetryDelayInSeconds),
                        logger);

                    var rabbitMQTimeoutPolicy = CircuitBreakerPolicyFactory.CreateTimeoutPolicy(
                        "RabbitMQ",
                        TimeSpan.FromSeconds(circuitBreakerSettings.RabbitMQ.TimeoutInSeconds),
                        logger);

                    var combinedRabbitMQPolicy = Policy.WrapAsync(rabbitMQTimeoutPolicy, rabbitMQRetryPolicy, rabbitMQPolicy);
                    circuitBreakerService.AddPolicy("RabbitMQ", combinedRabbitMQPolicy);
                }

                return circuitBreakerService;
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
                            sp.GetRequiredService<ILogger<RedisCacheService>>(),
                            cacheSettings);
                        var circuitBreakerService = sp.GetRequiredService<ICircuitBreakerService>();
                        var logger = sp.GetRequiredService<ILogger<CircuitBreakerCacheService>>();
                        return new CircuitBreakerCacheService(innerCacheService, circuitBreakerService, logger);
                    });
                }
                catch
                {
                    // Fallback to in-memory cache if Redis is unavailable
                    services.AddSingleton<ICacheService>(sp =>
                    {
                        var innerCacheService = new InMemoryCacheService(
                            sp.GetRequiredService<ILogger<InMemoryCacheService>>(),
                            cacheSettings);
                        var circuitBreakerService = sp.GetRequiredService<ICircuitBreakerService>();
                        var logger = sp.GetRequiredService<ILogger<CircuitBreakerCacheService>>();
                        return new CircuitBreakerCacheService(innerCacheService, circuitBreakerService, logger);
                    });
                }
            }
            else
            {
                services.AddSingleton<ICacheService>(sp =>
                {
                    var innerCacheService = new InMemoryCacheService(
                        sp.GetRequiredService<ILogger<InMemoryCacheService>>(),
                        cacheSettings);
                    var circuitBreakerService = sp.GetRequiredService<ICircuitBreakerService>();
                    var logger = sp.GetRequiredService<ILogger<CircuitBreakerCacheService>>();
                    return new CircuitBreakerCacheService(innerCacheService, circuitBreakerService, logger);
                });
            }

            // Configure Messaging services
            services.Configure<MessagingSettings>(
                configuration.GetSection("Messaging"));

            var messagingSettings = configuration.GetSection("Messaging").Get<MessagingSettings>();
            IMessagePublisher baseMessagePublisher;
            
            if (messagingSettings != null && messagingSettings.EnableMessaging)
            {
                try
                {
                    baseMessagePublisher = new RabbitMQMessagePublisher(
                        messagingSettings);
                }
                catch
                {
                    // Fallback to empty publisher if RabbitMQ is unavailable
                    baseMessagePublisher = new EmptyMessagePublisher();
                }
            }
            else
            {
                baseMessagePublisher = new EmptyMessagePublisher();
            }

            // Register circuit breaker wrapped message publisher as scoped
            services.AddScoped<IMessagePublisher>(sp =>
            {
                var circuitBreakerService = sp.GetRequiredService<ICircuitBreakerService>();
                var logger = sp.GetRequiredService<ILogger<CircuitBreakerMessagePublisher>>();
                return new CircuitBreakerMessagePublisher(baseMessagePublisher, circuitBreakerService, logger);
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
            // Add Entity Framework + SQLite

            //services.AddDbContext<SqlDataBaseDataContext>(options =>
            //   options.UseSqlite(
            //       configuration.GetConnectionString("SqliteDBContext")
            //   ));

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


