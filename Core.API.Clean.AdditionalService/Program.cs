using Core.API.AdditionalServiceLibrary;
using Core.API.Clean.AdditionalService;

public static class Program
{
    public static void Main(string[] args)
    {
        string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "AdditionalService";

        // Resolve environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrEmpty(environment))
            environment = "Development";

        // Build configuration
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

        // Read Kestrel endpoint
        string kestrelEndpointUrl = config.GetSection("Kestrel:Endpoints:Http:Url").Value ?? "http://localhost:8080";

        var builder = WebApplication.CreateBuilder(args);

        // Apply configuration + Kestrel
        builder.Configuration.AddConfiguration(config);

        builder.WebHost.ConfigureKestrel(options =>
        {
            if (!string.IsNullOrEmpty(kestrelEndpointUrl))
            {
                var uri = new Uri(kestrelEndpointUrl);

                options.ListenAnyIP(uri.Port, listenOptions =>
                {
                    if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                    {
                        listenOptions.UseHttps();
                    }
                });
            }
        });

        // Dependency Injection - Application Services
        builder.Services.AddApplicationServices(builder.Configuration);

        // Add CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        // Services
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddEndpointsApiExplorer();

        var app = builder.Build();

        // Use CORS
        app.UseCors("AllowAll");

        // Pipeline
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", $"{assemblyName} v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        CreateDbIfNotExists(app);

        app.Run();
    }
    private static void CreateDbIfNotExists(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<SqlDataBaseDataContext>();

        // Create database if it does not exist
        dbContext.Database.EnsureCreated();

        // Optional seed logic
        // RetrySQLDB.Retry(dbContext);
        // DatabaseInitializerPerson.InitializeSQLDB(dbContext);
    }
}
