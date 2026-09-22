using Core.Library.Clean.AdditionalService;
using Microsoft.EntityFrameworkCore;

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


