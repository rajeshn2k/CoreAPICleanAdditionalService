using Core.Library.Clean.AdditionalService;
using Core.API.Clean.AdditionalService.Cache;
using Core.API.Clean.AdditionalService.Messaging;
using Microsoft.EntityFrameworkCore;
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

            ConfigureServices_DataAccess(services, configuration);

            // Add Unit of Work and Directors
            services.AddTransient<BookDirector>();
            services.AddTransient<PersonDirector>();

            services.AddScoped<IMessagePublisher, EmptyMessagePublisher>();

            // Configure API Response settings
            services.Configure<ApiResponseSettings>(
                configuration.GetSection("ApiResponse"));

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

                    services.AddSingleton<ICacheService, RedisCacheService>();
                }
                catch
                {
                    // Fallback to in-memory cache if Redis is unavailable
                    services.AddSingleton<ICacheService, InMemoryCacheService>();
                }
            }
            else
            {
                services.AddSingleton<ICacheService, InMemoryCacheService>();
            }

            // Configure Messaging services
            services.Configure<MessagingSettings>(
                configuration.GetSection("Messaging"));

            var messagingSettings = configuration.GetSection("Messaging").Get<MessagingSettings>();
            if (messagingSettings != null && messagingSettings.EnableMessaging)
            {
                try
                {
                    services.AddSingleton<IMessagePublisher, RabbitMQMessagePublisher>();
                }
                catch
                {
                    // Fallback to empty publisher if RabbitMQ is unavailable
                    services.AddScoped<IMessagePublisher, EmptyMessagePublisher>();
                }
            }
            else
            {
                services.AddScoped<IMessagePublisher, EmptyMessagePublisher>();
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


