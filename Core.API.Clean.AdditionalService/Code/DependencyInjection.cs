using Core.API.AdditionalServiceLibrary;
using Core.Library.ArivuTharavuThalam;
using Microsoft.EntityFrameworkCore;

namespace Core.API.Clean.AdditionalService
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration>(configuration);

            // Logger
            //services.AddSingleton<ServiceActionLogger>();

            ConfigureServices_DataAccess(services, configuration);

            return services;
        }

        private static void ConfigureServices_DataAccess(IServiceCollection services, IConfiguration configuration)
        {
            // Add Entity Framework + SQLite

            services.AddDbContext<SqlDataBaseDataContext>(options =>
               options.UseSqlite(
                   configuration.GetConnectionString("SqliteDBContext")
               ));


            // Repositories
            services.AddScoped<IEntityGenericRepository<Book>, EntityFrameworkBookRepository>();
            services.AddScoped<IEntityGenericRepository<Person>, EntityFrameworkPersonRepository>();

            // Unit of Work
            services.AddScoped<IUnitOfWork, SqlDatabaseUnitOfWork>();

            // Add Unit of Work and Directors
            services.AddTransient<BookDirector>();
            services.AddTransient<PersonDirector>();
        }
    }
}


