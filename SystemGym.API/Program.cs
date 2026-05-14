using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using FluentValidation;
using MediatR;
using AutoMapper;

using SystemGym.Infrastructure.Persistence;
using SystemGym.Application.Abstractions;
using SystemGym.Application.Mappings;
using SystemGym.Application.Validators;
using SystemGym.Application.Features.Subscriptions.EventHandlers;
using SystemGym.API.Services;
using SystemGym.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ==================== LOGGING SETUP ====================
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        Path.Combine("logs", "systemgym-.txt"),
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "SystemGym.API")
    .CreateLogger();

builder.Host.UseSerilog();

// ==================== SERVICES CONFIGURATION ====================

// ---- Database Configuration ----
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' not found.");

builder.Services.AddDbContext<SystemGymDbContext>(options =>
    options.UseNpgsql(connectionString,
        npgsqlOptions => npgsqlOptions
            .MigrationsAssembly(typeof(SystemGymDbContext).Assembly.GetName().Name)
            .EnableRetryOnFailure(maxRetryCount: 3)));

// ---- Unit of Work & Repositories ----
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// ---- MediatR Configuration ----
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(SubscriptionCreatedEventHandler).Assembly,
        AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.FullName?.Contains("SystemGym.Application") ?? false) ?? typeof(SubscriptionCreatedEventHandler).Assembly);
});

// ---- FluentValidation ----
builder.Services.AddValidatorsFromAssemblyContaining<CreateSystemUserValidator>();

// ---- AutoMapper ----
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<SystemUserMappingProfile>();
    cfg.AddProfile<ClientMappingProfile>();
    cfg.AddProfile<ProductMappingProfile>();
    cfg.AddProfile<PlanMappingProfile>();
    cfg.AddProfile<SubscriptionMappingProfile>();
    cfg.AddProfile<SalesMappingProfile>();
    cfg.AddProfile<InventoryMappingProfile>();
});

// ---- JWT Authentication ----
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "your-secret-key-min-32-characters-long!");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// ---- Authorization ----
builder.Services.AddAuthorization();

// ---- Controllers ----
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// ---- API Documentation (Swagger) ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "SystemGym API",
        Version = "v1.0",
        Description = "API para la gestión de gimnasios: usuarios, clientes, planes, suscripciones, ventas e inventario.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "SystemGym Team",
            Email = "support@systemgym.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "Privado — Uso interno"
        }
    });

    // Incluir comentarios XML generados desde el proyecto API
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);

    // Configurar seguridad JWT en Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Introduce el token JWT con el prefijo Bearer. Ejemplo: Bearer {token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Agrupar por tag para mejor organización
    c.TagActionsBy(api => [api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "General"]);
    c.DocInclusionPredicate((name, api) => true);
});

// ---- CORS Configuration ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", corsBuilder =>
    {
        corsBuilder
            .WithOrigins("http://localhost:3000", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// ==================== BUILD & CONFIGURE APP ====================

var app = builder.Build();

// ---- Middleware Pipeline ----
// Swagger disponible en todos los entornos (acceso interno al localhost)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SystemGym API v1.0");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "SystemGym API - Documentación";
    c.DefaultModelsExpandDepth(-1); // Ocultar sección Schemas por defecto
    c.EnableFilter();
    c.EnableDeepLinking();
});

app.UseSerilogRequestLogging();

app.UseGlobalExceptionHandler();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint (usado por Docker health checks)
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .AllowAnonymous();

app.MapControllers();

try
{
    // Apply migrations automatically on startup
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<SystemGymDbContext>();
        await dbContext.Database.MigrateAsync();
        Log.Information("Database migrations applied successfully");
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
