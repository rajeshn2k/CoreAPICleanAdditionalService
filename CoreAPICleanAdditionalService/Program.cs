using Asp.Versioning;
using Core.API.Clean.AdditionalService;
using Core.Library.Clean.AdditionalService;

public static class Program
{
    public static void Main(string[] args)
    {
        string assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name ?? "AdditionalService";

        var builder = WebApplication.CreateBuilder(args);

        // ------------------------------------------------------------
        // Configuration
        // ------------------------------------------------------------

        string environment = builder.Environment.EnvironmentName;

        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

        // ------------------------------------------------------------
        // Kestrel
        // ------------------------------------------------------------

        string kestrelEndpointUrl = builder.Configuration
                .GetSection("Kestrel:Endpoints:Http:Url").Value ?? "http://localhost:8080";

        builder.WebHost.ConfigureKestrel(options =>
        {
            var uri = new Uri(kestrelEndpointUrl);

            options.ListenAnyIP(uri.Port, listenOptions =>
            {
                if (uri.Scheme.Equals(
                        "https",
                        StringComparison.OrdinalIgnoreCase))
                {
                    listenOptions.UseHttps();
                }
            });
        });

        // ------------------------------------------------------------
        // Application Services
        // ------------------------------------------------------------

        builder.Services.AddApplicationServices(builder.Configuration);

        //builder.Services.AddHostedService<InMemoryMessageConsumer>();

        // ------------------------------------------------------------
        // CORS
        // ------------------------------------------------------------

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // ------------------------------------------------------------
        // Controllers
        // ------------------------------------------------------------

        builder.Services.AddControllers();

        // ------------------------------------------------------------
        // API Versioning
        // ------------------------------------------------------------

        builder.Services
            .AddApiVersioning(options =>
            {
                // Default API version
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // If no version is specified, use v1
                options.AssumeDefaultVersionWhenUnspecified = true;

                // Adds response headers such as:
                // api-supported-versions: 1.0, 2.0
                options.ReportApiVersions = true;

                // Support multiple version readers:
                //
                // URL:
                //   /api/v2/Book
                //
                // Header:
                //   X-Api-Version: 2.0
                //
                // Query string:
                //   /api/Book?api-version=2.0
                //
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"),
                    new QueryStringApiVersionReader("api-version"));
            })
            .AddApiExplorer(options =>
            {
                // Generates groups: (v1)(v2)
                options.GroupNameFormat = "'v'VVV";

                // Replaces {version:apiVersion} with the
                // actual version in generated API descriptions.
                options.SubstituteApiVersionInUrl = true;
            });

        // ------------------------------------------------------------
        // OpenAPI - V1
        // ------------------------------------------------------------

        builder.Services.AddOpenApi("v1", options =>
        {
            options.ShouldInclude = description => description.GroupName == "v1";
        });

        // ------------------------------------------------------------
        // OpenAPI - V2
        // ------------------------------------------------------------

        builder.Services.AddOpenApi("v2", options =>
        {
            options.ShouldInclude = description => description.GroupName == "v2";
        });

        // ------------------------------------------------------------
        // Build
        // ------------------------------------------------------------

        var app = builder.Build();

        // ------------------------------------------------------------
        // Correlation ID
        // ------------------------------------------------------------

        app.UseCorrelationId();

        // ------------------------------------------------------------
        // Global Exception Handler
        // ------------------------------------------------------------

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        // ------------------------------------------------------------
        // Rate Limiting
        // ------------------------------------------------------------

        app.UseMiddleware<RateLimitingMiddleware>();

        // ------------------------------------------------------------
        // CORS
        // ------------------------------------------------------------

        app.UseCors("AllowAll");

        // ------------------------------------------------------------
        // OpenAPI / Swagger
        // ------------------------------------------------------------

        if (app.Environment.IsDevelopment())
        {
            // --------------------------------------------------------
            // OpenAPI JSON documents
            //
            // /openapi/v1.json
            // /openapi/v2.json
            // --------------------------------------------------------

            app.MapOpenApi("/openapi/{documentName}.json");

            // --------------------------------------------------------
            // Swagger UI
            //
            // /swagger
            // --------------------------------------------------------

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(
                    "/openapi/v1.json",
                    $"{assemblyName} v1");

                options.SwaggerEndpoint(
                    "/openapi/v2.json",
                    $"{assemblyName} v2");

                options.RoutePrefix = "swagger";
            });
        }

        // ------------------------------------------------------------
        // Authorization
        // ------------------------------------------------------------

        app.UseAuthorization();

        // ------------------------------------------------------------
        // Controllers
        // ------------------------------------------------------------

        app.MapControllers();

        // ------------------------------------------------------------
        // Database
        // ------------------------------------------------------------

        CreateDbIfNotExists(app);

        // ------------------------------------------------------------
        // Run
        // ------------------------------------------------------------

        app.Run();
    }

    // ------------------------------------------------------------
    // Database Initialization
    // ------------------------------------------------------------

    private static void CreateDbIfNotExists(IHost host)
    {
        using var scope = host.Services.CreateScope();

        var services = scope.ServiceProvider;

        var dbContext =
            services.GetRequiredService<SqlDataBaseDataContext>();

        dbContext.Database.EnsureCreated();
    }
}